namespace CarWash.Backend.DTOs.Admin;

public class UserSummaryResponse
{
    public int Id { get; set; }

    public string FullName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    public string Role { get; set; } = string.Empty;

    public bool IsActive { get; set; }

    public string Message { get; set; } = string.Empty;
}
