using PanCadastro.Domain.Entities;

namespace PanCadastro.Domain.Ports.Out;

// Port de Saída específico para Endereço.
public interface IEnderecoRepository : IRepository<Endereco>
{
    Task<IEnumerable<Endereco>> ObterPorPessoaFisicaAsync(Guid pessoaFisicaId, CancellationToken ct = default);
    Task<IEnumerable<Endereco>> ObterPorPessoaJuridicaAsync(Guid pessoaJuridicaId, CancellationToken ct = default);
}
