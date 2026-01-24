using MediatR;
using PontoAPP.Domain.Services;
using PontoAPP.Infrastructure.Multitenancy;

namespace PontoAPP.Application.Queries.Reports;

public class GenerateAFDQueryHandler : IRequestHandler<GenerateAFDQuery, string>
{
    private readonly IAFDGenerator _afdGenerator;
    private readonly ITenantAccessor _tenantAccessor;

    public GenerateAFDQueryHandler(
        IAFDGenerator afdGenerator,
        ITenantAccessor tenantAccessor)
    {
        _afdGenerator = afdGenerator;
        _tenantAccessor = tenantAccessor;
    }

    public async Task<string> Handle(GenerateAFDQuery request, CancellationToken cancellationToken)
    {
        var tenant = _tenantAccessor.GetTenantInfo() 
                       ?? throw new UnauthorizedAccessException("Tenant não identificado");

        return await _afdGenerator.GenerateAFDAsync(
            tenant.TenantId,
            request.StartDate,
            request.EndDate,
            request.UserId,
            cancellationToken);
    }
}