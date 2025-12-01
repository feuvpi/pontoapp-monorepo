using MediatR;
using Microsoft.Extensions.Logging;
using PontoAPP.Domain.Services;

namespace PontoAPP.Infrastructure.Services;

/// <summary>
/// Serviço para publicar eventos de domínio usando MediatR
/// </summary>
public class DomainEventService : IDomainEventService
{
    private readonly IMediator _mediator;
    private readonly ILogger<DomainEventService> _logger;

    public DomainEventService(IMediator mediator, ILogger<DomainEventService> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task PublishAsync<TEvent>(TEvent domainEvent, CancellationToken cancellationToken = default)
        where TEvent : class
    {
        try
        {
            _logger.LogInformation("Publishing domain event: {EventType}", typeof(TEvent).Name);
            
            await _mediator.Publish(domainEvent, cancellationToken);
            
            _logger.LogInformation("Domain event published successfully: {EventType}", typeof(TEvent).Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error publishing domain event: {EventType}", typeof(TEvent).Name);
            throw;
        }
    }
}