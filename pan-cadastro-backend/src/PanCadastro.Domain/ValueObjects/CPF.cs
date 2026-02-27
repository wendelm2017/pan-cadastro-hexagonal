using PanCadastro.Domain.Exceptions;

namespace PanCadastro.Domain.ValueObjects;

// CPF com validação de dígitos verificadores.
public sealed class CPF : IEquatable<CPF>
{
    public string Numero { get; }

    private CPF(string numero)
    {
        Numero = numero;
    }

    public static CPF Criar(string numero)
    {
        var apenasDigitos = new string(numero?.Where(char.IsDigit).ToArray() ?? Array.Empty<char>());

        if (!Validar(apenasDigitos))
            throw new DomainException($"CPF inválido: {numero}");

        return new CPF(apenasDigitos);
    }

    public static bool Validar(string cpf)
    {
        if (string.IsNullOrWhiteSpace(cpf))
            return false;

        var apenasDigitos = new string(cpf.Where(char.IsDigit).ToArray());

        if (apenasDigitos.Length != 11)
            return false;

        // Rejeita CPFs com todos os dígitos iguais (ex: 111.111.111-11)
        if (apenasDigitos.Distinct().Count() == 1)
            return false;

        // Cálcula o primeiro dígito verificador
        var soma = 0;
        for (int i = 0; i < 9; i++)
            soma += (apenasDigitos[i] - '0') * (10 - i);

        var resto = soma % 11;
        var primeiroDigito = resto < 2 ? 0 : 11 - resto;

        if (apenasDigitos[9] - '0' != primeiroDigito)
            return false;

        // Cálcula o segundo dígito verificador
        soma = 0;
        for (int i = 0; i < 10; i++)
            soma += (apenasDigitos[i] - '0') * (11 - i);

        resto = soma % 11;
        var segundoDigito = resto < 2 ? 0 : 11 - resto;

        return apenasDigitos[10] - '0' == segundoDigito;
    }

    public string Formatado => Convert.ToUInt64(Numero).ToString(@"000\.000\.000\-00");

    public bool Equals(CPF? other) => other is not null && Numero == other.Numero;
    public override bool Equals(object? obj) => obj is CPF other && Equals(other);
    public override int GetHashCode() => Numero.GetHashCode();
    public override string ToString() => Formatado;

    public static bool operator ==(CPF? left, CPF? right) => Equals(left, right);
    public static bool operator !=(CPF? left, CPF? right) => !Equals(left, right);
}
