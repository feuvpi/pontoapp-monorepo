using MediatR;
using Microsoft.Extensions.Logging;
using PontoAPP.Application.DTOs.Tenants;
using PontoAPP.Application.Exceptions;
using PontoAPP.Domain.Repositories;

namespace PontoAPP.Application.Queries.Tenants;

public class GetTenantByIdQueryHandler(
    ITenantRepository tenantRepository,
    ILogger<GetTenantByIdQueryHandler> logger)
    : IRequestHandler<GetTenantByIdQuery, TenantResponse>
{
    public async Task<TenantResponse> Handle(GetTenantByIdQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting tenant: {TenantId}", request.TenantId);

        var tenant = await tenantRepository.GetByIdAsync(request.TenantId, cancellationToken);
        
        if (tenant == null)
        {
            throw new NotFoundException("Tenant não encontrado");
        }

        return new TenantResponse
        {
            Id = tenant.Id,
            Name = tenant.Name,
            Slug = tenant.Slug,
            Email = tenant.Email.Value,
            CompanyDocument = tenant.CompanyDocument,
            IsActive = tenant.IsActive,
            CreatedAt = tenant.CreatedAt
        };
    }
}