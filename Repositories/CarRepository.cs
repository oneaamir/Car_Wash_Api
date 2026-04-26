using CarWash.Backend.Data;
using CarWash.Backend.Models;
using CarWash.Backend.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CarWash.Backend.Repositories;

public class CarRepository : ICarRepository
{
    private readonly AppDbContext _context;

    public CarRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Car car)
    {
        await _context.Cars.AddAsync(car);
    }

    public async Task<Car?> GetActiveByIdAsync(int id, int userId)
    {
        return await _context.Cars
            .FirstOrDefaultAsync(car => car.Id == id && car.UserId == userId && car.IsActive);
    }

    public async Task<List<Car>> GetActiveByUserIdAsync(int userId)
    {
        return await _context.Cars
            .Where(car => car.UserId == userId && car.IsActive)
            .ToListAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
