using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PontoAPP.Application.Commands.Devices;
using PontoAPP.Application.DTOs.Device;
using PontoAPP.Application.Exceptions;
using PontoAPP.Application.Queries.Devices;
using PontoAPP.Infrastructure.Multitenancy;

namespace PontoAPP.API.Controllers.V1;

[ApiController]
[Route("api/v1/[controller]")]
[Tags("Devices")]
[Authorize]
public class DevicesController(
    IMediator mediator,
    ILogger<DevicesController> logger,
    ITenantAccessor tenantAccessor)
    : BaseController(mediator, logger)
{
    /// <summary>
    /// Registra um novo dispositivo para o usuário logado
    /// </summary>
    [HttpPost("register")]
    [ProducesResponseType(typeof(DeviceResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RegisterDevice(
        [FromBody] RegisterDeviceRequest request,
        CancellationToken cancellationToken)
    {
        var tenantInfo = tenantAccessor.GetTenantInfo();
        if (tenantInfo == null)
            return BadRequest("Tenant não identificado");

        var userId = GetCurrentUserId();
        if (userId == null)
            return Unauthorized("Usuário não identificado");

        var command = new RegisterDeviceCommand
        {
            TenantId = tenantInfo.TenantId,
            UserId = userId.Value,
            DeviceId = request.DeviceId,
            Platform = request.Platform,
            Model = request.Model,
            OsVersion = request.OsVersion,
            BiometricCapable = request.BiometricCapable,
            PushToken = request.PushToken
        };

        try
        {
            var result = await Mediator.Send(command, cancellationToken);
            return Created(result, "Dispositivo registrado com sucesso");
        }
        catch (BusinessException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Lista dispositivos do usuário logado
    /// </summary>
    [HttpGet("my-devices")]
    [ProducesResponseType(typeof(IEnumerable<DeviceResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyDevices(
        [FromQuery] bool activeOnly = true,
        CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();
        if (userId == null)
            return Unauthorized("Usuário não identificado");

        var query = new GetUserDevicesQuery
        {
            UserId = userId.Value,
            ActiveOnly = activeOnly
        };

        var result = await Mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Lista dispositivos de um usuário específico (para gestores)
    /// </summary>
    [HttpGet("users/{userId:guid}")]
    [ProducesResponseType(typeof(IEnumerable<DeviceResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUserDevices(
        Guid userId,
        [FromQuery] bool activeOnly = true,
        CancellationToken cancellationToken = default)
    {
        var query = new GetUserDevicesQuery
        {
            UserId = userId,
            ActiveOnly = activeOnly
        };

        var result = await Mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Desativa um dispositivo
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeactivateDevice(
        Guid id,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (userId == null)
            return Unauthorized("Usuário não identificado");

        try
        {
            var command = new DeactivateDeviceCommand
            {
                DeviceId = id,
                UserId = userId.Value
            };

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