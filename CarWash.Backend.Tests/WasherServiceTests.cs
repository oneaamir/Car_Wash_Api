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

        ClassicAssert.IsFalse(result.IsSuccess);
        ClassicAssert.IsTrue(result.IsNotFound);
        ClassicAssert.AreEqual("Assigned booking not found.", result.ErrorMessage);
        ClassicAssert.IsFalse(bookingRepository.SaveChangesCalled);
        ClassicAssert.IsEmpty(emailService.SentEmails);
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

        ClassicAssert.IsFalse(result.IsSuccess);
        ClassicAssert.AreEqual("Washer cannot change booking status from Pending to Completed.", result.ErrorMessage);
        ClassicAssert.IsFalse(bookingRepository.SaveChangesCalled);
        ClassicAssert.IsEmpty(emailService.SentEmails);
    }
}
