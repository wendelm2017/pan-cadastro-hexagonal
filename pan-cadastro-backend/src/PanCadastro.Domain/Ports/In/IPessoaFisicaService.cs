using PanCadastro.Domain.Entities;

namespace PanCadastro.Domain.Ports.In;

// Esse Port In é o contrato do caso de uso de Pessoa Física.
// O Controller (Driving Adapter) chama esta interface e a Application Layer implementa.
//Ele define as operações de CRUD para Pessoa Física, e a implementação concreta do serviço
// de domínio na Application Layer
public interface IPessoaFisicaService
{
    Task<PessoaFisica> ObterPorIdAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<PessoaFisica>> ObterTodosAsync(CancellationToken ct = default);
    Task<PessoaFisica> CriarAsync(string nome, string cpf, DateTime dataNascimento, string email, string? telefone, CancellationToken ct = default);
    Task<PessoaFisica> AtualizarAsync(Guid id, string nome, DateTime dataNascimento, string email, string? telefone, CancellationToken ct = default);
    Task RemoverAsync(Guid id, CancellationToken ct = default);
}
