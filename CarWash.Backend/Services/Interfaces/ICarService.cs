using CarWash.Backend.DTOs.Car;

namespace CarWash.Backend.Services.Interfaces;

public interface ICarService
{
    Task<CarResponse> CreateCarAsync(int userId, CreateCarRequest request);

    Task<List<CarResponse>> GetMyCarsAsync(int userId);

    Task<CarResponse?> GetCarByIdAsync(int id, int userId);

    Task<CarResponse?> UpdateCarAsync(int id, int userId, UpdateCarRequest request);

    Task<CarResponse?> DeleteCarAsync(int id, int userId);
}
