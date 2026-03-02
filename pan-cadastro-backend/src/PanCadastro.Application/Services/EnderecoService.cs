using Microsoft.Extensions.Logging;
using PanCadastro.Domain.Entities;
using PanCadastro.Domain.Exceptions;
using PanCadastro.Domain.Ports.In;
using PanCadastro.Domain.Ports.Out;

namespace PanCadastro.Application.Services;

// Service de Endereços — CRUD + consulta ViaCEP
public class EnderecoService : IEnderecoService
{
    private readonly IEnderecoRepository _repository;
    private readonly IViaCepClient _viaCepClient;
    private readonly ILogger<EnderecoService> _logger;

    public EnderecoService(
        IEnderecoRepository repository,
        IViaCepClient viaCepClient,
        ILogger<EnderecoService> logger)
    {
        _repository = repository;
        _viaCepClient = viaCepClient;
        _logger = logger;
    }

    public async Task<Endereco> ObterPorIdAsync(Guid id, CancellationToken ct = default)
    {
        var endereco = await _repository.ObterPorIdAsync(id, ct)
            ?? throw new NotFoundException(nameof(Endereco), id);
        return endereco;
    }

    public async Task<IEnumerable<Endereco>> ObterTodosAsync(CancellationToken ct = default)
    {
        return await _repository.ObterTodosAsync(ct);
    }

    public async Task<IEnumerable<Endereco>> ObterPorPessoaFisicaAsync(Guid pessoaFisicaId, CancellationToken ct = default)
    {
        return await _repository.ObterPorPessoaFisicaAsync(pessoaFisicaId, ct);
    }

    public async Task<IEnumerable<Endereco>> ObterPorPessoaJuridicaAsync(Guid pessoaJuridicaId, CancellationToken ct = default)
    {
        return await _repository.ObterPorPessoaJuridicaAsync(pessoaJuridicaId, ct);
    }

    public async Task<Endereco> CriarAsync(
        string cep, string logradouro, string numero,
        string bairro, string cidade, string estado,
        string? complemento, Guid? pessoaFisicaId,
        Guid? pessoaJuridicaId, CancellationToken ct = default)
    {
        _logger.LogInformation("Criando endereço com CEP: {Cep}", cep);

        var endereco = Endereco.Criar(
            cep, logradouro, numero, bairro, cidade, estado,
            complemento, pessoaFisicaId, pessoaJuridicaId);

        await _repository.AdicionarAsync(endereco, ct);

        _logger.LogInformation("Endereço criado com ID: {Id}", endereco.Id);
        return endereco;
    }

    public async Task<Endereco> AtualizarAsync(
        Guid id, string cep, string logradouro, string numero,
        string bairro, string cidade, string estado,
        string? complemento, CancellationToken ct = default)
    {
        _logger.LogInformation("Atualizando endereço ID: {Id}", id);

        var endereco = await _repository.ObterPorIdAsync(id, ct)
            ?? throw new NotFoundException(nameof(Endereco), id);

        endereco.Atualizar(cep, logradouro, numero, bairro, cidade, estado, complemento);

        await _repository.AtualizarAsync(endereco, ct);

        _logger.LogInformation("Endereço atualizado: {Id}", id);
        return endereco;
    }

    public async Task RemoverAsync(Guid id, CancellationToken ct = default)
    {
        _logger.LogInformation("Removendo endereço ID: {Id}", id);

        if (!await _repository.ExisteAsync(id, ct))
            throw new NotFoundException(nameof(Endereco), id);

        await _repository.RemoverAsync(id, ct);
        _logger.LogInformation("Endereço removido: {Id}", id);
    }

    // consulta ViaCEP externo
    public async Task<ViaCepResponse?> ConsultarCepAsync(string cep, CancellationToken ct = default)
    {
        _logger.LogInformation("Consultando CEP: {Cep}", cep);

        try
        {
            var response = await _viaCepClient.ConsultarCepAsync(cep, ct);

            if (response is null || response.Erro)
            {
                _logger.LogWarning("CEP não encontrado: {Cep}", cep);
                return null;
            }

            _logger.LogInformation("CEP encontrado: {Cep} - {Localidade}/{Uf}", cep, response.Localidade, response.Uf);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao consultar CEP: {Cep}", cep);
            return null;
        }
    }
}
