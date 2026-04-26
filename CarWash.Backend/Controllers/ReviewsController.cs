using System.Security.Claims;
using CarWash.Backend.DTOs.Review;
using CarWash.Backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarWash.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Customer")]
public class ReviewsController : ControllerBase
{
    private readonly IReviewService _reviewService;

    public ReviewsController(IReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    [HttpPost]
    public async Task<ActionResult<ReviewResponse>> CreateReview(CreateReviewRequest request)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim))
        {
            return Unauthorized("Invalid token.");
        }

        var userId = int.Parse(userIdClaim);
        var result = await _reviewService.CreateReviewAsync(userId, request);

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

    [AllowAnonymous]
    [HttpGet("booking/{bookingId}")]
    public async Task<ActionResult<List<ReviewResponse>>> GetReviewsByBookingId(int bookingId)
    {
        var reviews = await _reviewService.GetReviewsByBookingIdAsync(bookingId);

        return Ok(reviews);
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<List<ReviewResponse>>> GetAllReviews()
    {
        var reviews = await _reviewService.GetAllReviewsAsync();

        return Ok(reviews);
    }
}
