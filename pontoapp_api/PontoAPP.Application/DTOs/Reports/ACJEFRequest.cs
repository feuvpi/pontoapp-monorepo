namespace PontoAPP.Application.DTOs.Reports;

public class ACJEFRequest
{
    public int Year { get; set; }
    public int Month { get; set; }
    public Guid? UserId { get; set; }
}