namespace CarWash.Backend.Models;

public class Receipt
{
    public int Id { get; set; }

    public int BookingId { get; set; }

    public int PaymentId { get; set; }

    public string ReceiptNumber { get; set; } = string.Empty;

    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;

    public string AfterWashImageUrl { get; set; } = string.Empty;

    public Booking? Booking { get; set; }

    public Payment? Payment { get; set; }
}
