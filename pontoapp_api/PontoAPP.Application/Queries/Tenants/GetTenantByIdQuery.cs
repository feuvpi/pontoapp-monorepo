using MediatR;
using PontoAPP.Application.DTOs.Tenants;

namespace PontoAPP.Application.Queries.Tenants;

public class GetTenantByIdQuery : IRequest<TenantResponse>
{
    public Guid TenantId { get; set; }
}