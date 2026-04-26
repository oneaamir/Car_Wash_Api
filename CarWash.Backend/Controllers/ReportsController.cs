using CarWash.Backend.DTOs.Report;
using CarWash.Backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarWash.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class ReportsController : ControllerBase
{
    private readonly IReportService _reportService;

    public ReportsController(IReportService reportService)
    {
        _reportService = reportService;
    }

    [HttpGet("bookings")]
    public async Task<ActionResult<BookingReportResponse>> GetBookingReport([FromQuery] ReportFilterRequest request)
    {
        if (request.DateFrom.HasValue && request.DateTo.HasValue && request.DateFrom > request.DateTo)
        {
            return BadRequest("DateFrom cannot be greater than DateTo.");
        }

        var response = await _reportService.GetBookingReportAsync(request);

        return Ok(response);
    }

    [HttpGet("revenue")]
    public async Task<ActionResult<RevenueReportResponse>> GetRevenueReport([FromQuery] ReportFilterRequest request)
    {
        if (request.DateFrom.HasValue && request.DateTo.HasValue && request.DateFrom > request.DateTo)
        {
            return BadRequest("DateFrom cannot be greater than DateTo.");
        }

        var response = await _reportService.GetRevenueReportAsync(request);

        return Ok(response);
    }
}
