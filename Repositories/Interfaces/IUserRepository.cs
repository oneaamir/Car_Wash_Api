using CarWash.Backend.Models;

namespace CarWash.Backend.Repositories.Interfaces;

public interface IUserRepository
{
    Task AddAsync(User user);

    Task<List<User>> GetAllAsync();

    Task<User?> GetByEmailAsync(string email);

    Task<User?> GetByIdAsync(int id);

    Task<User?> GetWasherByIdAsync(int id);

    Task SaveChangesAsync();
}
