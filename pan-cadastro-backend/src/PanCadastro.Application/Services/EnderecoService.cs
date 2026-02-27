using Microsoft.Extensions.Logging;
using PanCadastro.Domain.Entities;
using PanCadastro.Domain.Exceptions;
using PanCadastro.Domain.Ports.In;
using PanCadastro.Domain.Ports.Out;

namespace PanCadastro.Application.Services;

//Essa classe de serviço é responsável por implementar a lógica de negócio relacionada 
//aos endereços realizando operações de CRUD e consultas por pessoa física/jurídica.
//Ela se comunica com o repositório de endereços (IEnderecoRepository) para acessar os 
//dados na base, e com o cliente de API externa IViaCepClient. Possui tratamento de erros e registro de logs
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

    // Esse método obtém um endereço por ID, lançando uma NotFoundException se o endereço não for encontrado.
    public async Task<Endereco> ObterPorIdAsync(Guid id, CancellationToken ct = default)
    {
        var endereco = await _repository.ObterPorIdAsync(id, ct)
            ?? throw new NotFoundException(nameof(Endereco), id);
        return endereco;
    }

    // Esse método obtém todos os endereços cadastrados, retornando uma lista de endereços.
    public async Task<IEnumerable<Endereco>> ObterTodosAsync(CancellationToken ct = default)
    {
        return await _repository.ObterTodosAsync(ct);
    }

    // Esses métodos obtêm os endereços relacionados a uma pessoa física ou jurídica, usando o repositório para consultar por pessoa.
    public async Task<IEnumerable<Endereco>> ObterPorPessoaFisicaAsync(Guid pessoaFisicaId, CancellationToken ct = default)
    {
        return await _repository.ObterPorPessoaFisicaAsync(pessoaFisicaId, ct);
    }

    // Esse método obtém os endereços relacionados a uma pessoa jurídica, usando o repositório para consultar por pessoa.        
    public async Task<IEnumerable<Endereco>> ObterPorPessoaJuridicaAsync(Guid pessoaJuridicaId, CancellationToken ct = default)
    {
        return await _repository.ObterPorPessoaJuridicaAsync(pessoaJuridicaId, ct);
    }

    // Esse método cria um novo endereço, validando os dados de entrada e usando o repositório para adicionar o endereço ao banco de dados. Ele também registra logs para acompanhar a criação do endereço.
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

    // Esse método atualiza um endereço existente, validando os dados de entrada e usando o repositório para atualizar o endereço no banco de dados. Ele também registra logs para acompanhar a atualização do endereço.
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

    // Esse método remove um endereço por ID, usando o repositório para verificar se o endereço existe e para removê-lo do banco de dados. Ele também registra logs para acompanhar a remoção do endereço.
    public async Task RemoverAsync(Guid id, CancellationToken ct = default)
    {
        _logger.LogInformation("Removendo endereço ID: {Id}", id);

        if (!await _repository.ExisteAsync(id, ct))
            throw new NotFoundException(nameof(Endereco), id);

        await _repository.RemoverAsync(id, ct);
        _logger.LogInformation("Endereço removido: {Id}", id);
    }

    // Esse método consulta o CEP via API externa (ViaCEP).
    // O ViaCepClient é um Adapter que foi criado para cachear no MongoDB.
    public async Task<ViaCepResponse?> ConsultarCepAsync(string cep, CancellationToken ct = default)
    {
        _logger.LogInformation("Consultando CEP: {Cep}", cep);

        var response = await _viaCepClient.ConsultarCepAsync(cep, ct);

        if (response is null || response.Erro)
        {
            _logger.LogWarning("CEP não encontrado: {Cep}", cep);
            return null;
        }

        _logger.LogInformation("CEP encontrado: {Cep} - {Localidade}/{Uf}", cep, response.Localidade, response.Uf);
        return response;
    }
}
