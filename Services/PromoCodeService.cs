using CarWash.Backend.DTOs.PromoCode;
using CarWash.Backend.Models;
using CarWash.Backend.Repositories.Interfaces;
using CarWash.Backend.Services.Interfaces;

namespace CarWash.Backend.Services;

public class PromoCodeService : IPromoCodeService
{
    private readonly IPromoCodeRepository _promoCodeRepository;

    public PromoCodeService(IPromoCodeRepository promoCodeRepository)
    {
        _promoCodeRepository = promoCodeRepository;
    }

    public async Task<List<PromoCodeResponse>> GetAllPromoCodesAsync()
    {
        var promos = await _promoCodeRepository.GetAllActiveAsync();

        return promos
            .Select(promo => MapToResponse(promo, "Promo codes fetched successfully."))
            .ToList();
    }

    public async Task<PromoCodeResponse> CreatePromoCodeAsync(CreatePromoCodeRequest request)
    {
        var promo = new PromoCode
        {
            Code = request.Code,
            DiscountType = request.DiscountType,
            DiscountValue = request.DiscountValue,
            ExpiryDate = request.ExpiryDate
        };

        await _promoCodeRepository.AddAsync(promo);
        await _promoCodeRepository.SaveChangesAsync();

        return MapToResponse(promo, "Promo code created successfully.");
    }

    public async Task<PromoCodeResponse?> UpdatePromoCodeAsync(int id, UpdatePromoCodeRequest request)
    {
        var promo = await _promoCodeRepository.GetActiveByIdAsync(id);

        if (promo == null)
        {
            return null;
        }

        promo.Code = request.Code;
        promo.DiscountType = request.DiscountType;
        promo.DiscountValue = request.DiscountValue;
        promo.ExpiryDate = request.ExpiryDate;
        promo.UpdatedAt = DateTime.UtcNow;

        await _promoCodeRepository.SaveChangesAsync();

        return MapToResponse(promo, "Promo code updated successfully.");
    }

    public async Task<PromoCodeResponse?> DeletePromoCodeAsync(int id)
    {
        var promo = await _promoCodeRepository.GetActiveByIdAsync(id);

        if (promo == null)
        {
            return null;
        }

        promo.IsActive = false;
        promo.UpdatedAt = DateTime.UtcNow;

        await _promoCodeRepository.SaveChangesAsync();

        return MapToResponse(promo, "Promo code deleted successfully.");
    }

    public async Task<PromoValidationResult> ValidatePromoCodeAsync(ValidatePromoCodeRequest request)
    {
        var promo = await _promoCodeRepository.GetActiveByCodeAsync(request.Code);

        if (promo == null)
        {
            return new PromoValidationResult
            {
                IsNotFound = true,
                ErrorMessage = "Promo code not found."
            };
        }

        if (promo.ExpiryDate < DateTime.UtcNow)
        {
            return new PromoValidationResult
            {
                ErrorMessage = "Promo code has expired."
            };
        }

        decimal discountAmount = 0;

        if (promo.DiscountType.Equals("Flat", StringComparison.OrdinalIgnoreCase))
        {
            discountAmount = promo.DiscountValue;
        }
        else if (promo.DiscountType.Equals("Percentage", StringComparison.OrdinalIgnoreCase))
        {
            discountAmount = (request.OrderAmount * promo.DiscountValue) / 100;
        }
        else
        {
            return new PromoValidationResult
            {
                ErrorMessage = "Invalid discount type."
            };
        }

        if (discountAmount > request.OrderAmount)
        {
            discountAmount = request.OrderAmount;
        }

        return new PromoValidationResult
        {
            IsSuccess = true,
            Response = new PromoValidationResponse
            {
                Code = promo.Code,
                IsValid = true,
                OriginalAmount = request.OrderAmount,
                DiscountAmount = discountAmount,
                FinalAmount = request.OrderAmount - discountAmount,
                Message = "Promo code is valid."
            }
        };
    }

    private static PromoCodeResponse MapToResponse(PromoCode promo, string message)
    {
        return new PromoCodeResponse
        {
            Id = promo.Id,
            Code = promo.Code,
            DiscountType = promo.DiscountType,
            DiscountValue = promo.DiscountValue,
            ExpiryDate = promo.ExpiryDate,
            IsActive = promo.IsActive,
            Message = message
        };
    }
}
