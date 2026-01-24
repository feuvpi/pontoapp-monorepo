using Microsoft.EntityFrameworkCore;
using PontoAPP.Domain.Models.Reports;
using PontoAPP.Domain.Services;
using PontoAPP.Infrastructure.Data.Context;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace PontoAPP.Infrastructure.Services;

public class CRPGenerator(AppDbContext context) : ICRPGenerator
{
    public async Task<byte[]> GeneratePDFAsync(
        Guid timeRecordId,
        CancellationToken cancellationToken = default)
    {
        // Configurar licença QuestPDF
        QuestPDF.Settings.License = LicenseType.Community;

        var model = await BuildModelAsync(timeRecordId, cancellationToken);
        
        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A5.Landscape());
                page.Margin(1, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(9).FontFamily("Arial"));

                page.Content().Element(c => ComposeContent(c, model));
                
                page.Footer().AlignCenter().Text($"Documento gerado em {DateTime.Now:dd/MM/yyyy HH:mm:ss}")
                    .FontSize(7).FontColor(Colors.Grey.Medium);
            });
        }).GeneratePdf();
    }

    private void ComposeContent(IContainer container, CRPModel model)
    {
        container.Column(column =>
        {
            // Header
            column.Item().BorderBottom(2).PaddingBottom(5).Column(header =>
            {
                header.Item().Text("COMPROVANTE DE REGISTRO DE PONTO").FontSize(14).Bold().AlignCenter();
                header.Item().Text("Portaria 671/2021 - MTE").FontSize(8).AlignCenter();
            });

            // Empresa
            column.Item().PaddingTop(10).Background(Colors.Grey.Lighten4).Padding(8).Column(empresa =>
            {
                empresa.Item().Text("EMPRESA").FontSize(7).Bold();
                empresa.Item().Text(model.Empresa.RazaoSocial).FontSize(10);
                empresa.Item().Text($"CNPJ: {FormatCNPJ(model.Empresa.CNPJ)}").FontSize(8);
            });

            // Funcionário
            column.Item().PaddingTop(5).Background(Colors.Grey.Lighten4).Padding(8).Column(func =>
            {
                func.Item().Text("FUNCIONÁRIO").FontSize(7).Bold();
                func.Item().Text(model.Funcionario.Nome).FontSize(10);
                func.Item().Row(row =>
                {
                    row.RelativeItem().Text($"CPF: {FormatCPF(model.Funcionario.CPF)}").FontSize(8);
                    row.RelativeItem().Text($"PIS: {model.Funcionario.PIS}").FontSize(8);
                });
            });

            // Registro
            column.Item().PaddingTop(10).Border(2).BorderColor(Colors.Blue.Medium).Padding(10).Column(registro =>
            {
                registro.Item().AlignCenter().Text("REGISTRO DE PONTO").FontSize(11).Bold();
                
                registro.Item().PaddingTop(10).Row(row =>
                {
                    row.RelativeItem().Column(col =>
                    {
                        col.Item().Text("DATA/HORA").FontSize(7).Bold();
                        col.Item().Text(model.Registro.DataHora.ToString("dd/MM/yyyy HH:mm:ss")).FontSize(12).Bold();
                    });
                    
                    row.RelativeItem().Column(col =>
                    {
                        col.Item().Text("TIPO").FontSize(7).Bold();
                        col.Item().Text(model.Registro.Tipo).FontSize(12).Bold()
                            .FontColor(model.Registro.Tipo == "ENTRADA" ? Colors.Green.Medium : Colors.Red.Medium);
                    });
                });

                registro.Item().PaddingTop(10).Column(col =>
                {
                    col.Item().Text("NSR (Número Sequencial de Registro)").FontSize(7).Bold();
                    col.Item().Text(model.Registro.NSR.ToString()).FontSize(14).Bold();
                });

                registro.Item().PaddingTop(5).Column(col =>
                {
                    col.Item().Text("HASH DE ASSINATURA DIGITAL").FontSize(7).Bold();
                    col.Item().Text(model.Registro.Hash).FontSize(7).FontFamily("Courier New");
                });

                if (!string.IsNullOrEmpty(model.Registro.DispositivoIP))
                {
                    registro.Item().PaddingTop(5).Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text("DISPOSITIVO IP").FontSize(7).Bold();
                            col.Item().Text(model.Registro.DispositivoIP).FontSize(8);
                        });
                        
                        if (!string.IsNullOrEmpty(model.Registro.Localizacao))
                        {
                            row.RelativeItem().Column(col =>
                            {
                                col.Item().Text("LOCALIZAÇÃO").FontSize(7).Bold();
                                col.Item().Text(model.Registro.Localizacao).FontSize(8);
                            });
                        }
                    });
                }
            });

            // Footer com aviso legal
            column.Item().PaddingTop(15).Column(footer =>
            {
                footer.Item().BorderTop(1).PaddingTop(5).Text(
                    "Este comprovante possui validade legal conforme Portaria 671/2021 do MTE. " +
                    "O NSR e Hash garantem a autenticidade e imutabilidade do registro.")
                    .FontSize(7).Italic();
                
                footer.Item().PaddingTop(5).AlignCenter().Text(
                    "Guarde este comprovante para eventuais conferências.")
                    .FontSize(7).Bold();
            });
        });
    }

    private async Task<CRPModel> BuildModelAsync(
        Guid timeRecordId,
        CancellationToken cancellationToken)
    {
        var record = await context.TimeRecords
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == timeRecordId, cancellationToken)
            ?? throw new Exception("Registro de ponto não encontrado");

        // Buscar User e Tenant separadamente
        var user = await context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == record.UserId, cancellationToken)
            ?? throw new Exception("Usuário não encontrado");

        var tenant = await context.Tenants
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == record.TenantId, cancellationToken)
            ?? throw new Exception("Tenant não encontrado");

        return new CRPModel
        {
            Empresa = new CRPModel.EmpresaInfo
            {
                RazaoSocial = tenant.Name,
                CNPJ = tenant.CNPJ.Value
            },
            Funcionario = new CRPModel.FuncionarioInfo
            {
                Nome = user.FullName,
                CPF = user.CPF.Value,
                PIS = user.PIS?.Value ?? "Não informado"
            },
            Registro = new CRPModel.RegistroInfo
            {
                DataHora = record.RecordedAt,
                Tipo = record.Type.ToString().ToUpper(),
                NSR = record.NSR,
                Hash = record.SignatureHash,
                DispositivoIP = "Web App", // TimeRecord não tem campo IPAddress
                Localizacao = "" // TimeRecord não tem campo Location
            }
        };
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