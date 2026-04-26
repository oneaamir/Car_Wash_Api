using System.ComponentModel.DataAnnotations;

namespace CarWash.Backend.DTOs.Admin;

public class AssignWasherRequest
{
    [Required]
    public int WasherId { get; set; }
}
