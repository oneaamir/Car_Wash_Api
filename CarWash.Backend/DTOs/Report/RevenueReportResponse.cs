namespace CarWash.Backend.DTOs.Report;

public class RevenueReportResponse
{
    public int TotalPaymentAttempts { get; set; }

    public int PendingPayments { get; set; }

    public int SuccessfulPayments { get; set; }

    public int FailedPayments { get; set; }

    public decimal TotalRevenue { get; set; }

    public decimal AveragePaymentAmount { get; set; }

    public List<PaymentMethodSummary> RevenueByMethod { get; set; } = new();

    public string Message { get; set; } = string.Empty;
}
