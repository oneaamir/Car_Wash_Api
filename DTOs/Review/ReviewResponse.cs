namespace CarWash.Backend.DTOs.Review;

public class ReviewResponse
{
    public int Id { get; set; }

    public int BookingId { get; set; }

    public int UserId { get; set; }

    public int? WasherId { get; set; }

    public int Rating { get; set; }

    public string Comment { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public string Message { get; set; } = string.Empty;
}
