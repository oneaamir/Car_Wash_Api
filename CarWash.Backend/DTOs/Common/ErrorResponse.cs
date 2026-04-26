namespace CarWash.Backend.DTOs.Common;

public class ErrorResponse
{
    public string Message { get; set; } = string.Empty;

    public string TraceId { get; set; } = string.Empty;

    public string? Details { get; set; }
}
