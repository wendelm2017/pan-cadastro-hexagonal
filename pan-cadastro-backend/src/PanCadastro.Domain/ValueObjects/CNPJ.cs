using PanCadastro.Domain.Exceptions;

namespace PanCadastro.Domain.ValueObjects;

// Value Object para CNPJ com validação completa de dígitos verificadores.
// Ela é sealed para para atender o mesmo propósito da CEP
public sealed class CNPJ : IEquatable<CNPJ>
{
    public string Numero { get; }

    private CNPJ(string numero)
    {
        Numero = numero;
    }

    public static CNPJ Criar(string numero)
    {
        var apenasDigitos = new string(numero?.Where(char.IsDigit).ToArray() ?? Array.Empty<char>());

        if (!Validar(apenasDigitos))
            throw new DomainException($"CNPJ inválido: {numero}");

        return new CNPJ(apenasDigitos);
    }

    public static bool Validar(string cnpj)
    {
        if (string.IsNullOrWhiteSpace(cnpj))
            return false;

        var apenasDigitos = new string(cnpj.Where(char.IsDigit).ToArray());

        if (apenasDigitos.Length != 14)
            return false;

        if (apenasDigitos.Distinct().Count() == 1)
            return false;

        // Primeiro dígito verificador
        int[] multiplicadores1 = { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
        var soma = 0;
        for (int i = 0; i < 12; i++)
            soma += (apenasDigitos[i] - '0') * multiplicadores1[i];

        var resto = soma % 11;
        var primeiroDigito = resto < 2 ? 0 : 11 - resto;

        if (apenasDigitos[12] - '0' != primeiroDigito)
            return false;

        // Segundo dígito verificador
        int[] multiplicadores2 = { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
        soma = 0;
        for (int i = 0; i < 13; i++)
            soma += (apenasDigitos[i] - '0') * multiplicadores2[i];

        resto = soma % 11;
        var segundoDigito = resto < 2 ? 0 : 11 - resto;

        return apenasDigitos[13] - '0' == segundoDigito;
    }

    public string Formatado => Convert.ToUInt64(Numero).ToString(@"00\.000\.000\/0000\-00");

    public bool Equals(CNPJ? other) => other is not null && Numero == other.Numero;
    public override bool Equals(object? obj) => obj is CNPJ other && Equals(other);
    public override int GetHashCode() => Numero.GetHashCode();
    public override string ToString() => Formatado;

    public static bool operator ==(CNPJ? left, CNPJ? right) => Equals(left, right);
    public static bool operator !=(CNPJ? left, CNPJ? right) => !Equals(left, right);
}
