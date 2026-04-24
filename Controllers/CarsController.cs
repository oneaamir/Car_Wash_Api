using System.Security.Claims;
using CarWash.Backend.Data;
using CarWash.Backend.DTOs.Car;
using CarWash.Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarWash.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CarsController : ControllerBase
{
    private readonly AppDbContext _context;

    public CarsController(AppDbContext context)
    {
        _context = context;
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

        var car = new Car
        {
            UserId = userId,
            CarNumber = request.CarNumber,
            Brand = request.Brand,
            Model = request.Model,
            CarType = request.CarType,
            ImageUrl = request.ImageUrl
        };

        _context.Cars.Add(car);
        await _context.SaveChangesAsync();

        var response = new CarResponse
        {
            Id = car.Id,
            UserId = car.UserId,
            CarNumber = car.CarNumber,
            Brand = car.Brand,
            Model = car.Model,
            CarType = car.CarType,
            ImageUrl = car.ImageUrl,
            IsActive = car.IsActive,
            Message = "Car added successfully."
        };

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

        var cars = await _context.Cars
            .Where(car => car.UserId == userId && car.IsActive)
            .Select(car => new CarResponse
            {
                Id = car.Id,
                UserId = car.UserId,
                CarNumber = car.CarNumber,
                Brand = car.Brand,
                Model = car.Model,
                CarType = car.CarType,
                ImageUrl = car.ImageUrl,
                IsActive = car.IsActive,
                Message = "Cars fetched successfully."
            })
            .ToListAsync();

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

    var car = await _context.Cars
        .FirstOrDefaultAsync(car => car.Id == id && car.UserId == userId && car.IsActive);

    if (car == null)
    {
        return NotFound("Car not found.");
    }

    var response = new CarResponse
    {
        Id = car.Id,
        UserId = car.UserId,
        CarNumber = car.CarNumber,
        Brand = car.Brand,
        Model = car.Model,
        CarType = car.CarType,
        ImageUrl = car.ImageUrl,
        IsActive = car.IsActive,
        Message = "Car fetched successfully."
    };

    return Ok(response);
}

}
