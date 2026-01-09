using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PontoAPP.Application.DTOs.TimeRecords;
using PontoAPP.Domain.Enums;
using PontoAPP.Infrastructure.Data.Context;

namespace PontoAPP.Application.Queries.TimeRecords;

public class GetAllTimeRecordsQueryHandler(
    AppDbContext dbContext,
    ILogger<GetAllTimeRecordsQueryHandler> logger)
    : IRequestHandler<GetAllTimeRecordsQuery, IEnumerable<TimeRecordResponse>>
{
    public async Task<IEnumerable<TimeRecordResponse>> Handle(
        GetAllTimeRecordsQuery request, 
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Fetching time records for tenant: {TenantId}", request.TenantId);

        var query = dbContext.TimeRecords
            .Include(tr => tr.User)
            .Where(tr => tr.TenantId == request.TenantId);

        // Filter by user
        if (request.UserId.HasValue)
        {
            query = query.Where(tr => tr.UserId == request.UserId.Value);
        }

        // Filter by date range
        if (request.StartDate.HasValue)
        {
            query = query.Where(tr => tr.RecordedAt >= request.StartDate.Value);
        }

        if (request.EndDate.HasValue)
        {
            // Include entire end date
            var endOfDay = request.EndDate.Value.Date.AddDays(1);
            query = query.Where(tr => tr.RecordedAt < endOfDay);
        }

        // Filter by record type
        if (!string.IsNullOrWhiteSpace(request.RecordType))
        {
            if (Enum.TryParse<RecordType>(request.RecordType, true, out var recordType))
            {
                query = query.Where(tr => tr.Type == recordType);
            }
        }

        var records = await query
            .OrderByDescending(tr => tr.RecordedAt)
            .Take(500) // Limit for performance
            .ToListAsync(cancellationToken);

        logger.LogInformation("Found {Count} time records", records.Count);

        return records.Select(tr => new TimeRecordResponse
        {
            Id = tr.Id,
            UserId = tr.UserId,
            UserName = tr.User?.FullName ?? "Unknown",
            RecordedAt = tr.RecordedAt,
            Type = tr.Type.ToString(),
            Status = tr.Status.ToString(),
            AuthenticationType = tr.AuthenticationType.ToString(),
            Latitude = tr.Latitude,
            Longitude = tr.Longitude,
            Notes = tr.Notes,
            CreatedAt = tr.CreatedAt
        });
    }
}