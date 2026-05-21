using CarWash.Backend.DTOs.Admin;
using CarWash.Backend.DTOs.Payment;

namespace CarWash.Backend.Services.Interfaces;

public interface IAdminService
{
    Task<List<UserSummaryResponse>> GetUsersAsync();

    Task<List<AdminBookingResponse>> GetBookingsAsync();

    Task<List<PaymentResponse>> GetAllPaymentsAsync();

    Task<AdminServiceResult<AdminBookingResponse>> AssignWasherAsync(int id, AssignWasherRequest request);

    Task<AdminServiceResult<AdminBookingResponse>> UpdateBookingStatusAsync(int id, UpdateBookingStatusRequest request);

    Task<AdminServiceResult<PaymentResponse>> UpdatePaymentStatusAsync(int id, UpdatePaymentStatusRequest request);
}
