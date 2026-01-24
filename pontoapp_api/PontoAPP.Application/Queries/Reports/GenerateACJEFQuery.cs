using MediatR;
using PontoAPP.Domain.Models.Reports;

namespace PontoAPP.Application.Queries.Reports;

public record GenerateACJEFQuery(
    int Year,
    int Month,
    Guid? UserId = null) : IRequest<ACJEFModel>;