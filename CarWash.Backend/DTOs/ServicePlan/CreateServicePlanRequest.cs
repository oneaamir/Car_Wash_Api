using System.ComponentModel.DataAnnotations;

namespace CarWash.Backend.DTOs.ServicePlan;

public class CreateServicePlanRequest
{
    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;
//price zero ya negative nahi ho sakta
    [Range(0.01, double.MaxValue)]
    public decimal Price { get; set; }
}
