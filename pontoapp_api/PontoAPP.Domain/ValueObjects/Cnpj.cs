namespace PontoAPP.Domain.ValueObjects;

/// <summary>
/// Value Object para CNPJ com validação
/// </summary>
public sealed class Cnpj
{
    public string Value { get; private set; }

    private Cnpj(string value)
    {
        Value = value;
    }

    public static Cnpj Create(string cnpj)
    {
        if (string.IsNullOrWhiteSpace(cnpj))
            throw new ArgumentException("CNPJ cannot be empty", nameof(cnpj));

        // Remove caracteres não numéricos
        var cleanCnpj = new string(cnpj.Where(char.IsDigit).ToArray());

        if (cleanCnpj.Length != 14)
            throw new ArgumentException("CNPJ must have 14 digits", nameof(cnpj));

        if (!IsValid(cleanCnpj))
            throw new ArgumentException("Invalid CNPJ", nameof(cnpj));

        return new Cnpj(cleanCnpj);
    }

    private static bool IsValid(string cnpj)
    {
        // Verifica CNPJs inválidos conhecidos
        if (cnpj.All(c => c == cnpj[0]))
            return false;

        // Calcula primeiro dígito verificador
        int[] multiplier1 = { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
        var sum = 0;

        for (int i = 0; i < 12; i++)
            sum += int.Parse(cnpj[i].ToString()) * multiplier1[i];

        var remainder = sum % 11;
        var digit1 = remainder < 2 ? 0 : 11 - remainder;

        if (int.Parse(cnpj[12].ToString()) != digit1)
            return false;

        // Calcula segundo dígito verificador
        int[] multiplier2 = { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
        sum = 0;

        for (int i = 0; i < 13; i++)
            sum += int.Parse(cnpj[i].ToString()) * multiplier2[i];

        remainder = sum % 11;
        var digit2 = remainder < 2 ? 0 : 11 - remainder;

        return int.Parse(cnpj[13].ToString()) == digit2;
    }

    /// <summary>
    /// Retorna CNPJ formatado: 00.000.000/0000-00
    /// </summary>
    public string ToFormattedString()
    {
        return $"{Value.Substring(0, 2)}.{Value.Substring(2, 3)}.{Value.Substring(5, 3)}/{Value.Substring(8, 4)}-{Value.Substring(12, 2)}";
    }

    public override string ToString() => Value;

    public override bool Equals(object? obj)
    {
        if (obj is not Cnpj other)
            return false;

        return Value == other.Value;
    }

    public override int GetHashCode() => Value.GetHashCode();

    public static implicit operator string(Cnpj cnpj) => cnpj.Value;
}