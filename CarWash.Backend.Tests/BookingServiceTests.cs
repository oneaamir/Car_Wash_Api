using CarWash.Backend.Models;
using CarWash.Backend.Services;

namespace CarWash.Backend.Tests;

public class BookingServiceTests
{
    [Test]
    public async Task CancelBookingAsync_WhenStatusIsPending_CancelsBookingSuccessfully()
    {
        var bookingRepository = new FakeBookingRepository
        {
            BookingByIdAndUser = new Booking
            {
                Id = 11,
                UserId = 10,
                Status = "Pending"
            }
        };

        var service = new BookingService(
            bookingRepository,
            new FakeCarRepository(),
            new FakeServicePlanRepository(),
            new FakePromoCodeRepository(),
            new FakeAddOnRepository());

        var result = await service.CancelBookingAsync(11, 10);

        ClassicAssert.IsTrue(result.IsSuccess);
        ClassicAssert.IsNotNull(result.Response);
        ClassicAssert.AreEqual("Cancelled", result.Response!.Status);
        ClassicAssert.AreEqual("Booking cancelled successfully.", result.Response.Message);
        ClassicAssert.AreEqual("Cancelled", bookingRepository.BookingByIdAndUser!.Status);
        ClassicAssert.IsTrue(bookingRepository.SaveChangesCalled);
    }

    [Test]
    public async Task CancelBookingAsync_WhenStatusIsCompleted_ReturnsBusinessError()
    {
        var bookingRepository = new FakeBookingRepository
        {
            BookingByIdAndUser = new Booking
            {
                Id = 1,
                UserId = 10,
                Status = "Completed"
            }
        };

        var service = new BookingService(
            bookingRepository,
            new FakeCarRepository(),
            new FakeServicePlanRepository(),
            new FakePromoCodeRepository(),
            new FakeAddOnRepository());

        var result = await service.CancelBookingAsync(1, 10);

        ClassicAssert.IsFalse(result.IsSuccess);
        ClassicAssert.AreEqual("Booking cannot be cancelled when status is Completed.", result.ErrorMessage);
        ClassicAssert.IsFalse(bookingRepository.SaveChangesCalled);
    }
}
