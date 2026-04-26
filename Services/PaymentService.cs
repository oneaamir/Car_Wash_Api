using CarWash.Backend.DTOs.Payment;
using CarWash.Backend.Models;
using CarWash.Backend.Repositories.Interfaces;
using CarWash.Backend.Services.Interfaces;

namespace CarWash.Backend.Services;

public class PaymentService : IPaymentService
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IPaymentRepository _paymentRepository;

    public PaymentService(IBookingRepository bookingRepository, IPaymentRepository paymentRepository)
    {
        _bookingRepository = bookingRepository;
        _paymentRepository = paymentRepository;
    }

    public async Task<PaymentServiceResult> CreatePaymentAsync(int userId, CreatePaymentRequest request)
    {
        var booking = await _bookingRepository.GetByIdAndUserIdAsync(request.BookingId, userId);

        if (booking == null)
        {
            return new PaymentServiceResult
            {
                IsNotFound = true,
                ErrorMessage = "Booking not found."
            };
        }

        var existingPayment = await _paymentRepository.GetByBookingIdAsync(request.BookingId);

        if (existingPayment != null)
        {
            return new PaymentServiceResult
            {
                ErrorMessage = "Payment already exists for this booking."
            };
        }

        var payment = new Payment
        {
            BookingId = request.BookingId,
            Amount = booking.TotalAmount,
            PaymentStatus = "Pending",
            TransactionRef = request.TransactionRef,
            PaymentMethod = request.PaymentMethod
        };

        await _paymentRepository.AddAsync(payment);
        await _paymentRepository.SaveChangesAsync();

        return new PaymentServiceResult
        {
            IsSuccess = true,
            Response = MapToResponse(payment, "Payment created successfully.")
        };
    }

    public async Task<PaymentServiceResult> GetPaymentByBookingIdAsync(int bookingId, int userId)
    {
        var booking = await _bookingRepository.GetByIdAndUserIdAsync(bookingId, userId);

        if (booking == null)
        {
            return new PaymentServiceResult
            {
                IsNotFound = true,
                ErrorMessage = "Booking not found."
            };
        }

        var payment = await _paymentRepository.GetByBookingIdAsync(bookingId);

        if (payment == null)
        {
            return new PaymentServiceResult
            {
                IsNotFound = true,
                ErrorMessage = "Payment not found."
            };
        }

        return new PaymentServiceResult
        {
            IsSuccess = true,
            Response = MapToResponse(payment, "Payment fetched successfully.")
        };
    }

    private static PaymentResponse MapToResponse(Payment payment, string message)
    {
        return new PaymentResponse
        {
            Id = payment.Id,
            BookingId = payment.BookingId,
            Amount = payment.Amount,
            PaymentStatus = payment.PaymentStatus,
            TransactionRef = payment.TransactionRef,
            PaymentMethod = payment.PaymentMethod,
            Message = message
        };
    }
}
