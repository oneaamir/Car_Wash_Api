using CarWash.Backend.DTOs.AddOn;
using CarWash.Backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarWash.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class AddOnsController : ControllerBase
{
    private readonly IAddOnService _addOnService;

    public AddOnsController(IAddOnService addOnService)
    {
        _addOnService = addOnService;
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<List<AddOnResponse>>> GetAllAddOns()
    {
        var addOns = await _addOnService.GetAllAddOnsAsync();

        return Ok(addOns);
    }

    [AllowAnonymous]
    [HttpGet("{id}")]
    public async Task<ActionResult<AddOnResponse>> GetAddOnById(int id)
    {
        var response = await _addOnService.GetAddOnByIdAsync(id);

        if (response == null)
        {
            return NotFound("Add-on not found.");
        }

        return Ok(response);
    }

    [HttpPost]
    public async Task<ActionResult<AddOnResponse>> CreateAddOn(CreateAddOnRequest request)
    {
        var response = await _addOnService.CreateAddOnAsync(request);

        return Ok(response);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<AddOnResponse>> UpdateAddOn(int id, UpdateAddOnRequest request)
    {
        var response = await _addOnService.UpdateAddOnAsync(id, request);

        if (response == null)
        {
            return NotFound("Add-on not found.");
        }

        return Ok(response);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<AddOnResponse>> DeleteAddOn(int id)
    {
        var response = await _addOnService.DeleteAddOnAsync(id);

        if (response == null)
        {
            return NotFound("Add-on not found.");
        }

        return Ok(response);
    }
}
