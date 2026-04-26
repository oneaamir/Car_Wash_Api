using CarWash.Backend.Models;

namespace CarWash.Backend.Repositories.Interfaces;

public interface IAddOnRepository
{
    Task AddAsync(AddOn addOn);

    Task<AddOn?> GetActiveByIdAsync(int id);

    Task<List<AddOn>> GetActiveByIdsAsync(List<int> ids);

    Task<List<AddOn>> GetAllActiveAsync();

    Task SaveChangesAsync();
}
