using MediatR;
using Microsoft.Extensions.Logging;
using PontoAPP.Application.Exceptions;
using PontoAPP.Domain.Repositories;

namespace PontoAPP.Application.Commands.TimeRecords;

public class DeleteTimeRecordCommandHandler(
    ITimeRecordRepository timeRecordRepository,
    ILogger<DeleteTimeRecordCommandHandler> logger)
    : IRequestHandler<DeleteTimeRecordCommand, Unit>
{
    public async Task<Unit> Handle(
        DeleteTimeRecordCommand request, 
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Deleting time record: {RecordId}", request.RecordId);

        var timeRecord = await timeRecordRepository.GetByIdAsync(request.RecordId, cancellationToken);
        if (timeRecord == null)
        {
            throw new NotFoundException("Registro não encontrado.");
        }

        timeRecordRepository.Remove(timeRecord);
        await timeRecordRepository.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Time record deleted: {RecordId}", request.RecordId);

        return Unit.Value;
    }
}