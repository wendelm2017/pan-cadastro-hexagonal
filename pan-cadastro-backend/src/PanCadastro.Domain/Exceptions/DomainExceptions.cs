namespace PanCadastro.Domain.Exceptions;

// Classe de exceção base para violações de regra de domínio.
public class DomainException : Exception
{
    public DomainException(string message) : base(message) { }
    public DomainException(string message, Exception inner) : base(message, inner) { }
}

// Classe de exceção para entidade não encontrada.
public class NotFoundException : DomainException
{
    public NotFoundException(string entityName, object key)
        : base($"{entityName} com identificador '{key}' não foi encontrado(a).") { }
}
