using MediatR;
using Microsoft.AspNetCore.Mvc;
using PontoAPP.Application.Commands.Tenants;
using PontoAPP.Application.DTOs.Tenants;
using PontoAPP.Application.Validators.Tenants;

namespace PontoAPP.API.Controllers.V1;

/// <summary>
/// Tenant management endpoints
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Tags("Tenants")]
public class TenantsController : BaseController
{
    private readonly CreateTenantRequestValidator _validator;

    public TenantsController(
        IMediator mediator,
        ILogger<TenantsController> logger,
        CreateTenantRequestValidator validator) : base(mediator, logger)
    {
        _validator = validator;
    }

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
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
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
}