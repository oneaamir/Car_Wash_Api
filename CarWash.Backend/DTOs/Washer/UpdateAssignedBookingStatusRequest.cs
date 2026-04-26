using System.ComponentModel.DataAnnotations;

namespace CarWash.Backend.DTOs.Washer;

public class UpdateAssignedBookingStatusRequest
{
    [Required]
    public string Status { get; set; } = string.Empty;
}
