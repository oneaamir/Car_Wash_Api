namespace CarWash.Backend.DTOs.ServicePlan;

public class ServicePlanResponse
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public bool IsActive { get; set; }

    public string Message { get; set; } = string.Empty;
}
