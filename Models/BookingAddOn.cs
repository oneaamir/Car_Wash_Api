namespace CarWash.Backend.Models;

public class BookingAddOn
{
    public int Id { get; set; }

    public int BookingId { get; set; }

    public int AddOnId { get; set; }

    public Booking? Booking { get; set; }

    public AddOn? AddOn { get; set; }
}
