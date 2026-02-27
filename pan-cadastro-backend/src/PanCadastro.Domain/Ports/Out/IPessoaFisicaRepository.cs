using PanCadastro.Domain.Entities;

namespace PanCadastro.Domain.Ports.Out;

// Port de Saída específico para Pessoa Física.
// Estende o repositório genérico com queries de domínio.
public interface IPessoaFisicaRepository : IRepository<PessoaFisica>
{
    Task<PessoaFisica?> ObterPorCpfAsync(string cpf, CancellationToken ct = default);
    Task<bool> CpfExisteAsync(string cpf, CancellationToken ct = default);
    Task<PessoaFisica?> ObterComEnderecosAsync(Guid id, CancellationToken ct = default);
}
