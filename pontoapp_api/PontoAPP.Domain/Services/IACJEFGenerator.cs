// PontoAPP.Domain/Services/IACJEFGenerator.cs
using PontoAPP.Domain.Models.Reports;

namespace PontoAPP.Domain.Services;

public interface IACJEFGenerator
{
    Task<ACJEFModel> GenerateACJEFAsync(
        Guid tenantId,
        int year,
        int month,
        Guid? userId = null,
        CancellationToken cancellationToken = default);
}