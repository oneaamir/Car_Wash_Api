using CarWash.Backend.DTOs.PromoCode;
using CarWash.Backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarWash.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class PromoCodesController : ControllerBase
{
    private readonly IPromoCodeService _promoCodeService;

    public PromoCodesController(IPromoCodeService promoCodeService)
    {
        _promoCodeService = promoCodeService;
    }

    [AllowAnonymous]
    [HttpPost("validate")]
    public async Task<ActionResult<PromoValidationResponse>> ValidatePromoCode(ValidatePromoCodeRequest request)
    {
        var result = await _promoCodeService.ValidatePromoCodeAsync(request);

        if (result.IsNotFound)
        {
            return NotFound("Promo code not found.");
        }

        if (!result.IsSuccess)
        {
            return BadRequest(result.ErrorMessage);
        }

        return Ok(result.Response);
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<List<PromoCodeResponse>>> GetAllPromoCodes()
    {
        var promos = await _promoCodeService.GetAllPromoCodesAsync();

        return Ok(promos);
    }

    [HttpPost]
    public async Task<ActionResult<PromoCodeResponse>> CreatePromoCode(CreatePromoCodeRequest request)
    {
        var response = await _promoCodeService.CreatePromoCodeAsync(request);

        return Ok(response);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<PromoCodeResponse>> UpdatePromoCode(int id, UpdatePromoCodeRequest request)
    {
        var response = await _promoCodeService.UpdatePromoCodeAsync(id, request);

        if (response == null)
        {
            return NotFound("Promo code not found.");
        }

        return Ok(response);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<PromoCodeResponse>> DeletePromoCode(int id)
    {
        var response = await _promoCodeService.DeletePromoCodeAsync(id);

        if (response == null)
        {
            return NotFound("Promo code not found.");
        }

        return Ok(response);
    }
}
