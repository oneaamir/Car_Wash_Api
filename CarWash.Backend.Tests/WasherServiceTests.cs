using CarWash.Backend.DTOs.Washer;
using CarWash.Backend.Models;
using CarWash.Backend.Services;

namespace CarWash.Backend.Tests;

public class WasherServiceTests
{
    [Test]
    public async Task UpdateAssignedBookingStatusAsync_WhenBookingIsNotAssignedToWasher_ReturnsNotFound()
    {
        var bookingRepository = new FakeBookingRepository
        {
            BookingByIdAndAssignedWasher = null
        };

        var emailService = new FakeEmailService();
        var service = new WasherService(bookingRepository, emailService);

        var result = await service.UpdateAssignedBookingStatusAsync(5, 99, new UpdateAssignedBookingStatusRequest
        {
            Status = "InProgress"
        });

        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.IsNotFound, Is.True);
        Assert.That(result.ErrorMessage, Is.EqualTo("Assigned booking not found."));
        Assert.That(bookingRepository.SaveChangesCalled, Is.False);
        Assert.That(emailService.SentEmails, Is.Empty);
    }

    [Test]
    public async Task UpdateAssignedBookingStatusAsync_WhenTransitionIsInvalid_ReturnsBusinessError()
    {
        var bookingRepository = new FakeBookingRepository
        {
            BookingByIdAndAssignedWasher = new Booking
            {
                Id = 4,
                UserId = 40,
                AssignedWasherId = 7,
                Status = "Pending"
            }
        };

        var emailService = new FakeEmailService();
        var service = new WasherService(bookingRepository, emailService);

        var result = await service.UpdateAssignedBookingStatusAsync(4, 7, new UpdateAssignedBookingStatusRequest
        {
            Status = "Completed"
        });

        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.ErrorMessage, Is.EqualTo("Washer cannot change booking status from Pending to Completed."));
        Assert.That(bookingRepository.SaveChangesCalled, Is.False);
        Assert.That(emailService.SentEmails, Is.Empty);
    }
}
