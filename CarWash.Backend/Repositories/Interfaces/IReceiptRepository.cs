using CarWash.Backend.Models;

namespace CarWash.Backend.Repositories.Interfaces;

public interface IReceiptRepository
{
    Task AddAsync(Receipt receipt);

    Task<Receipt?> GetByBookingIdAsync(int bookingId);

    Task<List<Receipt>> GetByUserIdAsync(int userId);

    Task SaveChangesAsync();
}
