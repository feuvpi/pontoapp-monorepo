// PontoAPP.Application/Queries/Reports/GenerateACJEFQueryHandler.cs
using MediatR;
using PontoAPP.Domain.Models.Reports;
using PontoAPP.Domain.Services;
using PontoAPP.Infrastructure.Multitenancy;

namespace PontoAPP.Application.Queries.Reports;

public class GenerateACJEFQueryHandler : IRequestHandler<GenerateACJEFQuery, ACJEFModel>
{
    private readonly IACJEFGenerator _acjefGenerator;
    private readonly ITenantAccessor _tenantAccessor;

    public GenerateACJEFQueryHandler(
        IACJEFGenerator acjefGenerator,
        ITenantAccessor tenantAccessor)
    {
        _acjefGenerator = acjefGenerator;
        _tenantAccessor = tenantAccessor;
    }

    public async Task<ACJEFModel> Handle(GenerateACJEFQuery request, CancellationToken cancellationToken)
    {
        var tenant = _tenantAccessor.GetTenantInfo()
                       ?? throw new UnauthorizedAccessException("Tenant não identificado");

        return await _acjefGenerator.GenerateACJEFAsync(
            tenant.TenantId,
            request.Year,
            request.Month,
            request.UserId,
            cancellationToken);
    }
}