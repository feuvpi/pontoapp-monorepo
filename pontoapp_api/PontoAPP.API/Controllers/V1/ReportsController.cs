using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PontoAPP.Application.DTOs.Reports;
using PontoAPP.Application.Queries.Reports;
using System.Text;
using PontoAPP.Domain.Models.Reports;
using PontoAPP.Infrastructure.Multitenancy;

namespace PontoAPP.API.Controllers.V1;

[Authorize]
[ApiController]
[Route("api/v1/[controller]")]
public class ReportsController(
    IMediator mediator,
    ILogger<ReportsController> logger,
    ITenantAccessor tenantAccessor)
    : BaseController(mediator, logger)
{
    /// <summary>
    /// Gera AFD (Arquivo Fonte de Dados) - Portaria 671 Layout 9
    /// </summary>
    [HttpGet("afd")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GenerateAFD(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate,
        [FromQuery] Guid? userId = null)
    {
        if (startDate > endDate)
            return BadRequest("Data inicial não pode ser maior que data final");

        if ((endDate - startDate).TotalDays > 366)
            return BadRequest("Período máximo é de 1 ano");

        var query = new GenerateAFDQuery(startDate, endDate, userId);
        var afdContent = await Mediator.Send(query);

        var fileName = userId.HasValue
            ? $"AFD_{userId}_{startDate:yyyyMMdd}_{endDate:yyyyMMdd}.txt"
            : $"AFD_{startDate:yyyyMMdd}_{endDate:yyyyMMdd}.txt";

        var bytes = Encoding.UTF8.GetBytes(afdContent);

        return File(bytes, "text/plain", fileName);
    }
    
    [HttpGet("acjef")]
    [ProducesResponseType(typeof(ACJEFModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GenerateACJEF(
        [FromQuery] int year,
        [FromQuery] int month,
        [FromQuery] Guid? userId = null)
    {
        if (month < 1 || month > 12)
            return BadRequest("Mês inválido");

        if (year < 2000 || year > DateTime.Now.Year)
            return BadRequest("Ano inválido");

        var query = new GenerateACJEFQuery(year, month, userId);
        var result = await Mediator.Send(query);

        return Ok(result);
    }
    
    /// <summary>
    /// Gera Espelho de Ponto mensal (PDF)
    /// </summary>
    [HttpGet("espelho")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GenerateEspelho(
        [FromQuery] Guid userId,
        [FromQuery] int year,
        [FromQuery] int month)
    {
        if (month < 1 || month > 12)
            return BadRequest("Mês inválido");

        if (year < 2000 || year > DateTime.Now.Year + 1)
            return BadRequest("Ano inválido");

        var query = new GenerateEspelhoQuery(userId, year, month);
        var pdfBytes = await Mediator.Send(query);

        var fileName = $"Espelho_Ponto_{userId}_{year}{month:00}.pdf";

        return File(pdfBytes, "application/pdf", fileName);
    }

    /// <summary>
    /// Gera CRP (Comprovante de Registro de Ponto) individual (PDF)
    /// </summary>
    [HttpGet("crp/{timeRecordId:guid}")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GenerateCRP(Guid timeRecordId)
    {
        var query = new GenerateCRPQuery(timeRecordId);
        var pdfBytes = await Mediator.Send(query);

        var fileName = $"CRP_{timeRecordId}.pdf";

        return File(pdfBytes, "application/pdf", fileName);
    }
}