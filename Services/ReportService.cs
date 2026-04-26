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
        var successPayments = payments
            .Where(payment => IsWithinRange(payment.CreatedAt, request.DateFrom, request.DateTo))
            .Where(payment => payment.PaymentStatus == "Success")
            .ToList();
        var totalPaymentAttempts = filteredPayments.Count;
        var failedPayments = filteredPayments.Count(payment => payment.PaymentStatus == "Failed");

        var successfulPayments = successPayments.Count;
        var totalRevenue = successPayments.Sum(payment => payment.Amount);
        var averagePaymentAmount = successfulPayments == 0 ? 0 : totalRevenue / successfulPayments;

        return new RevenueReportResponse
        {
            TotalPaymentAttempts = totalPaymentAttempts,
            SuccessfulPayments = successfulPayments,
            FailedPayments = failedPayments,
            TotalRevenue = totalRevenue,
            AveragePaymentAmount = averagePaymentAmount,
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
