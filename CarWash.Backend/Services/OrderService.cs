using CarWash.Backend.DTOs.Order;
using CarWash.Backend.Models;
using CarWash.Backend.Repositories.Interfaces;
using CarWash.Backend.Services.Interfaces;

namespace CarWash.Backend.Services;

public class OrderService : IOrderService
{
    private readonly IBookingRepository _bookingRepository;

    public OrderService(IBookingRepository bookingRepository)
    {
        _bookingRepository = bookingRepository;
    }

    public async Task<List<OrderResponse>> GetMyOrdersAsync(int userId, GetMyOrdersRequest request)
    {
        var bookings = await _bookingRepository.GetByUserIdAsync(userId);

        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            bookings = bookings
                .Where(booking => booking.Status.Equals(request.Status.Trim(), StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        return bookings
            .OrderByDescending(booking => booking.BookingDate)
            .Select(booking => MapToResponse(booking, "Orders fetched successfully."))
            .ToList();
    }

    private static OrderResponse MapToResponse(Booking booking, string message)
    {
        var carDisplay = string.Join(
            " ",
            new[] { booking.Car?.Brand, booking.Car?.Model, booking.Car?.CarNumber }
                .Where(value => !string.IsNullOrWhiteSpace(value)));

        return new OrderResponse
        {
            BookingId = booking.Id,
            CarDisplay = carDisplay,
            ServicePlanName = booking.ServicePlan?.Name ?? string.Empty,
            AddOnNames = booking.BookingAddOns
                .Where(bookingAddOn => bookingAddOn.AddOn != null)
                .Select(bookingAddOn => bookingAddOn.AddOn!.Name)
                .ToList(),
            PromoCode = booking.PromoCode?.Code ?? string.Empty,
            BookingDate = booking.BookingDate,
            BookingType = booking.BookingType,
            Address = booking.Address,
            Status = booking.Status,
            TotalAmount = booking.TotalAmount,
            AssignedWasherName = booking.AssignedWasher?.FullName ?? string.Empty,
            Message = message
        };
    }
}
