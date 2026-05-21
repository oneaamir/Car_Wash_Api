using CarWash.Backend.Models;

namespace CarWash.Backend.Repositories.Interfaces;

public interface IPaymentRepository
{
    Task AddAsync(Payment payment);

    Task<List<Payment>> GetAllAsync();

    Task<Payment?> GetByIdAsync(int id);

    Task<Payment?> GetByBookingIdAsync(int bookingId);

    Task<List<Payment>> GetByUserIdAsync(int userId);

    Task SaveChangesAsync();
}
