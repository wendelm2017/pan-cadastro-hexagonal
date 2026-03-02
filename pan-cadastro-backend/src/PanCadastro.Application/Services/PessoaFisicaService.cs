using Microsoft.Extensions.Logging;
using PanCadastro.Domain.Entities;
using PanCadastro.Domain.Exceptions;
using PanCadastro.Domain.Ports.In;
using PanCadastro.Domain.Ports.Out;

namespace PanCadastro.Application.Services;

//Service de PF — CRUD + regra de unicidade CPF.
public class PessoaFisicaService : IPessoaFisicaService
{
    private readonly IPessoaFisicaRepository _repository;
    private readonly ILogger<PessoaFisicaService> _logger;

    public PessoaFisicaService(
        IPessoaFisicaRepository repository,
        ILogger<PessoaFisicaService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    // Esse método obtém uma pessoa física por ID, incluindo os endereços relacionados. 
    // Ele lança uma NotFoundException se a pessoa não for encontrada, e registra log.
    public async Task<PessoaFisica> ObterPorIdAsync(Guid id, CancellationToken ct = default)
    {
        _logger.LogInformation("Buscando Pessoa Física por ID: {Id}", id);

        var pessoa = await _repository.ObterComEnderecosAsync(id, ct)
            ?? throw new NotFoundException(nameof(PessoaFisica), id);

        return pessoa;
    }

    // Esse método obtém todas as pessoas físicas cadastradas, incluindo os endereços relacionados.
    // Ele registra log para acompanhar a operação.
    public async Task<IEnumerable<PessoaFisica>> ObterTodosAsync(CancellationToken ct = default)
    {
        _logger.LogInformation("Listando todas as Pessoas Físicas");
        return await _repository.ObterTodosAsync(ct);
    }

    // Esse método cria uma nova pessoa física, validando o CPF para garantir que seja único.
    // Ele usa o repositório para adicionar a pessoa ao banco de dados, e registra
    public async Task<PessoaFisica> CriarAsync(
        string nome, string cpf, DateTime dataNascimento,
        string email, string? telefone, CancellationToken ct = default)
    {
        _logger.LogInformation("Criando Pessoa Física com CPF: {Cpf}", cpf);

        // Regra: CPF deve ser único
        if (await _repository.CpfExisteAsync(cpf, ct))
            throw new DomainException($"CPF {cpf} já está cadastrado.");

        var pessoa = PessoaFisica.Criar(nome, cpf, dataNascimento, email, telefone);

        await _repository.AdicionarAsync(pessoa, ct);

        _logger.LogInformation("Pessoa Física criada com ID: {Id}", pessoa.Id);
        return pessoa;
    }

    // Esse método atualiza um endereço existente, validando os dados de entrada e usando o repositório para atualizar o endereço no banco de dados.
    public async Task<PessoaFisica> AtualizarAsync(
        Guid id, string nome, DateTime dataNascimento,
        string email, string? telefone, CancellationToken ct = default)
    {
        _logger.LogInformation("Atualizando Pessoa Física ID: {Id}", id);

        var pessoa = await _repository.ObterPorIdAsync(id, ct)
            ?? throw new NotFoundException(nameof(PessoaFisica), id);

        pessoa.Atualizar(nome, dataNascimento, email, telefone);

        await _repository.AtualizarAsync(pessoa, ct);

        _logger.LogInformation("Pessoa Física atualizada: {Id}", id);
        return pessoa;
    }

    // Esse método remove uma pessoa física por ID, usando o repositório para verificar se a pessoa existe e para removê-la do banco de dados.
    public async Task RemoverAsync(Guid id, CancellationToken ct = default)
    {
        _logger.LogInformation("Removendo Pessoa Física ID: {Id}", id);

        if (!await _repository.ExisteAsync(id, ct))
            throw new NotFoundException(nameof(PessoaFisica), id);

        await _repository.RemoverAsync(id, ct);

        _logger.LogInformation("Pessoa Física removida: {Id}", id);
    }
}
