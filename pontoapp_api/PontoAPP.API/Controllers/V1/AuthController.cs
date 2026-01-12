using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PontoAPP.Application.Commands.Auth;
using PontoAPP.Application.DTOs.Auth;
using PontoAPP.Application.Exceptions;

namespace PontoAPP.API.Controllers.V1;

[ApiController]
[Route("api/v1/[controller]")]
[Tags("Auth")]
public class AuthController(IMediator mediator, ILogger<AuthController> logger) 
    : BaseController(mediator, logger)
{
    /// <summary>
    /// Registra uma nova empresa + usuário admin
    /// </summary>
    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register(
        [FromBody] RegisterTenantRequest request,
        CancellationToken cancellationToken)
    {
        var command = new RegisterTenantCommand
        {
            CompanyName = request.CompanyName,
            CompanyDocument = request.CompanyDocument,
            AdminName = request.AdminName,
            AdminEmail = request.AdminEmail,
            Password = request.Password
        };

        var result = await Mediator.Send(command, cancellationToken);
        return Created(result, "Tenant created successfully");
    }

    /// <summary>
    /// Realiza login
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken)
    {
        var command = new LoginCommand
        {
            Email = request.Email,
            Password = request.Password
        };

        var result = await Mediator.Send(command, cancellationToken);
        return Success(result);
    }

    /// <summary>
    /// Renova o token de acesso
    /// </summary>
    [HttpPost("refresh")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RefreshToken(
        [FromBody] RefreshTokenRequest request,
        CancellationToken cancellationToken)
    {
        var command = new RefreshTokenCommand
        {
            RefreshToken = request.RefreshToken
        };

        var result = await Mediator.Send(command, cancellationToken);
        return Success(result);
    }
    
    /// <summary>
    /// Troca a senha do usuário logado
    /// </summary>
    [HttpPost("change-password")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ChangePassword(
        [FromBody] ChangePasswordRequest request,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (userId == null)
            return Unauthorized("Usuário não identificado");

        var command = new ChangePasswordCommand
        {
            UserId = userId.Value,
            CurrentPassword = request.CurrentPassword,
            NewPassword = request.NewPassword
        };

        try
        {
            await Mediator.Send(command, cancellationToken);
            return Success(new { message = "Senha alterada com sucesso" });
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
}