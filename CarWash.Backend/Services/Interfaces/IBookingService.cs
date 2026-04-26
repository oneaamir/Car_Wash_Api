using CarWash.Backend.DTOs.Booking;

namespace CarWash.Backend.Services.Interfaces;

public interface IBookingService
{
    Task<BookingServiceResult> CreateBookingAsync(int userId, CreateBookingRequest request);

    Task<List<BookingResponse>> GetMyBookingsAsync(int userId);

    Task<BookingResponse?> GetBookingByIdAsync(int id, int userId);

    Task<BookingServiceResult> CancelBookingAsync(int id, int userId);
}
