using PanCadastro.Domain.Entities;

namespace PanCadastro.Domain.Ports.In;

// Port de entradaque usa os mesmos conceitos de IPessoaFisicaService para Pessoa Jurídica.
public interface IPessoaJuridicaService
{
    Task<PessoaJuridica> ObterPorIdAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<PessoaJuridica>> ObterTodosAsync(CancellationToken ct = default);
    Task<PessoaJuridica> CriarAsync(string razaoSocial, string nomeFantasia, string cnpj, DateTime dataAbertura, string email, string? telefone, string? inscricaoEstadual, CancellationToken ct = default);
    Task<PessoaJuridica> AtualizarAsync(Guid id, string razaoSocial, string nomeFantasia, DateTime dataAbertura, string email, string? telefone, string? inscricaoEstadual, CancellationToken ct = default);
    Task RemoverAsync(Guid id, CancellationToken ct = default);
}
