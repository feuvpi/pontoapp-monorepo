using MediatR;

namespace PontoAPP.Application.Queries.Reports;

public record GenerateEspelhoQuery(
    Guid UserId,
    int Year,
    int Month
) : IRequest<byte[]>;