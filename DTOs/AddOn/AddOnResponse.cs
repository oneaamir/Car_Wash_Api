namespace CarWash.Backend.DTOs.AddOn;

public class AddOnResponse
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public bool IsActive { get; set; }

    public string Message { get; set; } = string.Empty;
}
