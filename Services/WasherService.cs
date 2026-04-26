using CarWash.Backend.DTOs.Booking;
using CarWash.Backend.DTOs.Washer;
using CarWash.Backend.Models;
using CarWash.Backend.Repositories.Interfaces;
using CarWash.Backend.Services.Interfaces;

namespace CarWash.Backend.Services;

public class WasherService : IWasherService
{
    private static readonly Dictionary<string, List<string>> AllowedWasherStatusTransitions = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Confirmed"] = new() { "InProgress" },
        ["InProgress"] = new() { "Completed" }
    };

    private readonly IBookingRepository _bookingRepository;
    private readonly IEmailService _emailService;

    public WasherService(IBookingRepository bookingRepository, IEmailService emailService)
    {
        _bookingRepository = bookingRepository;
        _emailService = emailService;
    }

    public async Task<List<BookingResponse>> GetAssignedBookingsAsync(int washerId)
    {
        var bookings = await _bookingRepository.GetByAssignedWasherIdAsync(washerId);

        return bookings
            .Select(booking => MapToResponse(booking, "Assigned bookings fetched successfully."))
            .ToList();
    }

    public async Task<BookingResponse?> GetAssignedBookingByIdAsync(int id, int washerId)
    {
        var booking = await _bookingRepository.GetByIdAndAssignedWasherIdAsync(id, washerId);

        return booking == null ? null : MapToResponse(booking, "Assigned booking fetched successfully.");
    }

    public async Task<WasherServiceResult> UpdateAssignedBookingStatusAsync(int id, int washerId, UpdateAssignedBookingStatusRequest request)
    {
        var booking = await _bookingRepository.GetByIdAndAssignedWasherIdAsync(id, washerId);

        if (booking == null)
        {
            return new WasherServiceResult
            {
                IsNotFound = true,
                ErrorMessage = "Assigned booking not found."
            };
        }

        var normalizedStatus = request.Status.Trim();

        if (booking.Status.Equals(normalizedStatus, StringComparison.OrdinalIgnoreCase))
        {
            return new WasherServiceResult
            {
                ErrorMessage = "Booking already has this status."
            };
        }

        if (!IsValidWasherTransition(booking.Status, normalizedStatus))
        {
            return new WasherServiceResult
            {
                ErrorMessage = $"Washer cannot change booking status from {booking.Status} to {normalizedStatus}."
            };
        }

        booking.Status = normalizedStatus;
        booking.UpdatedAt = DateTime.UtcNow;

        await _bookingRepository.SaveChangesAsync();

        if (booking.Status.Equals("Completed", StringComparison.OrdinalIgnoreCase) &&
            booking.User != null &&
            !string.IsNullOrWhiteSpace(booking.User.Email))
        {
            await _emailService.SendEmailAsync(
                booking.User.Email,
                "Booking Completed",
                $"<p>Hello {booking.User.FullName},</p><p>Your booking <strong>#{booking.Id}</strong> has been completed successfully.</p>",
                isHtml: true);
        }

        return new WasherServiceResult
        {
            IsSuccess = true,
            Response = MapToResponse(booking, "Assigned booking status updated successfully.")
        };
    }

    private static bool IsValidWasherTransition(string currentStatus, string newStatus)
    {
        if (!AllowedWasherStatusTransitions.TryGetValue(currentStatus, out var nextStatuses))
        {
            return false;
        }

        return nextStatuses.Contains(newStatus, StringComparer.OrdinalIgnoreCase);
    }

    private static BookingResponse MapToResponse(Booking booking, string message)
    {
        return new BookingResponse
        {
            Id = booking.Id,
            UserId = booking.UserId,
            CarId = booking.CarId,
            ServicePlanId = booking.ServicePlanId,
            PromoCodeId = booking.PromoCodeId,
            AssignedWasherId = booking.AssignedWasherId,
            CarNumber = booking.Car?.CarNumber ?? string.Empty,
            CarBrand = booking.Car?.Brand ?? string.Empty,
            CarModel = booking.Car?.Model ?? string.Empty,
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
            Notes = booking.Notes ?? string.Empty,
            Status = booking.Status,
            TotalAmount = booking.TotalAmount,
            Message = message
        };
    }
}
