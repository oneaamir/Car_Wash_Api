using System.ComponentModel.DataAnnotations;

namespace CarWash.Backend.DTOs.Car;

public class CreateCarRequest
{
    [Required]
    public string CarNumber { get; set; } = string.Empty;

    [Required]
    public string Brand { get; set; } = string.Empty;

    [Required]
    public string Model { get; set; } = string.Empty;

    [Required]
    public string CarType { get; set; } = string.Empty;

    public string ImageUrl { get; set; } = string.Empty;
}
