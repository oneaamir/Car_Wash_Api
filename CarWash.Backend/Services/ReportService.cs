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
        var filteredPayments = payments
            .Where(payment => IsWithinRange(payment.CreatedAt, request.DateFrom, request.DateTo))
            .ToList();

        var successPayments = filteredPayments
            .Where(payment => payment.PaymentStatus.Equals("Success", StringComparison.OrdinalIgnoreCase))
            .ToList();

        var totalPaymentAttempts = filteredPayments.Count;
        var pendingPayments = filteredPayments.Count(p => p.PaymentStatus.Equals("Pending", StringComparison.OrdinalIgnoreCase));
        var failedPayments  = filteredPayments.Count(p => p.PaymentStatus.Equals("Failed",  StringComparison.OrdinalIgnoreCase));
        var successfulPayments = successPayments.Count;
        var totalRevenue = successPayments.Sum(p => p.Amount);
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
            FailedPayments       = failedPayments,
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
