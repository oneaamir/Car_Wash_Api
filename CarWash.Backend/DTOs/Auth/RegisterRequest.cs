using System.ComponentModel.DataAnnotations;

namespace CarWash.Backend.DTOs.Auth;

public class RegisterRequest
{
    [Required]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Phone { get; set; } = string.Empty;

    [Required]
    [MinLength(6)]
    public string Password { get; set; } = string.Empty;

    [Required]
    [RegularExpression("^(Customer|Washer)$", ErrorMessage = "Role must be Customer or Washer.")]
    public string Role { get; set; } = "Customer";
}
