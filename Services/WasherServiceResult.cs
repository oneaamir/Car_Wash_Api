using CarWash.Backend.DTOs.Booking;

namespace CarWash.Backend.Services;

public class WasherServiceResult
{
    public bool IsSuccess { get; set; }

    public bool IsNotFound { get; set; }

    public string ErrorMessage { get; set; } = string.Empty;

    public BookingResponse? Response { get; set; }
}
