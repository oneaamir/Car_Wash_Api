using System.Security.Claims;
using CarWash.Backend.DTOs.Booking;
using CarWash.Backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarWash.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Customer")]
public class BookingsController : ControllerBase
{
    private readonly IBookingService _bookingService;

    public BookingsController(IBookingService bookingService)
    {
        _bookingService = bookingService;
    }

    [HttpPost]
    public async Task<ActionResult<BookingResponse>> CreateBooking(CreateBookingRequest request)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim))
        {
            return Unauthorized("Invalid token.");
        }

        var userId = int.Parse(userIdClaim);
        var result = await _bookingService.CreateBookingAsync(userId, request);

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

    [HttpGet("my")]
    public async Task<ActionResult<List<BookingResponse>>> GetMyBookings()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim))
        {
            return Unauthorized("Invalid token.");
        }

        var userId = int.Parse(userIdClaim);
        var bookings = await _bookingService.GetMyBookingsAsync(userId);

        return Ok(bookings);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BookingResponse>> GetBookingById(int id)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim))
        {
            return Unauthorized("Invalid token.");
        }

        var userId = int.Parse(userIdClaim);
        var response = await _bookingService.GetBookingByIdAsync(id, userId);

        if (response == null)
        {
            return NotFound("Booking not found.");
        }

        return Ok(response);
    }

    [HttpPut("{id}/cancel")]
    public async Task<ActionResult<BookingResponse>> CancelBooking(int id)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim))
        {
            return Unauthorized("Invalid token.");
        }

        var userId = int.Parse(userIdClaim);
        var result = await _bookingService.CancelBookingAsync(id, userId);

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
