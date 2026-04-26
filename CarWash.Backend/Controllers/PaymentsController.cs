using System.Security.Claims;
using CarWash.Backend.DTOs.Payment;
using CarWash.Backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarWash.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Customer")]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentService _paymentService;

    public PaymentsController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    [HttpPost]
    public async Task<ActionResult<PaymentResponse>> CreatePayment(CreatePaymentRequest request)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim))
        {
            return Unauthorized("Invalid token.");
        }

        var userId = int.Parse(userIdClaim);
        var result = await _paymentService.CreatePaymentAsync(userId, request);

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

    [HttpGet("{bookingId}")]
    public async Task<ActionResult<PaymentResponse>> GetPaymentByBookingId(int bookingId)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim))
        {
            return Unauthorized("Invalid token.");
        }

        var userId = int.Parse(userIdClaim);
        var result = await _paymentService.GetPaymentByBookingIdAsync(bookingId, userId);

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
