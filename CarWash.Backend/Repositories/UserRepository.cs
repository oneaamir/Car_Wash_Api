using CarWash.Backend.Data;
using CarWash.Backend.Models;
using CarWash.Backend.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CarWash.Backend.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(User user)
    {
        await _context.Users.AddAsync(user);
    }

    public async Task<List<User>> GetAllAsync()
    {
        return await _context.Users.ToListAsync();
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users
            .FirstOrDefaultAsync(user => user.Email == email);
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        return await _context.Users
            .FirstOrDefaultAsync(user => user.Id == id);
    }

    public async Task<User?> GetWasherByIdAsync(int id)
    {
        return await _context.Users
            .FirstOrDefaultAsync(user => user.Id == id && user.Role == "Washer" && user.IsActive);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
