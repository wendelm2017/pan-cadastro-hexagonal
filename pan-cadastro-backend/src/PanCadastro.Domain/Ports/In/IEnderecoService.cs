using PanCadastro.Domain.Entities;
using PanCadastro.Domain.Ports.Out;

namespace PanCadastro.Domain.Ports.In;

// Port de Entrada para Endereço com consulta ViaCEP.
// Na arquitetura hexagonal, esse serviço de domínio é a porta de entrada
// para as operações e nesse caso inclui a consulta ao ViaCEP
// Ele define as operações que serão implementadas no serviço de endereços
public interface IEnderecoService
{
    Task<Endereco> ObterPorIdAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<Endereco>> ObterTodosAsync(CancellationToken ct = default);
    Task<IEnumerable<Endereco>> ObterPorPessoaFisicaAsync(Guid pessoaFisicaId, CancellationToken ct = default);
    Task<IEnumerable<Endereco>> ObterPorPessoaJuridicaAsync(Guid pessoaJuridicaId, CancellationToken ct = default);
    Task<Endereco> CriarAsync(string cep, string logradouro, string numero, string bairro, string cidade, string estado, string? complemento, Guid? pessoaFisicaId, Guid? pessoaJuridicaId, CancellationToken ct = default);
    Task<Endereco> AtualizarAsync(Guid id, string cep, string logradouro, string numero, string bairro, string cidade, string estado, string? complemento, CancellationToken ct = default);
    Task RemoverAsync(Guid id, CancellationToken ct = default);
    Task<ViaCepResponse?> ConsultarCepAsync(string cep, CancellationToken ct = default);
}
