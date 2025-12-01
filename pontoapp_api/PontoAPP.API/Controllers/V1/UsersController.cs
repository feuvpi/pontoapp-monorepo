using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PontoAPP.Application.Commands.Users;
using PontoAPP.Application.DTOs.Users;
using PontoAPP.Application.Exceptions;
using PontoAPP.Application.Queries.Users;
using PontoAPP.Application.Validators.Users;
using PontoAPP.Infrastructure.Multitenancy;

namespace PontoAPP.API.Controllers.V1;

[ApiController]
[Route("api/v1/[controller]")]
[Tags("Users")]
[Authorize]
public class UsersController(
    IMediator mediator,
    ILogger<UsersController> logger,
    ITenantAccessor tenantAccessor,
    CreateUserRequestValidator createValidator)
    : BaseController(mediator, logger)
{
    /// <summary>
    /// Lista todos os usuários do tenant
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<UserResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUsers(
        [FromQuery] bool? activeOnly,
        [FromQuery] string? role,
        CancellationToken cancellationToken)
    {
        var query = new GetUsersQuery
        {
            ActiveOnly = activeOnly,
            Role = role
        };

        var users = await Mediator.Send(query, cancellationToken);
        return Ok(users);
    }

    /// <summary>
    /// Obtém um usuário por ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUserById(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetUserByIdQuery { UserId = id };
        var user = await Mediator.Send(query, cancellationToken);

        if (user == null)
            return NotFound("Usuário não encontrado");

        return Ok(user);
    }

    /// <summary>
    /// Cria um novo usuário no tenant
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateUser(
        [FromBody] CreateUserRequest request,
        CancellationToken cancellationToken)
    {
        var validationResult = await createValidator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
            return BadRequest("Validation failed", errors);
        }

        var tenantInfo = tenantAccessor.GetTenantInfo();
        if (tenantInfo == null)
            return BadRequest("Tenant não identificado");

        var command = new CreateUserCommand
        {
            TenantId = tenantInfo.TenantId,
            FullName = request.FullName,
            Email = request.Email,
            Password = request.Password,
            Role = request.Role,
            EmployeeCode = request.EmployeeCode,
            Department = request.Department,
            HiredAt = request.HiredAt
        };

        try
        {
            var result = await Mediator.Send(command, cancellationToken);
            return Created(result, "Usuário criado com sucesso");
        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Atualiza um usuário
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateUser(
        Guid id,
        [FromBody] UpdateUserRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateUserCommand
        {
            UserId = id,
            FullName = request.FullName,
            Email = request.Email,
            Role = request.Role,
            EmployeeCode = request.EmployeeCode,
            Department = request.Department,
            HiredAt = request.HiredAt
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
    /// Desativa um usuário (soft delete)
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeactivateUser(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var command = new DeactivateUserCommand { UserId = id };
            await Mediator.Send(command, cancellationToken);
            return NoContent();
        }
        catch (NotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    /// <summary>
    /// Reativa um usuário
    /// </summary>
    [HttpPost("{id:guid}/activate")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ActivateUser(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var command = new ActivateUserCommand { UserId = id };
            await Mediator.Send(command, cancellationToken);
            return Ok(new { message = "Usuário ativado com sucesso" });
        }
        catch (NotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    /// <summary>
    /// Reseta a senha de um usuário (Admin only)
    /// </summary>
    [HttpPost("{id:guid}/reset-password")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ResetPassword(
        Guid id,
        [FromBody] ResetPasswordRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.NewPassword) || request.NewPassword.Length < 6)
        {
            return BadRequest("Senha deve ter pelo menos 6 caracteres");
        }

        try
        {
            var command = new ResetPasswordCommand
            {
                UserId = id,
                NewPassword = request.NewPassword
            };
            await Mediator.Send(command, cancellationToken);
            return Ok(new { message = "Senha alterada com sucesso" });
        }
        catch (NotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }
}