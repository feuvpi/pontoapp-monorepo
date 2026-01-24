using MediatR;
using PontoAPP.Domain.Services;

namespace PontoAPP.Application.Queries.Reports;

public class GenerateCRPQueryHandler(ICRPGenerator generator)
    : IRequestHandler<GenerateCRPQuery, byte[]>
{
    public async Task<byte[]> Handle(GenerateCRPQuery request, CancellationToken cancellationToken)
    {
        return await generator.GeneratePDFAsync(request.TimeRecordId, cancellationToken);
    }
}