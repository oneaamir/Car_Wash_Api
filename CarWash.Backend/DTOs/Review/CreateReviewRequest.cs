using System.ComponentModel.DataAnnotations;

namespace CarWash.Backend.DTOs.Review;

public class CreateReviewRequest
{
    [Required]
    public int BookingId { get; set; }

    [Range(1, 5)]
    public int Rating { get; set; }

    public string Comment { get; set; } = string.Empty;
}
