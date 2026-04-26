using CarWash.Backend.Models;

namespace CarWash.Backend.Repositories.Interfaces;

public interface IReviewRepository
{
    Task AddAsync(Review review);

    Task<Review?> GetByBookingIdAndUserIdAsync(int bookingId, int userId);

    Task<List<Review>> GetByBookingIdAsync(int bookingId);

    Task<List<Review>> GetAllAsync();

    Task SaveChangesAsync();
}
