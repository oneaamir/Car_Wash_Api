using CarWash.Backend.DTOs.PromoCode;

namespace CarWash.Backend.Services.Interfaces;

public interface IPromoCodeService
{
    Task<List<PromoCodeResponse>> GetAllPromoCodesAsync();

    Task<PromoCodeResponse> CreatePromoCodeAsync(CreatePromoCodeRequest request);

    Task<PromoCodeResponse?> UpdatePromoCodeAsync(int id, UpdatePromoCodeRequest request);

    Task<PromoCodeResponse?> DeletePromoCodeAsync(int id);

    Task<PromoValidationResult> ValidatePromoCodeAsync(ValidatePromoCodeRequest request);
}
