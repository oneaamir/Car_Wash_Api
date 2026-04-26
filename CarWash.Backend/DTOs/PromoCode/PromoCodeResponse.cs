namespace CarWash.Backend.DTOs.PromoCode;

public class PromoCodeResponse
{
    public int Id { get; set; }

    public string Code { get; set; } = string.Empty;

    public string DiscountType { get; set; } = string.Empty;

    public decimal DiscountValue { get; set; }

    public DateTime ExpiryDate { get; set; }

    public bool IsActive { get; set; }

    public string Message { get; set; } = string.Empty;
}
