namespace PontoAPP.Application.DTOs.Users;

public class CreateUserRequest
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Role { get; set; } = "Employee"; // Employee, Manager, Admin
    public string? EmployeeCode { get; set; }
    public string? Department { get; set; }
    public DateTime? HiredAt { get; set; }
}
