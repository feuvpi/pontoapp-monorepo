using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using PontoAPP.Infrastructure.Data.Context;
using PontoAPP.Infrastructure.Multitenancy;

namespace PontoAPP.API.Filters;

public class SubscriptionValidationFilter(
    AppDbContext dbContext,
    ITenantAccessor tenantAccessor) : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next)
    {
        var tenantInfo = tenantAccessor.GetTenantInfo();
        
        if (tenantInfo == null)
        {
            await next();
            return;
        }

        var subscription = await dbContext.Subscriptions
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.TenantId == tenantInfo.TenantId);

        if (subscription == null || !subscription.IsValid())
        {
            context.Result = new ObjectResult(new
            {
                error = "Subscription inativa",
                message = "Sua subscription expirou. Renove para continuar.",
                trialExpired = subscription?.TrialEndDate < DateTime.UtcNow
            })
            {
                StatusCode = 402
            };
            return;
        }

        await next();
    }
}