using MediatR;
using Microsoft.Extensions.Logging;
using PontoAPP.Application.DTOs.Tenants;
using PontoAPP.Application.Exceptions;
using PontoAPP.Domain.Entities.Tenants;
using PontoAPP.Domain.Events;
using PontoAPP.Domain.Repositories;
using PontoAPP.Domain.Services;
using PontoAPP.Infrastructure.Data.Context;

namespace PontoAPP.Application.Commands.Tenants;

/// <summary>
/// Handles tenant creation:
/// 1. Validate uniqueness (slug, email)
/// 2. Create Tenant record
/// 3. Create Subscription (trial)
/// 4. Publish domain event
/// </summary>
public class CreateTenantCommandHandler(
    ITenantRepository tenantRepository,
    AppDbContext dbContext,
    IDomainEventService domainEventService,
    ILogger<CreateTenantCommandHandler> logger)
    : IRequestHandler<CreateTenantCommand, TenantResponse>
{
    public async Task<TenantResponse> Handle(CreateTenantCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Creating tenant: {TenantName} ({Slug})", request.Name, request.Slug);

        // 1. Validate uniqueness
        await ValidateUniquenessAsync(request, cancellationToken);

        // 2. Create Tenant entity
        var tenant = Tenant.Create(
            name: request.Name,
            slug: request.Slug,
            email: request.Email,
            cnpj: request.Cnpj
        );

        // 3. Create Subscription (trial)
        var subscription = Subscription.CreateTrial(
            tenantId: tenant.Id,
            trialDays: request.TrialDays,
            maxUsers: request.MaxUsers
        );

        try
        {
            // 4. Save tenant and subscription
            await tenantRepository.AddAsync(tenant, cancellationToken);
            await dbContext.Subscriptions.AddAsync(subscription, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);

            // 5. Publish domain event
            await domainEventService.PublishAsync(
                new TenantCreatedEvent(tenant.Id, tenant.Name),
                cancellationToken
            );

            logger.LogInformation("Tenant created: {TenantId} - {TenantName}", tenant.Id, tenant.Name);

            return MapToResponse(tenant, subscription);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating tenant: {TenantName}", request.Name);
            throw new BusinessException("Failed to create tenant. Please try again.", ex);
        }
    }

    private async Task ValidateUniquenessAsync(CreateTenantCommand request, CancellationToken cancellationToken)
    {
        if (await tenantRepository.SlugExistsAsync(request.Slug, cancellationToken))
        {
            throw new ValidationException($"Slug '{request.Slug}' is already in use.");
        }

        if (await tenantRepository.EmailExistsAsync(request.Email, cancellationToken))
        {
            throw new ValidationException($"Email '{request.Email}' is already registered.");
        }
    }

    private static TenantResponse MapToResponse(Tenant tenant, Subscription subscription)
    {
        return new TenantResponse
        {
            Id = tenant.Id,
            Name = tenant.Name,
            Slug = tenant.Slug,
            Email = tenant.Email.Value,
            CompanyDocument = tenant.CNPJ,
            IsActive = tenant.IsActive,
            CreatedAt = tenant.CreatedAt,
            Subscription = new SubscriptionInfo
            {
                Id = subscription.Id,
                Status = subscription.Status,
                StartDate = subscription.StartDate,
                TrialEndDate = subscription.TrialEndDate,
                EndDate = subscription.EndDate,
                MaxUsers = subscription.MaxUsers,
                MonthlyPrice = subscription.MonthlyPrice,
                IsValid = subscription.IsValid()
            }
        };
    }
}