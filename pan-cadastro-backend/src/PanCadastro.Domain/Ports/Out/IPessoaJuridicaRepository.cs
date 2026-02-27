using PanCadastro.Domain.Entities;

namespace PanCadastro.Domain.Ports.Out;

// Port de Saída específico para Pessoa Jurídica.
public interface IPessoaJuridicaRepository : IRepository<PessoaJuridica>
{
    Task<PessoaJuridica?> ObterPorCnpjAsync(string cnpj, CancellationToken ct = default);
    Task<bool> CnpjExisteAsync(string cnpj, CancellationToken ct = default);
    Task<PessoaJuridica?> ObterComEnderecosAsync(Guid id, CancellationToken ct = default);
}
