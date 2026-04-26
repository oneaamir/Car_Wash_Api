using System.ComponentModel.DataAnnotations;

namespace CarWash.Backend.DTOs.PromoCode;

public class CreatePromoCodeRequest
{
    [Required]
    public string Code { get; set; } = string.Empty;

    [Required]
    public string DiscountType { get; set; } = string.Empty;

    [Range(0.01, double.MaxValue)]
    public decimal DiscountValue { get; set; }

    public DateTime ExpiryDate { get; set; }
}
