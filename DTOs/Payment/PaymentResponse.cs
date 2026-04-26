namespace CarWash.Backend.DTOs.Payment;

public class PaymentResponse
{
    public int Id { get; set; }

    public int BookingId { get; set; }

    public decimal Amount { get; set; }

    public string PaymentStatus { get; set; } = string.Empty;

    public string TransactionRef { get; set; } = string.Empty;

    public string PaymentMethod { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;
}
