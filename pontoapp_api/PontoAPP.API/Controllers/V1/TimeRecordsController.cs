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
            DeviceId = request.DeviceId,
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
            return Success(result);
            //return Ok(result);
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
    
    
    /// <summary>
/// Lista TODOS os registros do tenant (Admin/RH/Manager)
/// </summary>
[HttpGet]
[Authorize(Roles = "Admin,HR,Manager")]
[ProducesResponseType(typeof(IEnumerable<TimeRecordResponse>), StatusCodes.Status200OK)]
public async Task<IActionResult> GetAllRecords(
    [FromQuery] Guid? userId,
    [FromQuery] DateTime? startDate,
    [FromQuery] DateTime? endDate,
    [FromQuery] string? recordType,
    CancellationToken cancellationToken)
{
    var tenantInfo = tenantAccessor.GetTenantInfo();
    if (tenantInfo == null)
        return BadRequest("Tenant não identificado");

    var query = new GetAllTimeRecordsQuery
    {
        TenantId = tenantInfo.TenantId,
        UserId = userId,
        StartDate = startDate,
        EndDate = endDate,
        RecordType = recordType
    };

    var result = await Mediator.Send(query, cancellationToken);
    return Ok(result);
}

/// <summary>
/// Cria registro manual (Admin/RH/Manager)
/// </summary>
[HttpPost("manual")]
[Authorize(Roles = "Admin,HR,Manager")]
[ProducesResponseType(typeof(TimeRecordResponse), StatusCodes.Status201Created)]
[ProducesResponseType(StatusCodes.Status400BadRequest)]
public async Task<IActionResult> CreateManualRecord(
    [FromBody] CreateManualRecordRequest request,
    CancellationToken cancellationToken)
{
    var tenantInfo = tenantAccessor.GetTenantInfo();
    if (tenantInfo == null)
        return BadRequest("Tenant não identificado");

    var command = new CreateManualRecordCommand
    {
        TenantId = tenantInfo.TenantId,
        UserId = request.UserId,
        RecordType = request.RecordType,
        RecordedAt = request.RecordedAt,
        Latitude = request.Latitude,
        Longitude = request.Longitude,
        Notes = request.Notes
    };

    try
    {
        var result = await Mediator.Send(command, cancellationToken);
        return Created(result, "Registro manual criado com sucesso");
    }
    catch (ValidationException ex)
    {
        return BadRequest(ex.Message);
    }
    catch (NotFoundException ex)
    {
        return NotFound(ex.Message);
    }
}

/// <summary>
/// Atualiza um registro (Admin/RH)
/// </summary>
[HttpPut("{id:guid}")]
[Authorize(Roles = "Admin,HR")]
[ProducesResponseType(typeof(TimeRecordResponse), StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
public async Task<IActionResult> UpdateRecord(
    Guid id,
    [FromBody] UpdateTimeRecordRequest request,
    CancellationToken cancellationToken)
{
    var command = new UpdateTimeRecordCommand
    {
        RecordId = id,
        RecordType = request.RecordType,
        RecordedAt = request.RecordedAt,
        Latitude = request.Latitude,
        Longitude = request.Longitude,
        Notes = request.Notes
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

/// <summary>
/// Deleta um registro (Admin/RH)
/// </summary>
[HttpDelete("{id:guid}")]
[Authorize(Roles = "Admin,HR")]
[ProducesResponseType(StatusCodes.Status204NoContent)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
public async Task<IActionResult> DeleteRecord(
    Guid id,
    CancellationToken cancellationToken)
{
    try
    {
        var command = new DeleteTimeRecordCommand { RecordId = id };
        await Mediator.Send(command, cancellationToken);
        return NoContent();
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