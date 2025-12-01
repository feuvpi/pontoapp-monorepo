using MediatR;
using Microsoft.AspNetCore.Mvc;
using PontoAPP.Application.DTOs.Common;

namespace PontoAPP.API.Controllers;

/// <summary>
/// Base controller with common functionality for all API controllers
/// </summary>
[ApiController]
[Produces("application/json")]
public abstract class BaseController : ControllerBase
{
    protected readonly IMediator Mediator;
    protected readonly ILogger Logger;

    protected BaseController(IMediator mediator, ILogger logger)
    {
        Mediator = mediator;
        Logger = logger;
    }

    /// <summary>
    /// Returns a successful response with data
    /// </summary>
    protected IActionResult Success<T>(T data, string? message = null)
    {
        return Ok(ApiResponse<T>.SuccessResponse(data, message));
    }

    /// <summary>
    /// Returns a created response (201) with data
    /// </summary>
    protected IActionResult Created<T>(T data, string? message = null)
    {
        return StatusCode(201, ApiResponse<T>.SuccessResponse(data, message));
    }

    /// <summary>
    /// Returns a bad request response (400)
    /// </summary>
    protected IActionResult BadRequest(string message, Dictionary<string, string[]>? errors = null)
    {
        return StatusCode(400, ErrorResponse.BadRequest(message, errors));
    }

    /// <summary>
    /// Returns an unauthorized response (401)
    /// </summary>
    protected IActionResult Unauthorized(string message = "Unauthorized")
    {
        return StatusCode(401, ErrorResponse.Unauthorized(message));
    }

    /// <summary>
    /// Returns a forbidden response (403)
    /// </summary>
    protected IActionResult Forbidden(string message = "Forbidden")
    {
        return StatusCode(403, ErrorResponse.Forbidden(message));
    }

    /// <summary>
    /// Returns a not found response (404)
    /// </summary>
    protected IActionResult NotFound(string message)
    {
        return StatusCode(404, ErrorResponse.NotFound(message));
    }

    /// <summary>
    /// Returns an internal server error response (500)
    /// </summary>
    protected IActionResult InternalServerError(string message = "An internal error occurred")
    {
        return StatusCode(500, ErrorResponse.InternalServerError(message));
    }
}