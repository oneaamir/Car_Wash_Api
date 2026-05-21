using CarWash.Backend.DTOs.Report;
using CarWash.Backend.Repositories.Interfaces;
using CarWash.Backend.Services.Interfaces;

namespace CarWash.Backend.Services;

public class ReportService : IReportService
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IPaymentRepository _paymentRepository;

    public ReportService(IBookingRepository bookingRepository, IPaymentRepository paymentRepository)
    {
        _bookingRepository = bookingRepository;
        _paymentRepository = paymentRepository;
    }

    public async Task<BookingReportResponse> GetBookingReportAsync(ReportFilterRequest request)
    {
        var bookings = await _bookingRepository.GetAllAsync();
        var filteredBookings = bookings
            .Where(booking => IsWithinRange(booking.CreatedAt, request.DateFrom, request.DateTo))
            .ToList();

        return new BookingReportResponse
        {
            TotalBookings = filteredBookings.Count,
            PendingBookings = filteredBookings.Count(booking => booking.Status == "Pending"),
            ConfirmedBookings = filteredBookings.Count(booking => booking.Status == "Confirmed"),
            InProgressBookings = filteredBookings.Count(booking => booking.Status == "InProgress"),
            CompletedBookings = filteredBookings.Count(booking => booking.Status == "Completed"),
            CancelledBookings = filteredBookings.Count(booking => booking.Status == "Cancelled"),
            Message = "Booking report fetched successfully."
        };
    }

    public async Task<RevenueReportResponse> GetRevenueReportAsync(ReportFilterRequest request)
    {
        var payments = await _paymentRepository.GetAllAsync();

        // Attempts = payments submitted (created) in this period (regardless of final status)
        var submittedPayments = payments
            .Where(p => IsWithinRange(p.CreatedAt, request.DateFrom, request.DateTo))
            .ToList();

        // Revenue = payments confirmed (UpdatedAt) as Success in this period
        // A payment created last month but marked Success this month counts toward this month's revenue
        var successPayments = payments
            .Where(p => p.PaymentStatus.Equals("Success", StringComparison.OrdinalIgnoreCase)
                     && p.UpdatedAt.HasValue
                     && IsWithinRange(p.UpdatedAt.Value, request.DateFrom, request.DateTo))
            .ToList();

        // Failed = payments marked Failed in this period
        var failedInPeriod = payments
            .Where(p => p.PaymentStatus.Equals("Failed", StringComparison.OrdinalIgnoreCase)
                     && p.UpdatedAt.HasValue
                     && IsWithinRange(p.UpdatedAt.Value, request.DateFrom, request.DateTo))
            .Count();

        var totalPaymentAttempts = submittedPayments.Count;
        var pendingPayments    = submittedPayments.Count(p => p.PaymentStatus.Equals("Pending", StringComparison.OrdinalIgnoreCase));
        var successfulPayments = successPayments.Count;
        var totalRevenue       = successPayments.Sum(p => p.Amount);
        var averagePaymentAmount = successfulPayments == 0 ? 0 : Math.Round(totalRevenue / successfulPayments, 2);

        var revenueByMethod = successPayments
            .GroupBy(p => string.IsNullOrWhiteSpace(p.PaymentMethod) ? "Other" : p.PaymentMethod)
            .Select(g => new PaymentMethodSummary
            {
                Method = g.Key,
                Count  = g.Count(),
                Amount = g.Sum(p => p.Amount)
            })
            .OrderByDescending(s => s.Amount)
            .ToList();

        return new RevenueReportResponse
        {
            TotalPaymentAttempts = totalPaymentAttempts,
            PendingPayments      = pendingPayments,
            SuccessfulPayments   = successfulPayments,
            FailedPayments       = failedInPeriod,
            TotalRevenue         = totalRevenue,
            AveragePaymentAmount = averagePaymentAmount,
            RevenueByMethod      = revenueByMethod,
            Message = "Revenue report fetched successfully."
        };
    }

    private static bool IsWithinRange(DateTime value, DateTime? dateFrom, DateTime? dateTo)
    {
        var isAfterStart = !dateFrom.HasValue || value >= dateFrom.Value;
        var isBeforeEnd = !dateTo.HasValue || value <= dateTo.Value.AddDays(1).AddTicks(-1);

        return isAfterStart && isBeforeEnd;
    }
}
