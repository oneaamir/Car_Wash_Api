using System.Security.Claims;
using CarWash.Backend.DTOs.Booking;
using CarWash.Backend.DTOs.Washer;
using CarWash.Backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarWash.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Washer")]
public class WashersController : ControllerBase
{
    private readonly IWasherService _washerService;

    public WashersController(IWasherService washerService)
    {
        _washerService = washerService;
    }

    [HttpGet("bookings")]
    public async Task<ActionResult<List<BookingResponse>>> GetAssignedBookings()
    {
        var washerIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(washerIdClaim))
        {
            return Unauthorized("Invalid token.");
        }

        var washerId = int.Parse(washerIdClaim);
        var bookings = await _washerService.GetAssignedBookingsAsync(washerId);

        return Ok(bookings);
    }

    [HttpGet("bookings/{id}")]
    public async Task<ActionResult<BookingResponse>> GetAssignedBookingById(int id)
    {
        var washerIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(washerIdClaim))
        {
            return Unauthorized("Invalid token.");
        }

        var washerId = int.Parse(washerIdClaim);
        var booking = await _washerService.GetAssignedBookingByIdAsync(id, washerId);

        if (booking == null)
        {
            return NotFound("Assigned booking not found.");
        }

        return Ok(booking);
    }

    [HttpPut("bookings/{id}/status")]
    public async Task<ActionResult<BookingResponse>> UpdateAssignedBookingStatus(int id, UpdateAssignedBookingStatusRequest request)
    {
        var washerIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(washerIdClaim))
        {
            return Unauthorized("Invalid token.");
        }

        var washerId = int.Parse(washerIdClaim);
        var result = await _washerService.UpdateAssignedBookingStatusAsync(id, washerId, request);

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
