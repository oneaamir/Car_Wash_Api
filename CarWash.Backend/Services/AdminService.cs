using CarWash.Backend.DTOs.Admin;
using CarWash.Backend.DTOs.Payment;
using CarWash.Backend.Repositories.Interfaces;
using CarWash.Backend.Services.Interfaces;

namespace CarWash.Backend.Services;

public class AdminService : IAdminService
{
    private static readonly HashSet<string> AllowedBookingStatuses = new(StringComparer.OrdinalIgnoreCase)
    {
        "Pending",
        "Confirmed",
        "InProgress",
        "Completed",
        "Cancelled"
    };

    private static readonly Dictionary<string, List<string>> AllowedStatusTransitions = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Pending"] = new() { "Confirmed", "Cancelled" },
        ["Confirmed"] = new() { "InProgress", "Cancelled" },
        ["InProgress"] = new() { "Completed" },
        ["Completed"] = new(),
        ["Cancelled"] = new()
    };

    private static readonly HashSet<string> AllowedPaymentStatuses = new(StringComparer.OrdinalIgnoreCase)
    {
        "Pending",
        "Success",
        "Failed"
    };

    private static readonly Dictionary<string, List<string>> AllowedPaymentStatusTransitions = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Pending"] = new() { "Success", "Failed" },
        ["Failed"] = new() { "Success" },
        ["Success"] = new()
    };

    private readonly IUserRepository _userRepository;
    private readonly IBookingRepository _bookingRepository;
    private readonly IPaymentRepository _paymentRepository;
    private readonly IEmailService _emailService;

    public AdminService(
        IUserRepository userRepository,
        IBookingRepository bookingRepository,
        IPaymentRepository paymentRepository,
        IEmailService emailService)
    {
        _userRepository = userRepository;
        _bookingRepository = bookingRepository;
        _paymentRepository = paymentRepository;
        _emailService = emailService;
    }

    public async Task<List<UserSummaryResponse>> GetUsersAsync()
    {
        var users = await _userRepository.GetAllAsync();

        return users
            .Select(user => new UserSummaryResponse
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Phone = user.Phone,
                Role = user.Role,
                IsActive = user.IsActive,
                Message = "Users fetched successfully."
            })
            .ToList();
    }

    public async Task<List<PaymentResponse>> GetAllPaymentsAsync()
    {
        var payments = await _paymentRepository.GetAllAsync();

        return payments
            .Select(payment => MapPaymentToResponse(payment, "Payments fetched successfully."))
            .ToList();
    }

    public async Task<List<AdminBookingResponse>> GetBookingsAsync()
    {
        var bookings = await _bookingRepository.GetAllAsync();

        return bookings
            .Select(booking => MapBookingToResponse(booking, "Bookings fetched successfully."))
            .ToList();
    }

    public async Task<AdminServiceResult<AdminBookingResponse>> AssignWasherAsync(int id, AssignWasherRequest request)
    {
        var booking = await _bookingRepository.GetByIdAsync(id);

        if (booking == null)
        {
            return new AdminServiceResult<AdminBookingResponse>
            {
                IsNotFound = true,
                ErrorMessage = "Booking not found."
            };
        }

        if (booking.Status.Equals("Completed", StringComparison.OrdinalIgnoreCase) ||
            booking.Status.Equals("Cancelled", StringComparison.OrdinalIgnoreCase))
        {
            return new AdminServiceResult<AdminBookingResponse>
            {
                ErrorMessage = "Washer cannot be assigned to completed or cancelled bookings."
            };
        }

        var washer = await _userRepository.GetWasherByIdAsync(request.WasherId);

        if (washer == null)
        {
            return new AdminServiceResult<AdminBookingResponse>
            {
                ErrorMessage = "Washer not found."
            };
        }

        if (booking.AssignedWasherId == washer.Id)
        {
            return new AdminServiceResult<AdminBookingResponse>
            {
                ErrorMessage = "This washer is already assigned to the booking."
            };
        }

        booking.AssignedWasherId = washer.Id;
        booking.AssignedWasher = washer;
        booking.UpdatedAt = DateTime.UtcNow;

        await _bookingRepository.SaveChangesAsync();
        await SendWasherAssignmentEmailAsync(booking, washer);

        return new AdminServiceResult<AdminBookingResponse>
        {
            IsSuccess = true,
            Response = MapBookingToResponse(booking, "Washer assigned successfully.")
        };
    }

    public async Task<AdminServiceResult<AdminBookingResponse>> UpdateBookingStatusAsync(int id, UpdateBookingStatusRequest request)
    {
        var booking = await _bookingRepository.GetByIdAsync(id);

        if (booking == null)
        {
            return new AdminServiceResult<AdminBookingResponse>
            {
                IsNotFound = true,
                ErrorMessage = "Booking not found."
            };
        }

        var normalizedStatus = request.Status.Trim();

        if (!AllowedBookingStatuses.Contains(normalizedStatus))
        {
            return new AdminServiceResult<AdminBookingResponse>
            {
                ErrorMessage = "Invalid booking status."
            };
        }

        if (booking.Status.Equals(normalizedStatus, StringComparison.OrdinalIgnoreCase))
        {
            return new AdminServiceResult<AdminBookingResponse>
            {
                ErrorMessage = "Booking already has this status."
            };
        }

        if (!IsValidStatusTransition(booking.Status, normalizedStatus))
        {
            return new AdminServiceResult<AdminBookingResponse>
            {
                ErrorMessage = $"Booking status cannot be changed from {booking.Status} to {normalizedStatus}."
            };
        }

        booking.Status = normalizedStatus;
        booking.UpdatedAt = DateTime.UtcNow;

        await _bookingRepository.SaveChangesAsync();

        return new AdminServiceResult<AdminBookingResponse>
        {
            IsSuccess = true,
            Response = MapBookingToResponse(booking, "Booking status updated successfully.")
        };
    }

    public async Task<AdminServiceResult<PaymentResponse>> UpdatePaymentStatusAsync(int id, UpdatePaymentStatusRequest request)
    {
        var payment = await _paymentRepository.GetByIdAsync(id);

        if (payment == null)
        {
            return new AdminServiceResult<PaymentResponse>
            {
                IsNotFound = true,
                ErrorMessage = "Payment not found."
            };
        }

        var normalizedStatus = request.PaymentStatus.Trim();

        if (!AllowedPaymentStatuses.Contains(normalizedStatus))
        {
            return new AdminServiceResult<PaymentResponse>
            {
                ErrorMessage = "Invalid payment status."
            };
        }

        if (payment.PaymentStatus.Equals(normalizedStatus, StringComparison.OrdinalIgnoreCase))
        {
            return new AdminServiceResult<PaymentResponse>
            {
                ErrorMessage = "Payment already has this status."
            };
        }

        if (!IsValidPaymentStatusTransition(payment.PaymentStatus, normalizedStatus))
        {
            return new AdminServiceResult<PaymentResponse>
            {
                ErrorMessage = $"Payment status cannot be changed from {payment.PaymentStatus} to {normalizedStatus}."
            };
        }

        payment.PaymentStatus = normalizedStatus;
        payment.UpdatedAt = DateTime.UtcNow;

        await _paymentRepository.SaveChangesAsync();

        if (payment.PaymentStatus.Equals("Success", StringComparison.OrdinalIgnoreCase))
        {
            var booking = await _bookingRepository.GetByIdAsync(payment.BookingId);

            if (booking?.User != null && !string.IsNullOrWhiteSpace(booking.User.Email))
            {
                await _emailService.SendEmailAsync(
                    booking.User.Email,
                    "Payment Successful",
                    $"<p>Hello {booking.User.FullName},</p><p>Your payment for booking <strong>#{booking.Id}</strong> was successful.</p>",
                    isHtml: true);
            }
        }

        return new AdminServiceResult<PaymentResponse>
        {
            IsSuccess = true,
            Response = MapPaymentToResponse(payment, "Payment status updated successfully.")
        };
    }

    private static AdminBookingResponse MapBookingToResponse(Models.Booking booking, string message)
    {
        return new AdminBookingResponse
        {
            Id = booking.Id,
            UserId = booking.UserId,
            CarId = booking.CarId,
            ServicePlanId = booking.ServicePlanId,
            PromoCodeId = booking.PromoCodeId,
            AssignedWasherId = booking.AssignedWasherId,
            CustomerName = booking.User?.FullName ?? string.Empty,
            CustomerEmail = booking.User?.Email ?? string.Empty,
            CarNumber = booking.Car?.CarNumber ?? string.Empty,
            ServicePlanName = booking.ServicePlan?.Name ?? string.Empty,
            PromoCode = booking.PromoCode?.Code ?? string.Empty,
            AssignedWasherName = booking.AssignedWasher?.FullName ?? string.Empty,
            AssignedWasherEmail = booking.AssignedWasher?.Email ?? string.Empty,
            AddOnNames = booking.BookingAddOns
                .Where(bookingAddOn => bookingAddOn.AddOn != null)
                .Select(bookingAddOn => bookingAddOn.AddOn!.Name)
                .ToList(),
            BookingType = booking.BookingType,
            BookingDate = booking.BookingDate,
            Address = booking.Address,
            Status = booking.Status,
            TotalAmount = booking.TotalAmount,
            CustomerPhone = booking.User?.Phone ?? string.Empty,
            CreatedAt = booking.CreatedAt,
            Message = message
        };
    }

    private static bool IsValidStatusTransition(string currentStatus, string newStatus)
    {
        if (!AllowedStatusTransitions.TryGetValue(currentStatus, out var nextStatuses))
        {
            return false;
        }

        return nextStatuses.Contains(newStatus, StringComparer.OrdinalIgnoreCase);
    }

    private static bool IsValidPaymentStatusTransition(string currentStatus, string newStatus)
    {
        if (!AllowedPaymentStatusTransitions.TryGetValue(currentStatus, out var nextStatuses))
        {
            return false;
        }

        return nextStatuses.Contains(newStatus, StringComparer.OrdinalIgnoreCase);
    }

    private static PaymentResponse MapPaymentToResponse(Models.Payment payment, string message)
    {
        return new PaymentResponse
        {
            Id = payment.Id,
            BookingId = payment.BookingId,
            Amount = payment.Amount,
            PaymentStatus = payment.PaymentStatus,
            TransactionRef = payment.TransactionRef,
            PaymentMethod = payment.PaymentMethod,
            CustomerName = payment.Booking?.User?.FullName ?? string.Empty,
            CustomerEmail = payment.Booking?.User?.Email ?? string.Empty,
            Message = message
        };
    }

    private async Task SendWasherAssignmentEmailAsync(Models.Booking booking, Models.User washer)
    {
        if (booking.User != null && !string.IsNullOrWhiteSpace(booking.User.Email))
        {
            await _emailService.SendEmailAsync(
                booking.User.Email,
                "Washer Assigned",
                $"<p>Hello {booking.User.FullName},</p><p>Washer <strong>{washer.FullName}</strong> has been assigned to your booking <strong>#{booking.Id}</strong>.</p>",
                isHtml: true);
        }

        if (!string.IsNullOrWhiteSpace(washer.Email))
        {
            await _emailService.SendEmailAsync(
                washer.Email,
                "New Booking Assigned",
                $"<p>Hello {washer.FullName},</p><p>Booking <strong>#{booking.Id}</strong> has been assigned to you.</p>",
                isHtml: true);
        }
    }
}
