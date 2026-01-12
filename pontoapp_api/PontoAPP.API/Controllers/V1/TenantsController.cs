using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PontoAPP.Application.Commands.Tenants;
using PontoAPP.Application.DTOs.Tenants;
using PontoAPP.Application.Exceptions;
using PontoAPP.Application.Queries.Tenants;
using PontoAPP.Application.Validators.Tenants;
using PontoAPP.Infrastructure.Multitenancy;
using ValidationException = System.ComponentModel.DataAnnotations.ValidationException;

namespace PontoAPP.API.Controllers.V1;

/// <summary>
/// Tenant management endpoints
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Tags("Tenants")]
public class TenantsController(
    IMediator mediator,
    ILogger<TenantsController> logger,
    CreateTenantRequestValidator validator,
    ITenantAccessor tenantAccessor)
    : BaseController(mediator, logger)
{
    /// <summary>
    /// Creates a new tenant (company) with dedicated database schema
    /// </summary>
    /// <remarks>
    /// This endpoint:
    /// - Creates a tenant record in the system database
    /// - Creates a subscription (trial period)
    /// - Provisions a dedicated PostgreSQL schema
    /// - Applies database migrations to the new schema
    /// 
    /// Sample request:
    /// 
    ///     POST /api/v1/tenants
    ///     {
    ///         "name": "Acme Corporation",
    ///         "slug": "acme",
    ///         "email": "admin@acme.com",
    ///         "companyDocument": "12345678000190"
    ///     }
    /// 
    /// </remarks>
    /// <param name="request">Tenant creation data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created tenant with subscription information</returns>
    /// <response code="201">Tenant created successfully</response>
    /// <response code="400">Invalid request data or validation failed</response>
    /// <response code="500">Internal server error during tenant creation</response>
    [HttpPost]
    [ProducesResponseType(typeof(TenantResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateTenant(
        [FromBody] CreateTenantRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            // Validate request
            var validationResult = await validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.ErrorMessage).ToArray()
                    );

                return BadRequest("Validation failed", errors);
            }

            Logger.LogInformation("Creating tenant: {TenantName}", request.Name);

            // Create command and execute
            var command = new CreateTenantCommand
            {
                Name = request.Name,
                Slug = request.Slug.ToLowerInvariant(),
                Email = request.Email,
                CompanyDocument = request.CompanyDocument
            };

            var result = await Mediator.Send(command, cancellationToken);

            Logger.LogInformation("Tenant created successfully: {TenantId}", result.Id);

            return Created(result, "Tenant created successfully. You can now register your first user.");
        }
        catch (Application.Exceptions.ValidationException ex)
        {
            Logger.LogWarning(ex, "Validation failed for tenant creation");
            return BadRequest(ex.Message, ex.Errors);
        }
        catch (Application.Exceptions.BusinessException ex)
        {
            Logger.LogError(ex, "Business error during tenant creation");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Unexpected error during tenant creation");
            return InternalServerError("Failed to create tenant. Please try again later.");
        }
    }

    /// <summary>
    /// Gets tenant information by ID
    /// </summary>
    /// <param name="id">Tenant ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Tenant information</returns>
    /// <response code="200">Tenant found</response>
    /// <response code="404">Tenant not found</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(TenantResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTenantById(
        Guid id,
        CancellationToken cancellationToken)
    {
        // TODO: Implement GetTenantByIdQuery
        return NotFound("Tenant not found");
    }
    
    /// <summary>
    /// Obtém dados do tenant atual
    /// </summary>
    [HttpGet("current")]
    [Authorize]
    [ProducesResponseType(typeof(TenantResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCurrentTenant(CancellationToken cancellationToken)
    {
        var tenantInfo = tenantAccessor.GetTenantInfo();

        var teste = 1;
        var query = new GetTenantByIdQuery { TenantId = tenantInfo.TenantId };
        var result = await Mediator.Send(query, cancellationToken);
    
        return Ok(result);
    }
    
    /// <summary>
    /// Atualiza dados do tenant atual (apenas Admin)
    /// </summary>
    [HttpPut("current")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(TenantResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateCurrentTenant(
        [FromBody] UpdateTenantRequest request,
        CancellationToken cancellationToken)
    {
        var tenantInfo = tenantAccessor.GetTenantInfo();
        if (tenantInfo == null)
            return BadRequest("Tenant não identificado");

        var command = new UpdateTenantCommand
        {
            TenantId = tenantInfo.TenantId,
            Name = request.Name,
            Email = request.Email,
            CompanyDocument = request.CompanyDocument
        };

        try
        {
            var result = await Mediator.Send(command, cancellationToken);
            return Ok(result);
        }
        catch (NotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}