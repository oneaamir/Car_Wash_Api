namespace CarWash.Backend.DTOs.Admin;

public class AdminBookingResponse
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int CarId { get; set; }

    public int ServicePlanId { get; set; }

    public int? PromoCodeId { get; set; }

    public int? AssignedWasherId { get; set; }

    public string CustomerName { get; set; } = string.Empty;

    public string CustomerEmail { get; set; } = string.Empty;

    public string CarNumber { get; set; } = string.Empty;

    public string ServicePlanName { get; set; } = string.Empty;

    public string PromoCode { get; set; } = string.Empty;

    public string AssignedWasherName { get; set; } = string.Empty;

    public string AssignedWasherEmail { get; set; } = string.Empty;

    public List<string> AddOnNames { get; set; } = new();

    public string BookingType { get; set; } = string.Empty;

    public DateTime BookingDate { get; set; }

    public string Address { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public decimal TotalAmount { get; set; }

    public string Message { get; set; } = string.Empty;
}
