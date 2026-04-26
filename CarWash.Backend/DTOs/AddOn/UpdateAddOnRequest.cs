using System.ComponentModel.DataAnnotations;

namespace CarWash.Backend.DTOs.AddOn;

public class UpdateAddOnRequest
{
    [Required]
    public string Name { get; set; } = string.Empty;

    [Range(0.0, double.MaxValue)]
    public decimal Price { get; set; }
}
