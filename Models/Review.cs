namespace CarWash.Backend.Models;

public class Review
{
    public int Id { get; set; }

    public int BookingId { get; set; }

    public int UserId { get; set; }

    public int? WasherId { get; set; }

    public int Rating { get; set; }

    public string? Comment { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Booking? Booking { get; set; }

    public User? User { get; set; }
}
