using CarWash.Backend.DTOs.Admin;
using CarWash.Backend.DTOs.Payment;
using CarWash.Backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarWash.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly IAdminService _adminService;

    public AdminController(IAdminService adminService)
    {
        _adminService = adminService;
    }

    [HttpGet("users")]
    public async Task<ActionResult<List<UserSummaryResponse>>> GetUsers()
    {
        var users = await _adminService.GetUsersAsync();

        return Ok(users);
    }

    [HttpGet("bookings")]
    public async Task<ActionResult<List<AdminBookingResponse>>> GetBookings()
    {
        var bookings = await _adminService.GetBookingsAsync();

        return Ok(bookings);
    }

    [HttpPut("bookings/{id}/assign-washer")]
    public async Task<ActionResult<AdminBookingResponse>> AssignWasher(int id, AssignWasherRequest request)
    {
        var result = await _adminService.AssignWasherAsync(id, request);

        if (result.IsNotFound)
        {
            return NotFound(result.ErrorMessage);
        }

        if (!result.IsSuccess)
        {
            return BadRequest(result.ErrorMessage);
        }

        return Ok(result.Response);
    }

    [HttpPut("bookings/{id}/status")]
    public async Task<ActionResult<AdminBookingResponse>> UpdateBookingStatus(int id, UpdateBookingStatusRequest request)
    {
        var result = await _adminService.UpdateBookingStatusAsync(id, request);

        if (result.IsNotFound)
        {
            return NotFound(result.ErrorMessage);
        }

        if (!result.IsSuccess)
        {
            return BadRequest(result.ErrorMessage);
        }

        return Ok(result.Response);
    }

    [HttpGet("payments")]
    public async Task<ActionResult<List<PaymentResponse>>> GetAllPayments()
    {
        var payments = await _adminService.GetAllPaymentsAsync();

        return Ok(payments);
    }

    [HttpPut("payments/{id}/status")]
    public async Task<ActionResult<PaymentResponse>> UpdatePaymentStatus(int id, UpdatePaymentStatusRequest request)
    {
        var result = await _adminService.UpdatePaymentStatusAsync(id, request);

        if (result.IsNotFound)
        {
            return NotFound(result.ErrorMessage);
        }

        if (!result.IsSuccess)
        {
            return BadRequest(result.ErrorMessage);
        }

        return Ok(result.Response);
    }
}
