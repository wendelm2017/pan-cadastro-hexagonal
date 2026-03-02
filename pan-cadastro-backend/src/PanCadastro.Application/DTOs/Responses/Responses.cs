namespace PanCadastro.Application.DTOs.Responses;

// DTOs de resposta da API (records imutáveis)
public record PessoaFisicaResponse(
    Guid Id,
    string Nome,
    string Cpf,
    string CpfFormatado,
    DateTime DataNascimento,
    string Email,
    string? Telefone,
    bool Ativo,
    DateTime CriadoEm,
    DateTime? AtualizadoEm,
    IEnumerable<EnderecoResponse> Enderecos
);

public record PessoaJuridicaResponse(
    Guid Id,
    string RazaoSocial,
    string NomeFantasia,
    string Cnpj,
    string CnpjFormatado,
    DateTime DataAbertura,
    string Email,
    string? Telefone,
    string? InscricaoEstadual,
    bool Ativo,
    DateTime CriadoEm,
    DateTime? AtualizadoEm,
    IEnumerable<EnderecoResponse> Enderecos
);

public record EnderecoResponse(
    Guid Id,
    string Cep,
    string CepFormatado,
    string Logradouro,
    string Numero,
    string? Complemento,
    string Bairro,
    string Cidade,
    string Estado,
    Guid? PessoaFisicaId,
    Guid? PessoaJuridicaId,
    bool Ativo,
    DateTime CriadoEm,
    DateTime? AtualizadoEm
);

public record ViaCepResponseDto(
    string Cep,
    string Logradouro,
    string Complemento,
    string Bairro,
    string Localidade,
    string Uf,
    string Estado,
    string Regiao,
    string Ibge,
    string Ddd,
    string Siafi
);

// emvelope padrão para respostas da API.
public record ApiResponse<T>(
    bool Sucesso,
    string? Mensagem,
    T? Dados
)
{
    public static ApiResponse<T> Ok(T dados, string? mensagem = null)
        => new(true, mensagem, dados);

    public static ApiResponse<T> Erro(string mensagem)
        => new(false, mensagem, default);
}
