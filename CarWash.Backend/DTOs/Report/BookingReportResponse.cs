namespace CarWash.Backend.DTOs.Report;

public class BookingReportResponse
{
    public int TotalBookings { get; set; }

    public int PendingBookings { get; set; }

    public int ConfirmedBookings { get; set; }

    public int CompletedBookings { get; set; }

    public int CancelledBookings { get; set; }

    public string Message { get; set; } = string.Empty;
}
