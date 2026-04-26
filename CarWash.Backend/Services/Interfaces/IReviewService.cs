using CarWash.Backend.DTOs.Review;

namespace CarWash.Backend.Services.Interfaces;

public interface IReviewService
{
    Task<ReviewServiceResult> CreateReviewAsync(int userId, CreateReviewRequest request);

    Task<List<ReviewResponse>> GetReviewsByBookingIdAsync(int bookingId);

    Task<List<ReviewResponse>> GetAllReviewsAsync();
}
