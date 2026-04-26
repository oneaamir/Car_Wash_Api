namespace CarWash.Backend.DTOs.Receipt;

public class ReceiptResponse
{
    public int Id { get; set; }

    public int BookingId { get; set; }

    public int PaymentId { get; set; }

    public string ReceiptNumber { get; set; } = string.Empty;

    public DateTime GeneratedAt { get; set; }

    public string AfterWashImageUrl { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;
}
