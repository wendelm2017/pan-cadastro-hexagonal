namespace PanCadastro.Domain.Ports.Out;

// contrato de cache pra consulta de cep - quem implementa decide se vai pro mongo, redis, etc
public interface ICepCache
{
    Task<ViaCepResponse?> ObterAsync(string cep, CancellationToken ct = default);
    Task ArmazenarAsync(string cep, ViaCepResponse response, CancellationToken ct = default);
}
