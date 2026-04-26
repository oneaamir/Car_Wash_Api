namespace CarWash.Backend.Models;

public class Car
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string CarNumber { get; set; } = string.Empty;

    public string Brand { get; set; } = string.Empty;

    public string Model { get; set; } = string.Empty;

    public string CarType { get; set; } = string.Empty;

    public string ImageUrl { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public User? User { get; set; }
}
