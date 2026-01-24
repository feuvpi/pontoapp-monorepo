namespace PontoAPP.Application.DTOs.Reports;

public class AFDRequest
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public Guid? UserId { get; set; } // Opcional - null = todos os usuários
}