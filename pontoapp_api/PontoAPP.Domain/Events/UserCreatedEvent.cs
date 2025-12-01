using MediatR;

namespace PontoAPP.Domain.Events;

/// <summary>
/// Evento disparado quando um novo usuário é criado
/// Pode ser usado para enviar email de boas-vindas, notificações, etc
/// </summary>
public class UserCreatedEvent : INotification
{
    public Guid UserId { get; }
    public Guid TenantId { get; }
    public string FullName { get; }
    public string Email { get; }
    public DateTime OccurredAt { get; }

    public UserCreatedEvent(Guid userId, Guid tenantId, string fullName, string email)
    {
        UserId = userId;
        TenantId = tenantId;
        FullName = fullName;
        Email = email;
        OccurredAt = DateTime.UtcNow;
    }
}