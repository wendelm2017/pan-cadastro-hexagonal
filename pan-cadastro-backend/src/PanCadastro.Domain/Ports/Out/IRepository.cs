using PanCadastro.Domain.Entities;

namespace PanCadastro.Domain.Ports.Out;

// Port de Saída para contrato genérico de repositório.
// Usa o pattern Repository que abstrai completamente a persistência
// permitindo no Adapter Driven a implementação com EF Core, Dapper, MongoDB, etc.
public interface IRepository<T> where T : BaseEntity
{
    Task<T?> ObterPorIdAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<T>> ObterTodosAsync(CancellationToken ct = default);
    Task<T> AdicionarAsync(T entity, CancellationToken ct = default);
    Task AtualizarAsync(T entity, CancellationToken ct = default);
    Task RemoverAsync(Guid id, CancellationToken ct = default);
    Task<bool> ExisteAsync(Guid id, CancellationToken ct = default);
}
