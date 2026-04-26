using CarWash.Backend.DTOs.Receipt;

namespace CarWash.Backend.Services;

public class ReceiptServiceResult
{
    public bool IsSuccess { get; set; }

    public bool IsNotFound { get; set; }

    public string ErrorMessage { get; set; } = string.Empty;

    public ReceiptResponse? Response { get; set; }
}
