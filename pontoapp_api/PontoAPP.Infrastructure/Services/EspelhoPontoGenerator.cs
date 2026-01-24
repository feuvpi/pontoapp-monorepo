using System.Globalization;
using Microsoft.EntityFrameworkCore;
using PontoAPP.Domain.Models.Reports;
using PontoAPP.Domain.Services;
using PontoAPP.Infrastructure.Data.Context;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace PontoAPP.Infrastructure.Services;

public class EspelhoPontoGenerator(AppDbContext context) : IEspelhoPontoGenerator
{
    public async Task<byte[]> GeneratePDFAsync(
        Guid tenantId,
        Guid userId,
        int year,
        int month,
        CancellationToken cancellationToken = default)
    {
        // Configurar licença QuestPDF
        QuestPDF.Settings.License = LicenseType.Community;

        var model = await BuildModelAsync(tenantId, userId, year, month, cancellationToken);
        
        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Arial"));

                page.Header().Element(ComposeHeader);
                page.Content().Element(c => ComposeContent(c, model));
                page.Footer().AlignCenter().Text(text =>
                {
                    text.Span("Página ");
                    text.CurrentPageNumber();
                    text.Span(" de ");
                    text.TotalPages();
                    text.Span($" | Gerado em {DateTime.Now:dd/MM/yyyy HH:mm}");
                });
            });
        }).GeneratePdf();

        void ComposeHeader(IContainer container)
        {
            container.Column(column =>
            {
                column.Item().BorderBottom(1).PaddingBottom(10).Column(header =>
                {
                    header.Item().Text("ESPELHO DE PONTO").FontSize(16).Bold();
                    header.Item().Text($"Competência: {month:00}/{year}").FontSize(12);
                });
                
                column.Item().PaddingTop(10).Row(row =>
                {
                    row.RelativeItem().Column(col =>
                    {
                        col.Item().Text("EMPRESA").Bold();
                        col.Item().Text(model.Empregador.RazaoSocial);
                        col.Item().Text($"CNPJ: {FormatCNPJ(model.Empregador.CNPJ)}");
                    });
                    
                    row.RelativeItem().Column(col =>
                    {
                        col.Item().Text("FUNCIONÁRIO").Bold();
                        col.Item().Text(model.Funcionario.Nome);
                        col.Item().Text($"CPF: {FormatCPF(model.Funcionario.CPF)}");
                        col.Item().Text($"PIS: {model.Funcionario.PIS}");
                    });
                });
            });
        }
    }

    private void ComposeContent(IContainer container, EspelhoModel model)
    {
        container.PaddingTop(20).Column(column =>
        {
            // Tabela de Marcações
            column.Item().Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(70);  // Data
                    columns.ConstantColumn(60);  // Dia Semana
                    columns.RelativeColumn(3);   // Marcações
                    columns.ConstantColumn(60);  // Total
                    columns.ConstantColumn(60);  // Extras
                });

                // Header
                table.Header(header =>
                {
                    header.Cell().Element(CellStyle).Text("Data").Bold();
                    header.Cell().Element(CellStyle).Text("Dia").Bold();
                    header.Cell().Element(CellStyle).Text("Marcações (NSR)").Bold();
                    header.Cell().Element(CellStyle).Text("Total").Bold();
                    header.Cell().Element(CellStyle).Text("Extras").Bold();
                    
                    static IContainer CellStyle(IContainer c) => c
                        .Background(Colors.Grey.Lighten3)
                        .Padding(5)
                        .BorderBottom(1);
                });

                // Rows
                foreach (var dia in model.Registros.OrderBy(r => r.Data))
                {
                    table.Cell().Element(CellStyle).Text(dia.Data.ToString("dd/MM/yyyy"));
                    table.Cell().Element(CellStyle).Text(dia.DiaSemana);
                    table.Cell().Element(CellStyle).Column(col =>
                    {
                        foreach (var marcacao in dia.Marcacoes)
                        {
                            col.Item().Text($"{marcacao.Hora} {marcacao.Tipo} ({marcacao.NSR})").FontSize(9);
                        }
                    });
                    table.Cell().Element(CellStyle).Text(dia.TotalHoras);
                    table.Cell().Element(CellStyle).Text(dia.HorasExtras);
                    
                    static IContainer CellStyle(IContainer c) => c
                        .Padding(5)
                        .BorderBottom(1)
                        .BorderColor(Colors.Grey.Lighten2);
                }
            });

            // Resumo Mensal
            column.Item().PaddingTop(20).BorderTop(2).PaddingTop(10).Column(resumo =>
            {
                resumo.Item().Text("RESUMO MENSAL").FontSize(12).Bold();
                resumo.Item().PaddingTop(5).Row(row =>
                {
                    row.RelativeItem().Text($"Dias Trabalhados: {model.Resumo.DiasTrabalhados}/{model.Resumo.DiasTrabalhadosPrevistos}");
                    row.RelativeItem().Text($"Faltas: {model.Resumo.Faltas}");
                });
                resumo.Item().Row(row =>
                {
                    row.RelativeItem().Text($"Total Horas: {model.Resumo.TotalHorasTrabalhadas}");
                    row.RelativeItem().Text($"Horas Extras: {model.Resumo.TotalHorasExtras}");
                });
            });

            // Assinatura
            column.Item().PaddingTop(40).Column(assinatura =>
            {
                assinatura.Item().BorderTop(1).Width(200).AlignCenter();
                assinatura.Item().AlignCenter().Text("Assinatura do Funcionário");
            });
        });
    }

    private async Task<EspelhoModel> BuildModelAsync(
        Guid tenantId,
        Guid userId,
        int year,
        int month,
        CancellationToken cancellationToken)
    {
        var startDate = new DateTime(year, month, 1);
        var endDate = startDate.AddMonths(1).AddDays(-1);

        var tenant = await context.Tenants
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == tenantId, cancellationToken)
            ?? throw new Exception("Tenant não encontrado");

        var user = await context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId && u.TenantId == tenantId, cancellationToken)
            ?? throw new Exception("Usuário não encontrado");

        // Buscar TimeRecords separadamente
        var timeRecords = await context.TimeRecords
            .AsNoTracking()
            .Where(tr => tr.UserId == userId && 
                         tr.TenantId == tenantId &&
                         tr.RecordedAt >= startDate &&
                         tr.RecordedAt <= endDate)
            .OrderBy(tr => tr.RecordedAt)
            .ToListAsync(cancellationToken);

        var model = new EspelhoModel
        {
            Ano = year,
            Mes = month,
            Empregador = new EspelhoModel.EmpregadorInfo
            {
                RazaoSocial = tenant.Name,
                CNPJ = tenant.CNPJ.Value,
                Endereco = tenant.Name
            },
            Funcionario = new EspelhoModel.FuncionarioInfo
            {
                Nome = user.FullName,
                CPF = user.CPF.Value,
                PIS = user.PIS?.Value ?? "Não informado",
                Cargo = "Não informado",
                DataAdmissao = user.CreatedAt
            }
        };

        // Agrupar registros por dia
        var recordsByDay = timeRecords
            .GroupBy(r => r.RecordedAt.Date)
            .OrderBy(g => g.Key);

        foreach (var dayGroup in recordsByDay)
        {
            var marcacoes = dayGroup
                .OrderBy(r => r.RecordedAt)
                .Select(r => new EspelhoModel.Marcacao
                {
                    Hora = r.RecordedAt.ToString("HH:mm"),
                    Tipo = r.Type.ToString().ToUpper(),
                    NSR = r.NSR,
                    Origem = "WEB"
                })
                .ToList();

            var totalMinutes = CalculateTotalMinutes(marcacoes);

            model.Registros.Add(new EspelhoModel.DiaRegistro
            {
                Data = dayGroup.Key,
                DiaSemana = GetDiaSemana(dayGroup.Key),
                Marcacoes = marcacoes,
                TotalHoras = FormatMinutesToHours(totalMinutes),
                HorasExtras = "00:00"
            });
        }

        // Calcular resumo
        var diasUteis = GetDiasUteis(year, month);
        model.Resumo = new EspelhoModel.ResumoMensal
        {
            DiasTrabalhadosPrevistos = diasUteis,
            DiasTrabalhados = model.Registros.Count,
            Faltas = diasUteis - model.Registros.Count,
            TotalHorasTrabalhadas = FormatMinutesToHours(
                model.Registros.Sum(r => CalculateTotalMinutes(r.Marcacoes))),
            TotalHorasExtras = "00:00",
            TotalHorasNoturnas = "00:00"
        };

        return model;
    }

    private int CalculateTotalMinutes(List<EspelhoModel.Marcacao> marcacoes)
    {
        var totalMinutes = 0;
        for (int i = 0; i < marcacoes.Count - 1; i += 2)
        {
            if (i + 1 < marcacoes.Count)
            {
                var entrada = TimeSpan.Parse(marcacoes[i].Hora);
                var saida = TimeSpan.Parse(marcacoes[i + 1].Hora);
                totalMinutes += (int)(saida - entrada).TotalMinutes;
            }
        }
        return totalMinutes;
    }

    private string FormatMinutesToHours(int minutes)
    {
        var hours = minutes / 60;
        var mins = minutes % 60;
        return $"{hours:00}:{mins:00}";
    }

    private string GetDiaSemana(DateTime date)
    {
        return date.ToString("ddd", new CultureInfo("pt-BR")).ToUpper();
    }

    private int GetDiasUteis(int year, int month)
    {
        var startDate = new DateTime(year, month, 1);
        var endDate = startDate.AddMonths(1).AddDays(-1);
        var count = 0;

        for (var date = startDate; date <= endDate; date = date.AddDays(1))
        {
            if (date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday)
                count++;
        }

        return count;
    }

    private string FormatCPF(string cpf)
    {
        if (cpf.Length != 11) return cpf;
        return $"{cpf.Substring(0, 3)}.{cpf.Substring(3, 3)}.{cpf.Substring(6, 3)}-{cpf.Substring(9, 2)}";
    }

    private string FormatCNPJ(string cnpj)
    {
        if (cnpj.Length != 14) return cnpj;
        return $"{cnpj.Substring(0, 2)}.{cnpj.Substring(2, 3)}.{cnpj.Substring(5, 3)}/{cnpj.Substring(8, 4)}-{cnpj.Substring(12, 2)}";
    }
}