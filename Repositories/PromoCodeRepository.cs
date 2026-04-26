using CarWash.Backend.Data;
using CarWash.Backend.Models;
using CarWash.Backend.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CarWash.Backend.Repositories;

public class PromoCodeRepository : IPromoCodeRepository
{
    private readonly AppDbContext _context;

    public PromoCodeRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(PromoCode promoCode)
    {
        await _context.PromoCodes.AddAsync(promoCode);
    }

    public async Task<PromoCode?> GetActiveByIdAsync(int id)
    {
        return await _context.PromoCodes
            .FirstOrDefaultAsync(promo => promo.Id == id && promo.IsActive);
    }

    public async Task<PromoCode?> GetActiveByCodeAsync(string code)
    {
        return await _context.PromoCodes
            .FirstOrDefaultAsync(promo => promo.Code == code && promo.IsActive);
    }

    public async Task<List<PromoCode>> GetAllActiveAsync()
    {
        return await _context.PromoCodes
            .Where(promo => promo.IsActive)
            .ToListAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
