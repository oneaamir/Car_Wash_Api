using System.ComponentModel.DataAnnotations;

namespace CarWash.Backend.DTOs.Admin;

public class UpdatePaymentStatusRequest
{
    [Required]
    public string PaymentStatus { get; set; } = string.Empty;
}
