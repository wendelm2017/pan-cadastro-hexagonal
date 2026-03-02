using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using PanCadastro.Domain.Ports.Out;

namespace PanCadastro.Adapters.Driven.ExternalServices;

// Aqui uso um Driven Adapter para implementar o Port IViaCepClient.
// O Pattern Adapter traduz a API externa (ViaCEP) para o contrato do Domain.
// Também uso HttpClient injetado via HttpClientFactory (resiliência).
public class ViaCepClient : IViaCepClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ViaCepClient> _logger;

    public ViaCepClient(HttpClient httpClient, ILogger<ViaCepClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<ViaCepResponse?> ConsultarCepAsync(string cep, CancellationToken ct = default)
    {
        try
        {
            var apenasDigitos = new string(cep.Where(char.IsDigit).ToArray());

            if (apenasDigitos.Length != 8)
            {
                _logger.LogWarning("CEP com formato inválido: {Cep}", cep);
                return null;
            }

            var url = $"{apenasDigitos}/json/";
            _logger.LogInformation("Consultando ViaCEP: {Url}", url);

            var response = await _httpClient.GetAsync(url, ct);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("ViaCEP retornou status {Status} para CEP: {Cep}",
                    response.StatusCode, cep);
                return null;
            }

            var viaCepDto = await response.Content.ReadFromJsonAsync<ViaCepApiResponse>(cancellationToken: ct);

            if (viaCepDto is null || viaCepDto.Erro)
            {
                _logger.LogWarning("CEP não encontrado no ViaCEP: {Cep}", cep);
                return new ViaCepResponse(cep, "", "", "", "", "", Erro: true);
            }

            return new ViaCepResponse(
                Cep: viaCepDto.Cep ?? "",
                Logradouro: viaCepDto.Logradouro ?? "",
                Complemento: viaCepDto.Complemento ?? "",
                Bairro: viaCepDto.Bairro ?? "",
                Localidade: viaCepDto.Localidade ?? "",
                Uf: viaCepDto.Uf ?? "",
                Estado: viaCepDto.Estado ?? "",
                Regiao: viaCepDto.Regiao ?? "",
                Ibge: viaCepDto.Ibge ?? "",
                Ddd: viaCepDto.Ddd ?? "",
                Siafi: viaCepDto.Siafi ?? ""
            );
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Erro ao consultar ViaCEP para CEP: {Cep}", cep);
            return null;
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogWarning(ex, "Timeout ao consultar ViaCEP para CEP: {Cep}", cep);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao consultar ViaCEP para CEP: {Cep}", cep);
            return null;
        }
    }

    // DTO interno para deserialização do JSON de retorno do ViaCEP.
    private sealed class ViaCepApiResponse
    {
        [JsonPropertyName("cep")]
        public string? Cep { get; set; }

        [JsonPropertyName("logradouro")]
        public string? Logradouro { get; set; }

        [JsonPropertyName("complemento")]
        public string? Complemento { get; set; }

        [JsonPropertyName("bairro")]
        public string? Bairro { get; set; }

        [JsonPropertyName("localidade")]
        public string? Localidade { get; set; }

        [JsonPropertyName("uf")]
        public string? Uf { get; set; }

        [JsonPropertyName("estado")]
        public string? Estado { get; set; }

        [JsonPropertyName("regiao")]
        public string? Regiao { get; set; }

        [JsonPropertyName("ibge")]
        public string? Ibge { get; set; }

        [JsonPropertyName("ddd")]
        public string? Ddd { get; set; }

        [JsonPropertyName("siafi")]
        public string? Siafi { get; set; }

        [JsonPropertyName("erro")]
        public bool Erro { get; set; }
    }
}
