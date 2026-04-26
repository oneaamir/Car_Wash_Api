using System.ComponentModel.DataAnnotations;

namespace CarWash.Backend.DTOs.Admin;

public class UpdateBookingStatusRequest
{
    [Required]
    public string Status { get; set; } = string.Empty;
}
