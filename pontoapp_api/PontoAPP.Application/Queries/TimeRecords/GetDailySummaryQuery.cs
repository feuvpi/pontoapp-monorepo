using MediatR;
using PontoAPP.Application.DTOs.TimeRecords;

namespace PontoAPP.Application.Queries.TimeRecords;

public class GetDailySummaryQuery : IRequest<DailySummaryResponse>
{
    public Guid UserId { get; set; }
    public DateTime? Date { get; set; } // null = hoje
}