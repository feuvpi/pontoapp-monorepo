namespace PontoAPP.Domain.ValueObjects;

/// <summary>
/// Value Object para PIS/PASEP com validação
/// </summary>
public sealed class Pis
{
    public string Value { get; }

    private Pis(string value)
    {
        Value = value;
    }

    public static Pis Create(string pis)
    {
        if (string.IsNullOrWhiteSpace(pis))
            throw new ArgumentException("PIS cannot be empty", nameof(pis));

        // Remove caracteres não numéricos
        var cleanPis = new string(pis.Where(char.IsDigit).ToArray());

        if (cleanPis.Length != 11)
            throw new ArgumentException("PIS must have 11 digits", nameof(pis));

        if (!IsValid(cleanPis))
            throw new ArgumentException("Invalid PIS", nameof(pis));

        return new Pis(cleanPis);
    }

    private static bool IsValid(string pis)
    {
        // Verifica PIS inválidos conhecidos
        if (pis.All(c => c == pis[0]))
            return false;

        // Multiplicadores para cálculo do dígito verificador
        int[] multipliers = { 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };

        var sum = 0;
        for (int i = 0; i < 10; i++)
            sum += int.Parse(pis[i].ToString()) * multipliers[i];

        var remainder = sum % 11;
        var digit = remainder < 2 ? 0 : 11 - remainder;

        return int.Parse(pis[10].ToString()) == digit;
    }

    /// <summary>
    /// Retorna PIS formatado: 000.00000.00-0
    /// </summary>
    public string ToFormattedString()
    {
        return $"{Value.Substring(0, 3)}.{Value.Substring(3, 5)}.{Value.Substring(8, 2)}-{Value.Substring(10, 1)}";
    }

    public override string ToString() => Value;

    public override bool Equals(object? obj)
    {
        if (obj is not Pis other)
            return false;

        return Value == other.Value;
    }

    public override int GetHashCode() => Value.GetHashCode();

    public static implicit operator string(Pis pis) => pis.Value;
}