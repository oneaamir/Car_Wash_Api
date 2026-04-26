using CarWash.Backend.Data;
using CarWash.Backend.Models;
using CarWash.Backend.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CarWash.Backend.Repositories;

public class AddOnRepository : IAddOnRepository
{
    private readonly AppDbContext _context;

    public AddOnRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(AddOn addOn)
    {
        await _context.AddOns.AddAsync(addOn);
    }

    public async Task<AddOn?> GetActiveByIdAsync(int id)
    {
        return await _context.AddOns
            .FirstOrDefaultAsync(addOn => addOn.Id == id && addOn.IsActive);
    }

    public async Task<List<AddOn>> GetActiveByIdsAsync(List<int> ids)
    {
        return await _context.AddOns
            .Where(addOn => ids.Contains(addOn.Id) && addOn.IsActive)
            .ToListAsync();
    }

    public async Task<List<AddOn>> GetAllActiveAsync()
    {
        return await _context.AddOns
            .Where(addOn => addOn.IsActive)
            .ToListAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
