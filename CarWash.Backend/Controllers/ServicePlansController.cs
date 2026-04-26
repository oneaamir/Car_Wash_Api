using CarWash.Backend.DTOs.ServicePlan;
using CarWash.Backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarWash.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class ServicePlansController : ControllerBase
{
    private readonly IServicePlanService _servicePlanService;

    public ServicePlansController(IServicePlanService servicePlanService)
    {
        _servicePlanService = servicePlanService;
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<List<ServicePlanResponse>>> GetAllPlans()
    {
        var plans = await _servicePlanService.GetAllPlansAsync();

        return Ok(plans);
    }

    [AllowAnonymous]
    [HttpGet("{id}")]
    public async Task<ActionResult<ServicePlanResponse>> GetPlanById(int id)
    {
        var response = await _servicePlanService.GetPlanByIdAsync(id);

        if (response == null)
        {
            return NotFound("Service plan not found.");
        }

        return Ok(response);
    }

    [HttpPost]
    public async Task<ActionResult<ServicePlanResponse>> CreatePlan(CreateServicePlanRequest request)
    {
        var response = await _servicePlanService.CreatePlanAsync(request);

        return Ok(response);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ServicePlanResponse>> UpdatePlan(int id, UpdateServicePlanRequest request)
    {
        var response = await _servicePlanService.UpdatePlanAsync(id, request);

        if (response == null)
        {
            return NotFound("Service plan not found.");
        }

        return Ok(response);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ServicePlanResponse>> DeletePlan(int id)
    {
        var response = await _servicePlanService.DeletePlanAsync(id);

        if (response == null)
        {
            return NotFound("Service plan not found.");
        }

        return Ok(response);
    }
}
