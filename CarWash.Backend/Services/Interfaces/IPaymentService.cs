using CarWash.Backend.DTOs.Payment;

namespace CarWash.Backend.Services.Interfaces;

public interface IPaymentService
{
    Task<PaymentServiceResult> CreatePaymentAsync(int userId, CreatePaymentRequest request);

    Task<PaymentServiceResult> GetPaymentByBookingIdAsync(int bookingId, int userId);

    Task<List<PaymentResponse>> GetMyPaymentsAsync(int userId);
}
