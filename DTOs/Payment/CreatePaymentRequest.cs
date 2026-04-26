using System.ComponentModel.DataAnnotations;

namespace CarWash.Backend.DTOs.Payment;

public class CreatePaymentRequest
{
    [Required]
    public int BookingId { get; set; }

    [Required]
    public string PaymentMethod { get; set; } = string.Empty;

    public string TransactionRef { get; set; } = string.Empty;
}
