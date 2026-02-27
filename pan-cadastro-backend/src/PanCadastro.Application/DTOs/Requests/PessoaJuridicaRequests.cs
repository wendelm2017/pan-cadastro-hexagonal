namespace PanCadastro.Application.DTOs.Requests;

//Dtos específicos para envio de dados de pessoa jurídica nas requisições de criação e atualização.
//uso o tipo record para criar DTOs imutáveis e concisos pois o record é ideal para representar dados 
//que não mudam após a criação, como os dados de uma pessoa jurídica.
public record CriarPessoaJuridicaRequest(
    string RazaoSocial,
    string NomeFantasia,
    string Cnpj,
    DateTime DataAbertura,
    string Email,
    string? Telefone,
    string? InscricaoEstadual
);

public record AtualizarPessoaJuridicaRequest(
    string RazaoSocial,
    string NomeFantasia,
    DateTime DataAbertura,
    string Email,
    string? Telefone,
    string? InscricaoEstadual
);
