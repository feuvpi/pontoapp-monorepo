using MediatR;

namespace PontoAPP.Application.Queries.Reports;

public record GenerateCRPQuery(
    Guid TimeRecordId
) : IRequest<byte[]>;