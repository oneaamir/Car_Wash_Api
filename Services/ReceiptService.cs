using CarWash.Backend.DTOs.Receipt;
using CarWash.Backend.Models;
using CarWash.Backend.Repositories.Interfaces;
using CarWash.Backend.Services.Interfaces;

namespace CarWash.Backend.Services;

public class ReceiptService : IReceiptService
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IPaymentRepository _paymentRepository;
    private readonly IReceiptRepository _receiptRepository;
    private readonly IEmailService _emailService;

    public ReceiptService(
        IBookingRepository bookingRepository,
        IPaymentRepository paymentRepository,
        IReceiptRepository receiptRepository,
        IEmailService emailService)
    {
        _bookingRepository = bookingRepository;
        _paymentRepository = paymentRepository;
        _receiptRepository = receiptRepository;
        _emailService = emailService;
    }

    public async Task<ReceiptServiceResult> GenerateReceiptAsync(int bookingId, int userId)
    {
        var booking = await _bookingRepository.GetByIdAndUserIdAsync(bookingId, userId);

        if (booking == null)
        {
            return new ReceiptServiceResult
            {
                IsNotFound = true,
                ErrorMessage = "Booking not found."
            };
        }

        var payment = await _paymentRepository.GetByBookingIdAsync(bookingId);

        if (payment == null)
        {
            return new ReceiptServiceResult
            {
                IsNotFound = true,
                ErrorMessage = "Payment not found."
            };
        }

        if (!payment.PaymentStatus.Equals("Success", StringComparison.OrdinalIgnoreCase))
        {
            return new ReceiptServiceResult
            {
                ErrorMessage = "Receipt can only be generated after successful payment."
            };
        }

        var existingReceipt = await _receiptRepository.GetByBookingIdAsync(bookingId);

        if (existingReceipt != null)
        {
            return new ReceiptServiceResult
            {
                ErrorMessage = "Receipt already exists for this booking."
            };
        }

        var receipt = new Receipt
        {
            BookingId = bookingId,
            PaymentId = payment.Id,
            ReceiptNumber = $"RCPT-{DateTime.UtcNow:yyyyMMddHHmmss}-{bookingId}",
            AfterWashImageUrl = string.Empty
        };

        await _receiptRepository.AddAsync(receipt);
        await _receiptRepository.SaveChangesAsync();

        if (booking.User != null && !string.IsNullOrWhiteSpace(booking.User.Email))
        {
            await _emailService.SendEmailAsync(
                booking.User.Email,
                "Receipt Generated",
                $"<p>Hello {booking.User.FullName},</p><p>Your receipt <strong>{receipt.ReceiptNumber}</strong> has been generated for booking <strong>#{booking.Id}</strong>.</p>",
                isHtml: true);
        }

        return new ReceiptServiceResult
        {
            IsSuccess = true,
            Response = MapToResponse(receipt, "Receipt generated successfully.")
        };
    }

    public async Task<ReceiptServiceResult> GetReceiptByBookingIdAsync(int bookingId, int userId)
    {
        var booking = await _bookingRepository.GetByIdAndUserIdAsync(bookingId, userId);

        if (booking == null)
        {
            return new ReceiptServiceResult
            {
                IsNotFound = true,
                ErrorMessage = "Booking not found."
            };
        }

        var receipt = await _receiptRepository.GetByBookingIdAsync(bookingId);

        if (receipt == null)
        {
            return new ReceiptServiceResult
            {
                IsNotFound = true,
                ErrorMessage = "Receipt not found."
            };
        }

        return new ReceiptServiceResult
        {
            IsSuccess = true,
            Response = MapToResponse(receipt, "Receipt fetched successfully.")
        };
    }

    private static ReceiptResponse MapToResponse(Receipt receipt, string message)
    {
        return new ReceiptResponse
        {
            Id = receipt.Id,
            BookingId = receipt.BookingId,
            PaymentId = receipt.PaymentId,
            ReceiptNumber = receipt.ReceiptNumber,
            GeneratedAt = receipt.GeneratedAt,
            AfterWashImageUrl = receipt.AfterWashImageUrl,
            Message = message
        };
    }
}
