using System.Security.Claims;
using CarWash.Backend.DTOs.Car;
using CarWash.Backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarWash.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Customer")]
public class CarsController : ControllerBase
{
    private readonly ICarService _carService;

    public CarsController(ICarService carService)
    {
        _carService = carService;
    }

    [HttpPost]
    public async Task<ActionResult<CarResponse>> CreateCar(CreateCarRequest request)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim))
        {
            return Unauthorized("Invalid token.");
        }

        var userId = int.Parse(userIdClaim);
        var response = await _carService.CreateCarAsync(userId, request);

        return Ok(response);
    }

    [HttpGet("my")]
    public async Task<ActionResult<List<CarResponse>>> GetMyCars()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim))
        {
            return Unauthorized("Invalid token.");
        }

        var userId = int.Parse(userIdClaim);
        var cars = await _carService.GetMyCarsAsync(userId);

        return Ok(cars);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CarResponse>> GetCarById(int id)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim))
        {
            return Unauthorized("Invalid token.");
        }

        var userId = int.Parse(userIdClaim);

        var response = await _carService.GetCarByIdAsync(id, userId);

        if (response == null)
        {
            return NotFound("Car not found.");
        }

        return Ok(response);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<CarResponse>> UpdateCar(int id, UpdateCarRequest request)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim))
        {
            return Unauthorized("Invalid token.");
        }

        var userId = int.Parse(userIdClaim);

        var response = await _carService.UpdateCarAsync(id, userId, request);

        if (response == null)
        {
            return NotFound("Car not found.");
        }

        return Ok(response);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<CarResponse>> DeleteCar(int id)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim))
        {
            return Unauthorized("Invalid token.");
        }

        var userId = int.Parse(userIdClaim);

        var response = await _carService.DeleteCarAsync(id, userId);

        if (response == null)
        {
            return NotFound("Car not found.");
        }

        return Ok(response);
    }
}
