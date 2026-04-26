using CarWash.Backend.DTOs.Review;
using CarWash.Backend.Models;
using CarWash.Backend.Services;

namespace CarWash.Backend.Tests;

public class ReviewServiceTests
{
    [Test]
    public async Task CreateReviewAsync_WhenBookingNotCompleted_ReturnsBusinessError()
    {
        var bookingRepository = new FakeBookingRepository
        {
            BookingByIdAndUser = new Booking
            {
                Id = 2,
                UserId = 20,
                Status = "Confirmed"
            }
        };

        var reviewRepository = new FakeReviewRepository();
        var service = new ReviewService(bookingRepository, reviewRepository);

        var result = await service.CreateReviewAsync(20, new CreateReviewRequest
        {
            BookingId = 2,
            Rating = 5,
            Comment = "Great service"
        });

        ClassicAssert.IsFalse(result.IsSuccess);
        ClassicAssert.AreEqual("Review can only be added after booking is completed.", result.ErrorMessage);
        ClassicAssert.IsFalse(reviewRepository.SaveChangesCalled);
    }
}
