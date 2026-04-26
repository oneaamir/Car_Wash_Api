namespace CarWash.Backend.Models;

public class Booking
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int CarId { get; set; }

    public int ServicePlanId { get; set; }

    public int? PromoCodeId { get; set; }

    public int? AssignedWasherId { get; set; }

    public string BookingType { get; set; } = string.Empty;

    public DateTime BookingDate { get; set; }

    public string Address { get; set; } = string.Empty;

    public string? Notes { get; set; }

    public string Status { get; set; } = "Pending";

    public decimal TotalAmount { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public User? User { get; set; }

    public User? AssignedWasher { get; set; }

    public Car? Car { get; set; }

    public ServicePlan? ServicePlan { get; set; }

    public PromoCode? PromoCode { get; set; }

    public List<BookingAddOn> BookingAddOns { get; set; } = new();
}
