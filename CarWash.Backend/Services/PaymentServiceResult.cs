using CarWash.Backend.DTOs.Payment;

namespace CarWash.Backend.Services;

public class PaymentServiceResult
{
    public bool IsSuccess { get; set; }

    public bool IsNotFound { get; set; }

    public string ErrorMessage { get; set; } = string.Empty;

    public PaymentResponse? Response { get; set; }
}
