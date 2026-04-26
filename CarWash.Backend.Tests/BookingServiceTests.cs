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

        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Response, Is.Not.Null);
        Assert.That(result.Response!.Status, Is.EqualTo("Cancelled"));
        Assert.That(result.Response.Message, Is.EqualTo("Booking cancelled successfully."));
        Assert.That(bookingRepository.BookingByIdAndUser!.Status, Is.EqualTo("Cancelled"));
        Assert.That(bookingRepository.SaveChangesCalled, Is.True);
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

        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.ErrorMessage, Is.EqualTo("Booking cannot be cancelled when status is Completed."));
        Assert.That(bookingRepository.SaveChangesCalled, Is.False);
    }
}
