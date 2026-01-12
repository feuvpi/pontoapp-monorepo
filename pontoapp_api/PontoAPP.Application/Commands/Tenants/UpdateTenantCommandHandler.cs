using MediatR;
using Microsoft.Extensions.Logging;
using PontoAPP.Application.DTOs.Tenants;
using PontoAPP.Application.Exceptions;
using PontoAPP.Domain.Repositories;

namespace PontoAPP.Application.Commands.Tenants;

public class UpdateTenantCommandHandler(
    ITenantRepository tenantRepository,
    ILogger<UpdateTenantCommandHandler> logger)
    : IRequestHandler<UpdateTenantCommand, TenantResponse>
{
    public async Task<TenantResponse> Handle(UpdateTenantCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Updating tenant: {TenantId}", request.TenantId);

        var tenant = await tenantRepository.GetByIdAsync(request.TenantId, cancellationToken);
        if (tenant == null)
        {
            throw new NotFoundException("Tenant não encontrado");
        }

        // Update tenant (você precisa ter esse método no Tenant entity)
        tenant.Update(request.Name, request.Email, request.CompanyDocument);

        tenantRepository.Update(tenant);
        await tenantRepository.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Tenant updated: {TenantId}", request.TenantId);

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