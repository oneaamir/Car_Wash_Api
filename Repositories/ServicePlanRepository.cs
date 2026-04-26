using CarWash.Backend.Data;
using CarWash.Backend.Models;
using CarWash.Backend.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CarWash.Backend.Repositories;

public class ServicePlanRepository : IServicePlanRepository
{
    private readonly AppDbContext _context;

    public ServicePlanRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(ServicePlan servicePlan)
    {
        await _context.ServicePlans.AddAsync(servicePlan);
    }

    public async Task<ServicePlan?> GetActiveByIdAsync(int id)
    {
        return await _context.ServicePlans
            .FirstOrDefaultAsync(plan => plan.Id == id && plan.IsActive);
    }

    public async Task<List<ServicePlan>> GetAllActiveAsync()
    {
        return await _context.ServicePlans
            .Where(plan => plan.IsActive)
            .ToListAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
