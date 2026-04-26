using CarWash.Backend.Models;

namespace CarWash.Backend.Repositories.Interfaces;

public interface IServicePlanRepository
{
    Task AddAsync(ServicePlan servicePlan);

    Task<ServicePlan?> GetActiveByIdAsync(int id);

    Task<List<ServicePlan>> GetAllActiveAsync();

    Task SaveChangesAsync();
}
