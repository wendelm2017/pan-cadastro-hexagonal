using Microsoft.Extensions.Logging;
using PanCadastro.Domain.Ports.Out;

namespace PanCadastro.Adapters.Driven.ExternalServices;

// decorator que adiciona cache ao viacep client original
// fluxo: consulta cache → se nao tem, chama a api → armazena no cache pro proximo
// se o cache falhar em qualquer ponto, segue chamando a api normalmente
public class CachedViaCepClient : IViaCepClient
{
    private readonly IViaCepClient _inner;
    private readonly ICepCache _cache;
    private readonly ILogger<CachedViaCepClient> _logger;

    public CachedViaCepClient(IViaCepClient inner, ICepCache cache, ILogger<CachedViaCepClient> logger)
    {
        _inner = inner;
        _cache = cache;
        _logger = logger;
    }

    public async Task<ViaCepResponse?> ConsultarCepAsync(string cep, CancellationToken ct = default)
    {
        var apenasDigitos = new string(cep.Where(char.IsDigit).ToArray());

        // tenta buscar do cache primeiro
        var cached = await _cache.ObterAsync(apenasDigitos, ct);
        if (cached is not null && !cached.Erro)
        {
            _logger.LogInformation("CEP {Cep} servido do cache", apenasDigitos);
            return cached;
        }

        // cache miss - vai na api do viacep
        _logger.LogInformation("CEP {Cep} nao encontrado no cache, chamando ViaCEP", apenasDigitos);
        var response = await _inner.ConsultarCepAsync(cep, ct);

        // se a api retornou dado valido, guarda no cache pra proxima vez
        if (response is not null && !response.Erro)
        {
            await _cache.ArmazenarAsync(apenasDigitos, response, ct);
        }

        return response;
    }
}
