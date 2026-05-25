using CarWash.Backend.DTOs.Booking;
using CarWash.Backend.Models;
using CarWash.Backend.Repositories.Interfaces;
using CarWash.Backend.Services.Interfaces;

namespace CarWash.Backend.Services;

public class BookingService : IBookingService
{
    private static readonly HashSet<string> CustomerCancellableStatuses = new(StringComparer.OrdinalIgnoreCase)
    {
        "Pending",
        "Confirmed"
    };

    private readonly IBookingRepository _bookingRepository;
    private readonly ICarRepository _carRepository;
    private readonly IServicePlanRepository _servicePlanRepository;
    private readonly IPromoCodeRepository _promoCodeRepository;
    private readonly IAddOnRepository _addOnRepository;

    public BookingService(
        IBookingRepository bookingRepository,
        ICarRepository carRepository,
        IServicePlanRepository servicePlanRepository,
        IPromoCodeRepository promoCodeRepository,
        IAddOnRepository addOnRepository)
    {
        _bookingRepository = bookingRepository;
        _carRepository = carRepository;
        _servicePlanRepository = servicePlanRepository;
        _promoCodeRepository = promoCodeRepository;
        _addOnRepository = addOnRepository;
    }

    public async Task<BookingServiceResult> CreateBookingAsync(int userId, CreateBookingRequest request)
    {
        var car = await _carRepository.GetActiveByIdAsync(request.CarId, userId);

        if (car == null)
        {
            return new BookingServiceResult
            {
                IsNotFound = true,
                ErrorMessage = "Car not found."
            };
        }

        var servicePlan = await _servicePlanRepository.GetActiveByIdAsync(request.ServicePlanId);

        if (servicePlan == null)
        {
            return new BookingServiceResult
            {
                IsNotFound = true,
                ErrorMessage = "Service plan not found."
            };
        }

        decimal totalAmount = servicePlan.Price;
        int? promoCodeId = null;
        PromoCode? promo = null;
        var selectedAddOns = new List<AddOn>();

        if (request.AddOnIds.Count > 0)
        {
            var distinctAddOnIds = request.AddOnIds.Distinct().ToList();
            selectedAddOns = await _addOnRepository.GetActiveByIdsAsync(distinctAddOnIds);

            if (selectedAddOns.Count != distinctAddOnIds.Count)
            {
                return new BookingServiceResult
                {
                    ErrorMessage = "One or more add-ons are invalid."
                };
            }

            totalAmount += selectedAddOns.Sum(addOn => addOn.Price);
        }

        if (!string.IsNullOrWhiteSpace(request.PromoCode))
        {
            promo = await _promoCodeRepository.GetActiveByCodeAsync(request.PromoCode);

            if (promo == null || promo.ExpiryDate < DateTime.UtcNow)
            {
                return new BookingServiceResult
                {
                    ErrorMessage = "Invalid or expired promo code."
                };
            }

            decimal discountAmount = 0;

            if (promo.DiscountType.Equals("Flat", StringComparison.OrdinalIgnoreCase))
            {
                discountAmount = promo.DiscountValue;
            }
            else if (promo.DiscountType.Equals("Percentage", StringComparison.OrdinalIgnoreCase))
            {
                discountAmount = (totalAmount * promo.DiscountValue) / 100;
            }
            else
            {
                return new BookingServiceResult
                {
                    ErrorMessage = "Invalid discount type."
                };
            }

            if (discountAmount > totalAmount)
            {
                discountAmount = totalAmount;
            }

            totalAmount -= discountAmount;
            promoCodeId = promo.Id;
        }

        var booking = new Booking
        {
            UserId = userId,
            CarId = request.CarId,
            ServicePlanId = request.ServicePlanId,
            PromoCodeId = promoCodeId,
            BookingType = request.BookingType,
            BookingDate = request.BookingDate,
            Address = request.Address,
            Notes = request.Notes,
            TotalAmount = totalAmount,
            Car = car,
            ServicePlan = servicePlan,
            PromoCode = promo,
            BookingAddOns = selectedAddOns
                .Select(addOn => new BookingAddOn
                {
                    AddOnId = addOn.Id,
                    AddOn = addOn
                })
                .ToList()
        };

        await _bookingRepository.AddAsync(booking);
        await _bookingRepository.SaveChangesAsync();

        return new BookingServiceResult
        {
            IsSuccess = true,
            Response = MapToResponse(booking, "Booking created successfully.")
        };
    }

    public async Task<List<BookingResponse>> GetMyBookingsAsync(int userId)
    {
        var bookings = await _bookingRepository.GetByUserIdAsync(userId);

        return bookings
            .Select(booking => MapToResponse(booking, "Bookings fetched successfully."))
            .ToList();
    }

    public async Task<BookingResponse?> GetBookingByIdAsync(int id, int userId)
    {
        var booking = await _bookingRepository.GetByIdAndUserIdAsync(id, userId);

        return booking == null ? null : MapToResponse(booking, "Booking fetched successfully.");
    }

    public async Task<BookingServiceResult> CancelBookingAsync(int id, int userId)
    {
        var booking = await _bookingRepository.GetByIdAndUserIdAsync(id, userId);

        if (booking == null)
        {
            return new BookingServiceResult
            {
                IsNotFound = true,
                ErrorMessage = "Booking not found."
            };
        }

        if (booking.Status.Equals("Cancelled", StringComparison.OrdinalIgnoreCase))
        {
            return new BookingServiceResult
            {
                ErrorMessage = "Booking is already cancelled."
            };
        }

        if (!CustomerCancellableStatuses.Contains(booking.Status))
        {
            return new BookingServiceResult
            {
                ErrorMessage = $"Booking cannot be cancelled when status is {booking.Status}."
            };
        }

        booking.Status = "Cancelled";
        booking.UpdatedAt = DateTime.UtcNow;

        await _bookingRepository.SaveChangesAsync();

        return new BookingServiceResult
        {
            IsSuccess = true,
            Response = MapToResponse(booking, "Booking cancelled successfully.")
        };
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
            CustomerName = booking.User?.FullName ?? string.Empty,
            CustomerPhone = booking.User?.Phone ?? string.Empty,
            ServicePlanDescription = booking.ServicePlan?.Description ?? string.Empty,
            CreatedAt = booking.CreatedAt,
            Message = message
        };
    }
}
