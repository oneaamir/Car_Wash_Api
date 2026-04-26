using CarWash.Backend.DTOs.Report;

namespace CarWash.Backend.Services.Interfaces;

public interface IReportService
{
    Task<BookingReportResponse> GetBookingReportAsync(ReportFilterRequest request);

    Task<RevenueReportResponse> GetRevenueReportAsync(ReportFilterRequest request);
}
