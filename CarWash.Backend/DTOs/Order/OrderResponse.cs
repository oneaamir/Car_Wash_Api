namespace CarWash.Backend.DTOs.Order;

public class OrderResponse
{
    public int BookingId { get; set; }

    public string CarDisplay { get; set; } = string.Empty;

    public string ServicePlanName { get; set; } = string.Empty;

    public List<string> AddOnNames { get; set; } = new();

    public string PromoCode { get; set; } = string.Empty;

    public DateTime BookingDate { get; set; }

    public string BookingType { get; set; } = string.Empty;

    public string Address { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public decimal TotalAmount { get; set; }

    public string AssignedWasherName { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;
}
