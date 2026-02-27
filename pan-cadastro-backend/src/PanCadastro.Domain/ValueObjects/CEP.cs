using PanCadastro.Domain.Exceptions;

namespace PanCadastro.Domain.ValueObjects;

// Classe sealed Value Object para CEP — validação de formato (8 dígitos numéricos).
//Ela é sealed para garantir imutabilidade e segurança, 
//e implementa IEquatable para comparação de valor.
public sealed class CEP : IEquatable<CEP>
{
    public string Numero { get; }

    private CEP(string numero)
    {
        Numero = numero;
    }

    public static CEP Criar(string numero)
    {
        var apenasDigitos = new string(numero?.Where(char.IsDigit).ToArray() ?? Array.Empty<char>());

        if (apenasDigitos.Length != 8)
            throw new DomainException($"CEP inválido: {numero}. O CEP deve conter 8 dígitos.");

        return new CEP(apenasDigitos);
    }

    public string Formatado => $"{Numero[..5]}-{Numero[5..]}";

    public bool Equals(CEP? other) => other is not null && Numero == other.Numero;
    public override bool Equals(object? obj) => obj is CEP other && Equals(other);
    public override int GetHashCode() => Numero.GetHashCode();
    public override string ToString() => Formatado;

    public static bool operator ==(CEP? left, CEP? right) => Equals(left, right);
    public static bool operator !=(CEP? left, CEP? right) => !Equals(left, right);
}
