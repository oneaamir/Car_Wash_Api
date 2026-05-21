using CarWash.Backend.DTOs.Receipt;

namespace CarWash.Backend.Services.Interfaces;

public interface IReceiptService
{
    Task<ReceiptServiceResult> GenerateReceiptAsync(int bookingId, int userId);

    Task<ReceiptServiceResult> GetReceiptByBookingIdAsync(int bookingId, int userId);

    Task<List<ReceiptResponse>> GetMyReceiptsAsync(int userId);
}
