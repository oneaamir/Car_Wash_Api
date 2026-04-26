using CarWash.Backend.DTOs.AddOn;
using CarWash.Backend.Models;
using CarWash.Backend.Repositories.Interfaces;
using CarWash.Backend.Services.Interfaces;

namespace CarWash.Backend.Services;

public class AddOnService : IAddOnService
{
    private readonly IAddOnRepository _addOnRepository;

    public AddOnService(IAddOnRepository addOnRepository)
    {
        _addOnRepository = addOnRepository;
    }

    public async Task<List<AddOnResponse>> GetAllAddOnsAsync()
    {
        var addOns = await _addOnRepository.GetAllActiveAsync();

        return addOns
            .Select(addOn => MapToResponse(addOn, "Add-ons fetched successfully."))
            .ToList();
    }

    public async Task<AddOnResponse?> GetAddOnByIdAsync(int id)
    {
        var addOn = await _addOnRepository.GetActiveByIdAsync(id);

        return addOn == null ? null : MapToResponse(addOn, "Add-on fetched successfully.");
    }

    public async Task<AddOnResponse> CreateAddOnAsync(CreateAddOnRequest request)
    {
        var addOn = new AddOn
        {
            Name = request.Name,
            Price = request.Price
        };

        await _addOnRepository.AddAsync(addOn);
        await _addOnRepository.SaveChangesAsync();

        return MapToResponse(addOn, "Add-on created successfully.");
    }

    public async Task<AddOnResponse?> UpdateAddOnAsync(int id, UpdateAddOnRequest request)
    {
        var addOn = await _addOnRepository.GetActiveByIdAsync(id);

        if (addOn == null)
        {
            return null;
        }

        addOn.Name = request.Name;
        addOn.Price = request.Price;
        addOn.UpdatedAt = DateTime.UtcNow;

        await _addOnRepository.SaveChangesAsync();

        return MapToResponse(addOn, "Add-on updated successfully.");
    }

    public async Task<AddOnResponse?> DeleteAddOnAsync(int id)
    {
        var addOn = await _addOnRepository.GetActiveByIdAsync(id);

        if (addOn == null)
        {
            return null;
        }

        addOn.IsActive = false;
        addOn.UpdatedAt = DateTime.UtcNow;

        await _addOnRepository.SaveChangesAsync();

        return MapToResponse(addOn, "Add-on deleted successfully.");
    }

    private static AddOnResponse MapToResponse(AddOn addOn, string message)
    {
        return new AddOnResponse
        {
            Id = addOn.Id,
            Name = addOn.Name,
            Price = addOn.Price,
            IsActive = addOn.IsActive,
            Message = message
        };
    }
}
