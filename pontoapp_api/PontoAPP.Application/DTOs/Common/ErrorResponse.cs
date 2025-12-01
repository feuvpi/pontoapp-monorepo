namespace PontoAPP.Application.DTOs.Common;

/// <summary>
/// Standard API response wrapper for errors
/// </summary>
public class ErrorResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public int StatusCode { get; set; }
    public Dictionary<string, string[]>? Errors { get; set; }
    public DateTime Timestamp { get; set; }

    public ErrorResponse()
    {
        Success = false;
        Timestamp = DateTime.UtcNow;
    }

    public static ErrorResponse BadRequest(string message, Dictionary<string, string[]>? errors = null)
    {
        return new ErrorResponse
        {
            StatusCode = 400,
            Message = message,
            Errors = errors
        };
    }

    public static ErrorResponse Unauthorized(string message = "Unauthorized")
    {
        return new ErrorResponse
        {
            StatusCode = 401,
            Message = message
        };
    }

    public static ErrorResponse Forbidden(string message = "Forbidden")
    {
        return new ErrorResponse
        {
            StatusCode = 403,
            Message = message
        };
    }

    public static ErrorResponse NotFound(string message)
    {
        return new ErrorResponse
        {
            StatusCode = 404,
            Message = message
        };
    }

    public static ErrorResponse InternalServerError(string message = "An internal error occurred")
    {
        return new ErrorResponse
        {
            StatusCode = 500,
            Message = message
        };
    }
}