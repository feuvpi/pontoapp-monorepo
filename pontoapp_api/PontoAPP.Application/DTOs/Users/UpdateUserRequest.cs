namespace PontoAPP.Application.DTOs.Users;

public class UpdateUserRequest
{
    public string? FullName { get; set; }
    public string? Email { get; set; }
    public string? Role { get; set; }
    public string? EmployeeCode { get; set; }
    public string? Department { get; set; }
    public DateTime? HiredAt { get; set; }
}
