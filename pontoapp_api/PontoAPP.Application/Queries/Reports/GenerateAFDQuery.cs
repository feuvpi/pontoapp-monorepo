
using MediatR;

namespace PontoAPP.Application.Queries.Reports;

public record GenerateAFDQuery(
    DateTime StartDate,
    DateTime EndDate,
    Guid? UserId = null) : IRequest<string>;