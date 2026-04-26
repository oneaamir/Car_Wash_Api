namespace CarWash.Backend.DTOs.PromoCode;

public class PromoValidationResponse
{
    public string Code { get; set; } = string.Empty;

    public bool IsValid { get; set; }

    public decimal OriginalAmount { get; set; }

    public decimal DiscountAmount { get; set; }

    public decimal FinalAmount { get; set; }

    public string Message { get; set; } = string.Empty;
}
