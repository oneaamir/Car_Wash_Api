using CarWash.Backend.DTOs.Review;

namespace CarWash.Backend.Services;

public class ReviewServiceResult
{
    public bool IsSuccess { get; set; }

    public bool IsNotFound { get; set; }

    public string ErrorMessage { get; set; } = string.Empty;

    public ReviewResponse? Response { get; set; }
}
