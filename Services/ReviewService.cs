using CarWash.Backend.DTOs.Review;
using CarWash.Backend.Models;
using CarWash.Backend.Repositories.Interfaces;
using CarWash.Backend.Services.Interfaces;

namespace CarWash.Backend.Services;

public class ReviewService : IReviewService
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IReviewRepository _reviewRepository;

    public ReviewService(IBookingRepository bookingRepository, IReviewRepository reviewRepository)
    {
        _bookingRepository = bookingRepository;
        _reviewRepository = reviewRepository;
    }

    public async Task<ReviewServiceResult> CreateReviewAsync(int userId, CreateReviewRequest request)
    {
        var booking = await _bookingRepository.GetByIdAndUserIdAsync(request.BookingId, userId);

        if (booking == null)
        {
            return new ReviewServiceResult
            {
                IsNotFound = true,
                ErrorMessage = "Booking not found."
            };
        }

        if (!booking.Status.Equals("Completed", StringComparison.OrdinalIgnoreCase))
        {
            return new ReviewServiceResult
            {
                ErrorMessage = "Review can only be added after booking is completed."
            };
        }

        var existingReview = await _reviewRepository.GetByBookingIdAndUserIdAsync(request.BookingId, userId);

        if (existingReview != null)
        {
            return new ReviewServiceResult
            {
                ErrorMessage = "Review already exists for this booking."
            };
        }

        var review = new Review
        {
            BookingId = request.BookingId,
            UserId = userId,
            Rating = request.Rating,
            Comment = request.Comment
        };

        await _reviewRepository.AddAsync(review);
        await _reviewRepository.SaveChangesAsync();

        return new ReviewServiceResult
        {
            IsSuccess = true,
            Response = MapToResponse(review, "Review created successfully.")
        };
    }

    public async Task<List<ReviewResponse>> GetReviewsByBookingIdAsync(int bookingId)
    {
        var reviews = await _reviewRepository.GetByBookingIdAsync(bookingId);

        return reviews
            .Select(review => MapToResponse(review, "Reviews fetched successfully."))
            .ToList();
    }

    public async Task<List<ReviewResponse>> GetAllReviewsAsync()
    {
        var reviews = await _reviewRepository.GetAllAsync();

        return reviews
            .Select(review => MapToResponse(review, "Reviews fetched successfully."))
            .ToList();
    }

    private static ReviewResponse MapToResponse(Review review, string message)
    {
        return new ReviewResponse
        {
            Id = review.Id,
            BookingId = review.BookingId,
            UserId = review.UserId,
            WasherId = review.WasherId,
            Rating = review.Rating,
            Comment = review.Comment ?? string.Empty,
            CreatedAt = review.CreatedAt,
            Message = message
        };
    }
}
