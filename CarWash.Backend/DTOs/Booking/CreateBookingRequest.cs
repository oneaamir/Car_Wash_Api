using System.ComponentModel.DataAnnotations;

namespace CarWash.Backend.DTOs.Booking;

public class CreateBookingRequest
{
    [Required]
    public int CarId { get; set; }

    [Required]
    public int ServicePlanId { get; set; }

    public List<int> AddOnIds { get; set; } = new();

    public string PromoCode { get; set; } = string.Empty;

    [Required]
    public string BookingType { get; set; } = string.Empty;

    [Required]
    public DateTime BookingDate { get; set; }

    [Required]
    public string Address { get; set; } = string.Empty;

    public string Notes { get; set; } = string.Empty;
}
