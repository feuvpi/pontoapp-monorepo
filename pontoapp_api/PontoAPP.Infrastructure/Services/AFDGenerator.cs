using System.Globalization;
using System.Text;
using Microsoft.EntityFrameworkCore;
using PontoAPP.Domain.Services;
using PontoAPP.Infrastructure.Data.Context;

namespace PontoAPP.Infrastructure.Services;

public class AFDGenerator(AppDbContext context) : IAFDGenerator
{
    private long _currentNSR = 1;

    public async Task<string> GenerateAFDAsync(
        Guid tenantId,
        DateTime startDate,
        DateTime endDate,
        Guid? userId = null,
        CancellationToken cancellationToken = default)
    {
        var sb = new StringBuilder();

        // 1. Header (Tipo 0)
        sb.AppendLine(GenerateHeader());

        // 2. Empregador (Tipo 1)
        var tenant = await context.Tenants
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == tenantId, cancellationToken)
            ?? throw new Exception("Tenant não encontrado");

        sb.AppendLine(GenerateEmployer(tenant));

        // 3. Empregados + Marcações (Tipo 2 + Tipo 3)
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

        foreach (var user in users)
        {
            // Tipo 2 - Dados do Empregado
            sb.AppendLine(GenerateEmployee(user));

            // Tipo 3 - Marcações de Ponto
            foreach (var record in user.TimeRecords.OrderBy(r => r.RecordedAt))
            {
                sb.AppendLine(GenerateTimeRecord(record));
            }
        }

        // 4. Trailer (Tipo 9)
        sb.AppendLine(GenerateTrailer());

        return sb.ToString();
    }

    private string GenerateHeader()
    {
        // Tipo 0 - Identificação do Arquivo
        var nsr = FormatNSR(_currentNSR++);
        var tipo = "0";
        var tipoIdentificador = "1"; // 1=CNPJ
        var identificador = "00000000000000"; // Será preenchido com CNPJ do empregador
        var razaoSocial = "SISTEMA_PONTO".PadRight(150);
        var tipoRegistro = "1"; // 1=Admissão
        var dataGeracao = DateTime.Now.ToString("ddMMyyyy");
        var horaGeracao = DateTime.Now.ToString("HHmm");

        return $"{nsr}{tipo}{tipoIdentificador}{identificador}{razaoSocial}{tipoRegistro}{dataGeracao}{horaGeracao}";
    }

    private string GenerateEmployer(Domain.Entities.Tenants.Tenant tenant)
    {
        // Tipo 1 - Dados do Empregador
        var nsr = FormatNSR(_currentNSR++);
        var tipo = "1";
        var tipoIdentificador = "1"; // 1=CNPJ
        var identificador = tenant.CNPJ.Value.PadRight(14);
        var cei = (tenant.CEI ?? "").PadRight(12);
        var razaoSocial = tenant.Name.PadRight(150);
        var local = tenant.Name.PadRight(100);

        return $"{nsr}{tipo}{tipoIdentificador}{identificador}{cei}{razaoSocial}{local}";
    }

    private string GenerateEmployee(Domain.Entities.Identity.User user)
    {
        // Tipo 2 - Dados do Empregado
        var nsr = FormatNSR(_currentNSR++);
        var tipo = "2";
        var tipoIdentificador = "1"; // 1=CPF
        var identificador = user.CPF.Value.PadRight(14);
        var pis = (user.PIS?.Value ?? "").PadRight(11);
        var nome = user.FullName.PadRight(52);
        var dataAdmissao = "01012024"; // TODO: Adicionar campo na entidade User

        return $"{nsr}{tipo}{tipoIdentificador}{identificador}{pis}{nome}{dataAdmissao}";
    }

    private string GenerateTimeRecord(Domain.Entities.TimeTracking.TimeRecord record)
    {
        // Tipo 3 - Marcação de Ponto
        var nsr = FormatNSR(_currentNSR++);
        var tipo = "3";
        var data = record.RecordedAt.ToString("ddMMyyyy");
        var hora = record.RecordedAt.ToString("HHmm");
        var tipoMarcacao = record.Type switch
        {
            Domain.Enums.RecordType.ClockIn => "E", // Entrada
            Domain.Enums.RecordType.ClockOut => "S", // Saída
            _ => "E"
        };

        return $"{nsr}{tipo}{data}{hora}{tipoMarcacao}";
    }

    private string GenerateTrailer()
    {
        // Tipo 9 - Final do Arquivo
        var nsr = FormatNSR(_currentNSR++);
        var tipo = "9";
        var totalRegistros = (_currentNSR - 1).ToString().PadLeft(9, '0');

        return $"{nsr}{tipo}{totalRegistros}";
    }

    private string FormatNSR(long nsr)
    {
        return nsr.ToString().PadLeft(9, '0');
    }
}