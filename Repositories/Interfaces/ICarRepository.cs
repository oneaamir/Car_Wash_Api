using CarWash.Backend.Models;

namespace CarWash.Backend.Repositories.Interfaces;

public interface ICarRepository
{
    Task AddAsync(Car car);

    Task<Car?> GetActiveByIdAsync(int id, int userId);

    Task<List<Car>> GetActiveByUserIdAsync(int userId);

    Task SaveChangesAsync();
}
