namespace CarWash.Backend.DTOs.Car;

public class CarResponse
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string CarNumber { get; set; } = string.Empty;

    public string Brand { get; set; } = string.Empty;

    public string Model { get; set; } = string.Empty;

    public string CarType { get; set; } = string.Empty;

    public string ImageUrl { get; set; } = string.Empty;

    public bool IsActive { get; set; }

    public string Message { get; set; } = string.Empty;
}
