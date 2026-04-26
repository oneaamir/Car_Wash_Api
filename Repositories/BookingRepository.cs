using CarWash.Backend.Data;
using CarWash.Backend.Models;
using CarWash.Backend.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CarWash.Backend.Repositories;

public class BookingRepository : IBookingRepository
{
    private readonly AppDbContext _context;

    public BookingRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Booking booking)
    {
        await _context.Bookings.AddAsync(booking);
    }

    public async Task<List<Booking>> GetAllAsync()
    {
        return await _context.Bookings
            .Include(booking => booking.User)
            .Include(booking => booking.AssignedWasher)
            .Include(booking => booking.Car)
            .Include(booking => booking.ServicePlan)
            .Include(booking => booking.PromoCode)
            .Include(booking => booking.BookingAddOns)
                .ThenInclude(bookingAddOn => bookingAddOn.AddOn)
            .ToListAsync();
    }

    public async Task<Booking?> GetByIdAsync(int id)
    {
        return await _context.Bookings
            .Include(booking => booking.User)
            .Include(booking => booking.AssignedWasher)
            .Include(booking => booking.Car)
            .Include(booking => booking.ServicePlan)
            .Include(booking => booking.PromoCode)
            .Include(booking => booking.BookingAddOns)
                .ThenInclude(bookingAddOn => bookingAddOn.AddOn)
            .FirstOrDefaultAsync(booking => booking.Id == id);
    }

    public async Task<Booking?> GetByIdAndUserIdAsync(int id, int userId)
    {
        return await _context.Bookings
            .Include(booking => booking.User)
            .Include(booking => booking.AssignedWasher)
            .Include(booking => booking.Car)
            .Include(booking => booking.ServicePlan)
            .Include(booking => booking.PromoCode)
            .Include(booking => booking.BookingAddOns)
                .ThenInclude(bookingAddOn => bookingAddOn.AddOn)
            .FirstOrDefaultAsync(booking => booking.Id == id && booking.UserId == userId);
    }

    public async Task<Booking?> GetByIdAndAssignedWasherIdAsync(int id, int washerId)
    {
        return await _context.Bookings
            .Include(booking => booking.User)
            .Include(booking => booking.AssignedWasher)
            .Include(booking => booking.Car)
            .Include(booking => booking.ServicePlan)
            .Include(booking => booking.PromoCode)
            .Include(booking => booking.BookingAddOns)
                .ThenInclude(bookingAddOn => bookingAddOn.AddOn)
            .FirstOrDefaultAsync(booking => booking.Id == id && booking.AssignedWasherId == washerId);
    }

    public async Task<List<Booking>> GetByUserIdAsync(int userId)
    {
        return await _context.Bookings
            .Include(booking => booking.User)
            .Include(booking => booking.AssignedWasher)
            .Include(booking => booking.Car)
            .Include(booking => booking.ServicePlan)
            .Include(booking => booking.PromoCode)
            .Include(booking => booking.BookingAddOns)
                .ThenInclude(bookingAddOn => bookingAddOn.AddOn)
            .Where(booking => booking.UserId == userId)
            .ToListAsync();
    }

    public async Task<List<Booking>> GetByAssignedWasherIdAsync(int washerId)
    {
        return await _context.Bookings
            .Include(booking => booking.User)
            .Include(booking => booking.AssignedWasher)
            .Include(booking => booking.Car)
            .Include(booking => booking.ServicePlan)
            .Include(booking => booking.PromoCode)
            .Include(booking => booking.BookingAddOns)
                .ThenInclude(bookingAddOn => bookingAddOn.AddOn)
            .Where(booking => booking.AssignedWasherId == washerId)
            .ToListAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
