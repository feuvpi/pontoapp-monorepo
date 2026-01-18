using System.Text.RegularExpressions;

namespace PontoAPP.Domain.ValueObjects;

/// <summary>
/// Value Object para CPF com validação
/// Armazena apenas os 11 dígitos (sem formatação)
/// </summary>
public sealed class CPF
{
    private static readonly Regex CPFRegex = new(
        @"^\d{11}$",
        RegexOptions.Compiled);

    public string Value { get; private set; }

    private CPF(string value)
    {
        Value = value;
    }

    /// <summary>
    /// Cria um CPF validado
    /// Aceita formato com ou sem pontuação: "12345678901" ou "123.456.789-01"
    /// </summary>
    public static CPF Create(string cpf)
    {
        if (string.IsNullOrWhiteSpace(cpf))
            throw new ArgumentException("CPF cannot be empty", nameof(cpf));

        // Remove caracteres não numéricos (pontos, traços)
        var cleanCPF = new string(cpf.Where(char.IsDigit).ToArray());

        if (!CPFRegex.IsMatch(cleanCPF))
            throw new ArgumentException("CPF must have 11 digits", nameof(cpf));

        if (!IsValid(cleanCPF))
            throw new ArgumentException("Invalid CPF", nameof(cpf));

        return new CPF(cleanCPF);
    }

    private static bool IsValid(string cpf)
    {
        // Verifica CPFs inválidos conhecidos (todos dígitos iguais)
        if (cpf.All(c => c == cpf[0]))
            return false;

        // Calcula primeiro dígito verificador
        var sum = 0;
        for (int i = 0; i < 9; i++)
            sum += int.Parse(cpf[i].ToString()) * (10 - i);

        var remainder = sum % 11;
        var digit1 = remainder < 2 ? 0 : 11 - remainder;

        if (int.Parse(cpf[9].ToString()) != digit1)
            return false;

        // Calcula segundo dígito verificador
        sum = 0;
        for (int i = 0; i < 10; i++)
            sum += int.Parse(cpf[i].ToString()) * (11 - i);

        remainder = sum % 11;
        var digit2 = remainder < 2 ? 0 : 11 - remainder;

        return int.Parse(cpf[10].ToString()) == digit2;
    }

    /// <summary>
    /// Retorna CPF formatado: 000.000.000-00
    /// </summary>
    public string ToFormattedString()
    {
        return $"{Value.Substring(0, 3)}.{Value.Substring(3, 3)}.{Value.Substring(6, 3)}-{Value.Substring(9, 2)}";
    }

    public override string ToString() => Value;

    public override bool Equals(object? obj)
    {
        if (obj is not CPF other)
            return false;

        return Value == other.Value;
    }

    public override int GetHashCode() => Value.GetHashCode();

    public static implicit operator string(CPF cpf) => cpf.Value;
}