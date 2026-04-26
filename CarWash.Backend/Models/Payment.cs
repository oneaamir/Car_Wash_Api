namespace CarWash.Backend.Models;

public class Payment
{
    public int Id { get; set; }

    public int BookingId { get; set; }

    public decimal Amount { get; set; }

    public string PaymentStatus { get; set; } = "Pending";

    public string TransactionRef { get; set; } = string.Empty;

    public string PaymentMethod { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public Booking? Booking { get; set; }
}
