namespace PontoAPP.Domain.Services;

/// <summary>
/// Serviço para publicar eventos de domínio
/// Pode ser implementado com MediatR ou outro mecanismo
/// </summary>
public interface IDomainEventService
{
    Task PublishAsync<TEvent>(TEvent domainEvent, CancellationToken cancellationToken = default) 
        where TEvent : class;
}