using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PontoAPP.Application.Commands.TimeRecords;
using PontoAPP.Application.DTOs.TimeRecords;
using PontoAPP.Application.Exceptions;
using PontoAPP.Application.Queries.TimeRecords;
using PontoAPP.Infrastructure.Multitenancy;

namespace PontoAPP.API.Controllers.V1;

[ApiController]
[Route("api/v1/[controller]")]
[Tags("Time Records")]
[Authorize]
public class TimeRecordsController(
    IMediator mediator,
    ILogger<TimeRecordsController> logger,
    ITenantAccessor tenantAccessor)
    : BaseController(mediator, logger)
{
    /// <summary>
    /// Registra entrada (clock-in)
    /// </summary>
    [HttpPost("clock-in")]
    [ProducesResponseType(typeof(TimeRecordResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ClockIn(
        [FromBody] ClockInRequest request,
        CancellationToken cancellationToken)
    {
        var tenantInfo = tenantAccessor.GetTenantInfo();
        if (tenantInfo == null)
            return BadRequest("Tenant não identificado");

        var userId = GetCurrentUserId();
        if (userId == null)
            return Unauthorized("Usuário não identificado");

        var command = new ClockInCommand
        {
            TenantId = tenantInfo.TenantId,
            UserId = userId.Value,
            AuthenticationType = request.AuthenticationType,
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            Notes = request.Notes
        };

        try
        {
            var result = await Mediator.Send(command, cancellationToken);
            return Created(result, "Entrada registrada com sucesso");
        }
        catch (BusinessException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Registra saída (clock-out)
    /// </summary>
    [HttpPost("clock-out")]
    [ProducesResponseType(typeof(TimeRecordResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ClockOut(
        [FromBody] ClockOutRequest request,
        CancellationToken cancellationToken)
    {
        var tenantInfo = tenantAccessor.GetTenantInfo();
        if (tenantInfo == null)
            return BadRequest("Tenant não identificado");

        var userId = GetCurrentUserId();
        if (userId == null)
            return Unauthorized("Usuário não identificado");

        var command = new ClockOutCommand
        {
            TenantId = tenantInfo.TenantId,
            UserId = userId.Value,
            AuthenticationType = request.AuthenticationType,
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            Notes = request.Notes
        };

        try
        {
            var result = await Mediator.Send(command, cancellationToken);
            return Created(result, "Saída registrada com sucesso");
        }
        catch (BusinessException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Obtém resumo do dia (registros + horas trabalhadas)
    /// </summary>
    [HttpGet("daily-summary")]
    [ProducesResponseType(typeof(DailySummaryResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDailySummary(
        [FromQuery] DateTime? date,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (userId == null)
            return Unauthorized("Usuário não identificado");

        var query = new GetDailySummaryQuery
        {
            UserId = userId.Value,
            Date = date
        };

        try
        {
            var result = await Mediator.Send(query, cancellationToken);
            return Ok(result);
        }
        catch (NotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    /// <summary>
    /// Obtém resumo do dia de um usuário específico (para gestores)
    /// </summary>
    [HttpGet("users/{userId:guid}/daily-summary")]
    [ProducesResponseType(typeof(DailySummaryResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUserDailySummary(
        Guid userId,
        [FromQuery] DateTime? date,
        CancellationToken cancellationToken)
    {
        var query = new GetDailySummaryQuery
        {
            UserId = userId,
            Date = date
        };

        try
        {
            var result = await Mediator.Send(query, cancellationToken);
            return Ok(result);
        }
        catch (NotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    /// <summary>
    /// Lista registros do usuário logado
    /// </summary>
    [HttpGet("my-records")]
    [ProducesResponseType(typeof(IEnumerable<TimeRecordResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyRecords(
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (userId == null)
            return Unauthorized("Usuário não identificado");

        var query = new GetUserRecordsQuery
        {
            UserId = userId.Value,
            StartDate = startDate,
            EndDate = endDate
        };

        try
        {
            var result = await Mediator.Send(query, cancellationToken);
            return Ok(result);
        }
        catch (NotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    /// <summary>
    /// Lista registros de um usuário específico (para gestores)
    /// </summary>
    [HttpGet("users/{userId:guid}/records")]
    [ProducesResponseType(typeof(IEnumerable<TimeRecordResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUserRecords(
        Guid userId,
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate,
        CancellationToken cancellationToken)
    {
        var query = new GetUserRecordsQuery
        {
            UserId = userId,
            StartDate = startDate,
            EndDate = endDate
        };

        try
        {
            var result = await Mediator.Send(query, cancellationToken);
            return Ok(result);
        }
        catch (NotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    private Guid? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                          ?? User.FindFirst("sub")?.Value
                          ?? User.FindFirst("user_id")?.Value;

        if (Guid.TryParse(userIdClaim, out var userId))
            return userId;

        return null;
    }
}