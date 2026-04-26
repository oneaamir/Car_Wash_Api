namespace CarWash.Backend.Services;

public class AdminServiceResult<T>
{
    public bool IsSuccess { get; set; }

    public bool IsNotFound { get; set; }

    public string ErrorMessage { get; set; } = string.Empty;

    public T? Response { get; set; }
}
