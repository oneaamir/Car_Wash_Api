using CarWash.Backend.DTOs.Car;
using CarWash.Backend.Models;
using CarWash.Backend.Repositories.Interfaces;
using CarWash.Backend.Services.Interfaces;

namespace CarWash.Backend.Services;

public class CarService : ICarService
{
    private readonly ICarRepository _carRepository;

    public CarService(ICarRepository carRepository)
    {
        _carRepository = carRepository;
    }

    public async Task<CarResponse> CreateCarAsync(int userId, CreateCarRequest request)
    {
        var car = new Car
        {
            UserId = userId,
            CarNumber = request.CarNumber,
            Brand = request.Brand,
            Model = request.Model,
            CarType = request.CarType,
            ImageUrl = request.ImageUrl
        };

        await _carRepository.AddAsync(car);
        await _carRepository.SaveChangesAsync();

        return MapToResponse(car, "Car added successfully.");
    }

    public async Task<List<CarResponse>> GetMyCarsAsync(int userId)
    {
        var cars = await _carRepository.GetActiveByUserIdAsync(userId);

        return cars
            .Select(car => MapToResponse(car, "Cars fetched successfully."))
            .ToList();
    }

    public async Task<CarResponse?> GetCarByIdAsync(int id, int userId)
    {
        var car = await _carRepository.GetActiveByIdAsync(id, userId);

        return car == null ? null : MapToResponse(car, "Car fetched successfully.");
    }

    public async Task<CarResponse?> UpdateCarAsync(int id, int userId, UpdateCarRequest request)
    {
        var car = await _carRepository.GetActiveByIdAsync(id, userId);

        if (car == null)
        {
            return null;
        }

        car.CarNumber = request.CarNumber;
        car.Brand = request.Brand;
        car.Model = request.Model;
        car.CarType = request.CarType;
        car.ImageUrl = request.ImageUrl;
        car.UpdatedAt = DateTime.UtcNow;

        await _carRepository.SaveChangesAsync();

        return MapToResponse(car, "Car updated successfully.");
    }

    public async Task<CarResponse?> DeleteCarAsync(int id, int userId)
    {
        var car = await _carRepository.GetActiveByIdAsync(id, userId);

        if (car == null)
        {
            return null;
        }

        car.IsActive = false;
        car.UpdatedAt = DateTime.UtcNow;

        await _carRepository.SaveChangesAsync();

        return MapToResponse(car, "Car deleted successfully.");
    }

    private static CarResponse MapToResponse(Car car, string message)
    {
        return new CarResponse
        {
            Id = car.Id,
            UserId = car.UserId,
            CarNumber = car.CarNumber,
            Brand = car.Brand,
            Model = car.Model,
            CarType = car.CarType,
            ImageUrl = car.ImageUrl,
            IsActive = car.IsActive,
            Message = message
        };
    }
}
