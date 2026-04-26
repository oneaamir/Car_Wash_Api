using CarWash.Backend.Models;

namespace CarWash.Backend.Repositories.Interfaces;

public interface IPromoCodeRepository
{
    Task AddAsync(PromoCode promoCode);

    Task<PromoCode?> GetActiveByIdAsync(int id);

    Task<PromoCode?> GetActiveByCodeAsync(string code);

    Task<List<PromoCode>> GetAllActiveAsync();

    Task SaveChangesAsync();
}
