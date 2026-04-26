using CarWash.Backend.Models;

namespace CarWash.Backend.Repositories.Interfaces;

public interface IBookingRepository
{
    Task AddAsync(Booking booking);

    Task<List<Booking>> GetAllAsync();

    Task<Booking?> GetByIdAsync(int id);

    Task<Booking?> GetByIdAndUserIdAsync(int id, int userId);

    Task<Booking?> GetByIdAndAssignedWasherIdAsync(int id, int washerId);

    Task<List<Booking>> GetByUserIdAsync(int userId);

    Task<List<Booking>> GetByAssignedWasherIdAsync(int washerId);

    Task SaveChangesAsync();
}
