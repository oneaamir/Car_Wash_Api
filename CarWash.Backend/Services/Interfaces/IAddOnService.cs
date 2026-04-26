using CarWash.Backend.DTOs.AddOn;

namespace CarWash.Backend.Services.Interfaces;

public interface IAddOnService
{
    Task<List<AddOnResponse>> GetAllAddOnsAsync();

    Task<AddOnResponse?> GetAddOnByIdAsync(int id);

    Task<AddOnResponse> CreateAddOnAsync(CreateAddOnRequest request);

    Task<AddOnResponse?> UpdateAddOnAsync(int id, UpdateAddOnRequest request);

    Task<AddOnResponse?> DeleteAddOnAsync(int id);
}
