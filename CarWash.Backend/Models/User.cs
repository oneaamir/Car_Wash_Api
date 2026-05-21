namespace CarWash.Backend.Models;

public class User
{
    public int Id { get; set; }

    public string FullName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public string Role { get; set; } = "Customer";

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }
    
    public List<Car> Cars { get; set; } = new();

}





// Agar tum property ko initialize nahi karte → default value null hoti hai.

// null hone se runtime error (NullReferenceException) aa sakta hai jab tum methods call karo.

// Agar tum string.Empty set karte ho → property hamesha empty string ("") hogi, safe rahegi.

// Best prac