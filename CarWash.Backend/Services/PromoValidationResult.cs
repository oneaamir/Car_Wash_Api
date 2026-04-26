using CarWash.Backend.DTOs.PromoCode;

namespace CarWash.Backend.Services;

public class PromoValidationResult
{
    public bool IsSuccess { get; set; }

    public bool IsNotFound { get; set; }

    public string ErrorMessage { get; set; } = string.Empty;

    public PromoValidationResponse? Response { get; set; }
}
