namespace PontoAPP.Application.DTOs.Users;

public class UserResponse
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public string? EmployeeCode { get; set; }
    public string? Department { get; set; }
    public DateTime? HiredAt { get; set; }
    public DateTime CreatedAt { get; set; }
}