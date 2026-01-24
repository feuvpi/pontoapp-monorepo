// PontoAPP.Infrastructure/Services/Reports/ACJEFGenerator.cs

using Microsoft.EntityFrameworkCore;
using PontoAPP.Domain.Models.Reports;
using PontoAPP.Domain.Services;
using PontoAPP.Infrastructure.Data.Context;

namespace PontoAPP.Infrastructure.Services;

public class ACJEFGenerator(AppDbContext context) : IACJEFGenerator
{
    public async Task<ACJEFModel> GenerateACJEFAsync(
        Guid tenantId,
        int year,
        int month,
        Guid? userId = null,
        CancellationToken cancellationToken = default)
    {
        var startDate = new DateTime(year, month, 1);
        var endDate = startDate.AddMonths(1).AddDays(-1);

        var tenant = await context.Tenants
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == tenantId, cancellationToken)
            ?? throw new Exception("Tenant não encontrado");

        var query = context.Users
            .AsNoTracking()
            .Where(u => u.TenantId == tenantId && u.IsActive);

        if (userId.HasValue)
            query = query.Where(u => u.Id == userId.Value);

        var users = await query
            .Include(u => u.TimeRecords.Where(tr => 
                tr.RecordedAt >= startDate && 
                tr.RecordedAt <= endDate))
            .OrderBy(u => u.CPF.Value)
            .ToListAsync(cancellationToken);

        var response = new ACJEFModel
        {
            Empregador = new EmployerInfo
            {
                CNPJ = tenant.CNPJ.Value,
                RazaoSocial = tenant.Name
            },
            Competencia = $"{year:D4}-{month:D2}",
            Empregados = new List<EmployeeInfo>()
        };

        foreach (var user in users)
        {
            var employeeData = new EmployeeInfo
            {
                CPF = user.CPF.Value,
                Nome = user.FullName,
                PIS = user.PIS?.Value ?? string.Empty,
                Registros = new List<DailyRecordInfo>()
            };

            var recordsByDay = user.TimeRecords
                .OrderBy(r => r.RecordedAt)
                .GroupBy(r => r.RecordedAt.Date);

            foreach (var dayGroup in recordsByDay)
            {
                var marcacoes = dayGroup
                    .OrderBy(r => r.RecordedAt)
                    .Select(r => new TimeStampInfo
                    {
                        Hora = r.RecordedAt.ToString("HH:mm"),
                        Tipo = r.Type == Domain.Enums.RecordType.ClockIn ? "entrada" : "saida",
                        NSR = r.NSR.ToString()
                    })
                    .ToList();

                var jornada = CalculateWorkSummary(marcacoes);

                employeeData.Registros.Add(new DailyRecordInfo
                {
                    Data = dayGroup.Key.ToString("yyyy-MM-dd"),
                    Marcacoes = marcacoes,
                    Jornada = jornada
                });
            }

            response.Empregados.Add(employeeData);
        }

        return response;
    }

    private static WorkSummaryInfo CalculateWorkSummary(List<TimeStampInfo> marcacoes)
    {
        var totalMinutes = 0;

        for (int i = 0; i < marcacoes.Count - 1; i += 2)
        {
            if (i + 1 >= marcacoes.Count) break;

            var entrada = TimeSpan.Parse(marcacoes[i].Hora);
            var saida = TimeSpan.Parse(marcacoes[i + 1].Hora);
            
            var worked = saida - entrada;
            totalMinutes += (int)worked.TotalMinutes;
        }

        var hours = totalMinutes / 60;
        var minutes = totalMinutes % 60;

        return new WorkSummaryInfo
        {
            HorasTrabalhadas = $"{hours:D2}:{minutes:D2}",
            HorasExtras = "00:00",
            HorasNoturnas = "00:00"
        };
    }
}