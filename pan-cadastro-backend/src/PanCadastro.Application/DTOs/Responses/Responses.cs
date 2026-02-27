namespace PanCadastro.Application.DTOs.Responses;

//Dto criados para envio de dados de pessoa física, pessoa jurídica e endereço nas respostas da API.
//uso o tipo record para criar DTOs imutáveis e concisos pois o record é ideal para representar dados
//que não mudam após a criação, como os dados de uma pessoa física.
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
    string Uf
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
