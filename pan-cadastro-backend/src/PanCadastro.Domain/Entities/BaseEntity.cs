namespace PanCadastro.Domain.Entities;

// Essa é uma entidade base com propriedades comuns de auditoria.
// Usei o pattern Layer Supertype para centralizar comportamento comum das 
//entidades e o conceito SOLID de responsabilidade única, mantendo as 
//entidades focadas apenas em suas propriedades e regras de negócio específicas. 
public abstract class BaseEntity
{
    public Guid Id { get; protected set; }
    public DateTime CriadoEm { get; protected set; }
    public DateTime? AtualizadoEm { get; protected set; }
    public bool Ativo { get; protected set; }

    protected BaseEntity()
    {
        Id = Guid.NewGuid();
        CriadoEm = DateTime.UtcNow;
        Ativo = true;
    }

    public void Desativar()
    {
        Ativo = false;
        AtualizadoEm = DateTime.UtcNow;
    }

    public void Ativar()
    {
        Ativo = true;
        AtualizadoEm = DateTime.UtcNow;
    }

    protected void MarcarAtualizado()
    {
        AtualizadoEm = DateTime.UtcNow;
    }
}
