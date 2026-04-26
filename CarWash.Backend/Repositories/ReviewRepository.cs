using CarWash.Backend.Data;
using CarWash.Backend.Models;
using CarWash.Backend.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CarWash.Backend.Repositories;

public class ReviewRepository : IReviewRepository
{
    private readonly AppDbContext _context;

    public ReviewRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Review review)
    {
        await _context.Reviews.AddAsync(review);
    }

    public async Task<Review?> GetByBookingIdAndUserIdAsync(int bookingId, int userId)
    {
        return await _context.Reviews
            .FirstOrDefaultAsync(review => review.BookingId == bookingId && review.UserId == userId);
    }

    public async Task<List<Review>> GetByBookingIdAsync(int bookingId)
    {
        return await _context.Reviews
            .Where(review => review.BookingId == bookingId)
            .ToListAsync();
    }

    public async Task<List<Review>> GetAllAsync()
    {
        return await _context.Reviews.ToListAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
