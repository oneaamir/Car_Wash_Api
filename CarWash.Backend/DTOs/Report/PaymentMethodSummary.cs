namespace CarWash.Backend.DTOs.Report;

public class PaymentMethodSummary
{
    public string Method { get; set; } = string.Empty;

    public int Count { get; set; }

    public decimal Amount { get; set; }
}
