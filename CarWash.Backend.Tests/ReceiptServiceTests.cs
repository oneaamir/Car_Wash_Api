using CarWash.Backend.Models;
using CarWash.Backend.Services;

namespace CarWash.Backend.Tests;

public class ReceiptServiceTests
{
    [Test]
    public async Task GenerateReceiptAsync_WhenPaymentNotSuccessful_ReturnsBusinessError()
    {
        var bookingRepository = new FakeBookingRepository
        {
            BookingByIdAndUser = new Booking
            {
                Id = 3,
                UserId = 30,
                Status = "Completed"
            }
        };

        var paymentRepository = new FakePaymentRepository
        {
            PaymentByBookingId = new Payment
            {
                Id = 50,
                BookingId = 3,
                PaymentStatus = "Pending"
            }
        };

        var receiptRepository = new FakeReceiptRepository();
        var emailService = new FakeEmailService();
        var service = new ReceiptService(bookingRepository, paymentRepository, receiptRepository, emailService);

        var result = await service.GenerateReceiptAsync(3, 30);

        ClassicAssert.IsFalse(result.IsSuccess);
        ClassicAssert.AreEqual("Receipt can only be generated after successful payment.", result.ErrorMessage);
        ClassicAssert.IsFalse(receiptRepository.SaveChangesCalled);
        ClassicAssert.IsEmpty(emailService.SentEmails);
    }
}
