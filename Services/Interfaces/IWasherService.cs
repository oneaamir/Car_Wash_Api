using CarWash.Backend.DTOs.Booking;
using CarWash.Backend.DTOs.Washer;

namespace CarWash.Backend.Services.Interfaces;

public interface IWasherService
{
    Task<List<BookingResponse>> GetAssignedBookingsAsync(int washerId);

    Task<BookingResponse?> GetAssignedBookingByIdAsync(int id, int washerId);

    Task<WasherServiceResult> UpdateAssignedBookingStatusAsync(int id, int washerId, UpdateAssignedBookingStatusRequest request);
}
