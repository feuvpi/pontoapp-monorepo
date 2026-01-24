using MediatR;
using PontoAPP.Application.Exceptions;
using PontoAPP.Domain.Services;
using PontoAPP.Infrastructure.Multitenancy;

namespace PontoAPP.Application.Queries.Reports;

public class GenerateEspelhoQueryHandler(
    IEspelhoPontoGenerator generator,
    ITenantAccessor tenantAccessor)
    : IRequestHandler<GenerateEspelhoQuery, byte[]>
{
    public async Task<byte[]> Handle(GenerateEspelhoQuery request, CancellationToken cancellationToken)
    {
        var tenant = tenantAccessor.GetTenantInfo()
                       ?? throw new UnauthorizedAccessException("Tenant não identificado");

        if (request.Month < 1 || request.Month > 12)
            throw new ValidationException("Mês deve estar entre 1 e 12");

        if (request.Year < 2000 || request.Year > DateTime.Now.Year + 1)
            throw new ValidationException("Ano inválido");

        return await generator.GeneratePDFAsync(
            tenant.TenantId,
            request.UserId,
            request.Year,
            request.Month,
            cancellationToken);
    }
}