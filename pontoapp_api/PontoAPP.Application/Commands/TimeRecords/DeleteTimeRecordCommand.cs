using MediatR;

namespace PontoAPP.Application.Commands.TimeRecords;

public class DeleteTimeRecordCommand : IRequest<Unit>
{
    public Guid RecordId { get; set; }
}