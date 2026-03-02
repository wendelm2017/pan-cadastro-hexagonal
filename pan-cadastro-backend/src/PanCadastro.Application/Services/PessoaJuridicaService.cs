using Microsoft.Extensions.Logging;
using PanCadastro.Domain.Entities;
using PanCadastro.Domain.Exceptions;
using PanCadastro.Domain.Ports.In;
using PanCadastro.Domain.Ports.Out;

namespace PanCadastro.Application.Services;

//Tem a lógica e a estrutura similar ao serviço de pessoa física, mas adaptada para pessoas jurídicas. Ele tem métodos de CRUD para pessoas jurídicas, validação de CNPJ para garantir que seja único, e tratamento de erros. O serviço se comunica com o repositório de pessoas jurídicas para acessar os dados no banco de dados, e registra logs para acompanhar as operações realizadas.
public class PessoaJuridicaService : IPessoaJuridicaService
{
    private readonly IPessoaJuridicaRepository _repository;
    private readonly ILogger<PessoaJuridicaService> _logger;

    public PessoaJuridicaService(
        IPessoaJuridicaRepository repository,
        ILogger<PessoaJuridicaService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    // Esse método obtém uma pessoa jurídica por ID, incluindo os endereços relacionados. 
    // Ele lança uma NotFoundException se a pessoa não for encontrada, e registra log.
    public async Task<PessoaJuridica> ObterPorIdAsync(Guid id, CancellationToken ct = default)
    {
        _logger.LogInformation("Buscando Pessoa Jurídica por ID: {Id}", id);

        var empresa = await _repository.ObterComEnderecosAsync(id, ct)
            ?? throw new NotFoundException(nameof(PessoaJuridica), id);

        return empresa;
    }

    // Esse método obtém todas as pessoas jurídicas cadastradas, incluindo os endereços relacionados.
    public async Task<IEnumerable<PessoaJuridica>> ObterTodosAsync(CancellationToken ct = default)
    {
        _logger.LogInformation("Listando todas as Pessoas Jurídicas");
        return await _repository.ObterTodosAsync(ct);
    }

    // Esse método cria uma nova pessoa jurídica, validando o CNPJ para garantir que seja único.
    public async Task<PessoaJuridica> CriarAsync(
        string razaoSocial, string nomeFantasia, string cnpj,
        DateTime dataAbertura, string email, string? telefone,
        string? inscricaoEstadual, CancellationToken ct = default)
    {
        _logger.LogInformation("Criando Pessoa Jurídica com CNPJ: {Cnpj}", cnpj);

        if (await _repository.CnpjExisteAsync(cnpj, ct))
            throw new DomainException($"CNPJ {cnpj} já está cadastrado.");

        var empresa = PessoaJuridica.Criar(
            razaoSocial, nomeFantasia, cnpj,
            dataAbertura, email, telefone, inscricaoEstadual);

        await _repository.AdicionarAsync(empresa, ct);

        _logger.LogInformation("Pessoa Jurídica criada com ID: {Id}", empresa.Id);
        return empresa;
    }

    // Esse método atualiza um endereço existente, validando os dados de entrada e 
    // usando o repositório para atualizar o endereço no banco de dados.
    public async Task<PessoaJuridica> AtualizarAsync(
        Guid id, string razaoSocial, string nomeFantasia,
        DateTime dataAbertura, string email, string? telefone,
        string? inscricaoEstadual, CancellationToken ct = default)
    {
        _logger.LogInformation("Atualizando Pessoa Jurídica ID: {Id}", id);

        var empresa = await _repository.ObterPorIdAsync(id, ct)
            ?? throw new NotFoundException(nameof(PessoaJuridica), id);

        empresa.Atualizar(razaoSocial, nomeFantasia, dataAbertura, email, telefone, inscricaoEstadual);

        await _repository.AtualizarAsync(empresa, ct);

        _logger.LogInformation("Pessoa Jurídica atualizada: {Id}", id);
        return empresa;
    }

    // Esse método remove uma pessoa jurídica por ID, usando o repositório para verificar se a pessoa existe e para removê-la do banco de dados.
    public async Task RemoverAsync(Guid id, CancellationToken ct = default)
    {
        _logger.LogInformation("Removendo Pessoa Jurídica ID: {Id}", id);

        if (!await _repository.ExisteAsync(id, ct))
            throw new NotFoundException(nameof(PessoaJuridica), id);

        await _repository.RemoverAsync(id, ct);

        _logger.LogInformation("Pessoa Jurídica removida: {Id}", id);
    }
}
