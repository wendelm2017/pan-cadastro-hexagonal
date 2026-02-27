namespace PanCadastro.Domain.Ports.Out;

// Port de Saída para consulta de CEP em serviço externo.
// Usa o pattern Adapter onde o Domain define o contrato e o Driven Adapter implementa.
public interface IViaCepClient
{
    Task<ViaCepResponse?> ConsultarCepAsync(string cep, CancellationToken ct = default);
}

// O modelo de resposta do serviço de CEP é definido no Domain para não depender de DTOs externos.
public record ViaCepResponse(
    string Cep,
    string Logradouro,
    string Complemento,
    string Bairro,
    string Localidade,
    string Uf,
    bool Erro = false
);
