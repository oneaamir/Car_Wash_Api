using System.ComponentModel.DataAnnotations;

namespace CarWash.Backend.DTOs.PromoCode;

public class ValidatePromoCodeRequest
{
    [Required]
    public string Code { get; set; } = string.Empty;

    [Range(0.01, double.MaxValue)]
    public decimal OrderAmount { get; set; }
}
