using CarWash.Backend.DTOs.ServicePlan;

namespace CarWash.Backend.Services.Interfaces;

public interface IServicePlanService
{
    Task<List<ServicePlanResponse>> GetAllPlansAsync();

    Task<ServicePlanResponse?> GetPlanByIdAsync(int id);

    Task<ServicePlanResponse> CreatePlanAsync(CreateServicePlanRequest request);

    Task<ServicePlanResponse?> UpdatePlanAsync(int id, UpdateServicePlanRequest request);

    Task<ServicePlanResponse?> DeletePlanAsync(int id);
}
