using CarWash.Backend.DTOs.ServicePlan;
using CarWash.Backend.Models;
using CarWash.Backend.Repositories.Interfaces;
using CarWash.Backend.Services.Interfaces;

namespace CarWash.Backend.Services;

public class ServicePlanService : IServicePlanService
{
    private readonly IServicePlanRepository _servicePlanRepository;

    public ServicePlanService(IServicePlanRepository servicePlanRepository)
    {
        _servicePlanRepository = servicePlanRepository;
    }

    public async Task<List<ServicePlanResponse>> GetAllPlansAsync()
    {
        var plans = await _servicePlanRepository.GetAllActiveAsync();

        return plans
            .Select(plan => MapToResponse(plan, "Service plans fetched successfully."))
            .ToList();
    }

    public async Task<ServicePlanResponse?> GetPlanByIdAsync(int id)
    {
        var plan = await _servicePlanRepository.GetActiveByIdAsync(id);

        return plan == null ? null : MapToResponse(plan, "Service plan fetched successfully.");
    }

    public async Task<ServicePlanResponse> CreatePlanAsync(CreateServicePlanRequest request)
    {
        var plan = new ServicePlan
        {
            Name = request.Name,
            Description = request.Description,
            Price = request.Price
        };

        await _servicePlanRepository.AddAsync(plan);
        await _servicePlanRepository.SaveChangesAsync();

        return MapToResponse(plan, "Service plan created successfully.");
    }

    public async Task<ServicePlanResponse?> UpdatePlanAsync(int id, UpdateServicePlanRequest request)
    {
        var plan = await _servicePlanRepository.GetActiveByIdAsync(id);

        if (plan == null)
        {
            return null;
        }

        plan.Name = request.Name;
        plan.Description = request.Description;
        plan.Price = request.Price;
        plan.UpdatedAt = DateTime.UtcNow;

        await _servicePlanRepository.SaveChangesAsync();

        return MapToResponse(plan, "Service plan updated successfully.");
    }

    public async Task<ServicePlanResponse?> DeletePlanAsync(int id)
    {
        var plan = await _servicePlanRepository.GetActiveByIdAsync(id);

        if (plan == null)
        {
            return null;
        }

        plan.IsActive = false;
        plan.UpdatedAt = DateTime.UtcNow;

        await _servicePlanRepository.SaveChangesAsync();

        return MapToResponse(plan, "Service plan deleted successfully.");
    }

    private static ServicePlanResponse MapToResponse(ServicePlan plan, string message)
    {
        return new ServicePlanResponse
        {
            Id = plan.Id,
            Name = plan.Name,
            Description = plan.Description,
            Price = plan.Price,
            IsActive = plan.IsActive,
            Message = message
        };
    }
}
