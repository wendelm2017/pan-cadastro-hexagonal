namespace PanCadastro.Application.DTOs.Requests;

//Dto específico para envio de dados de endereço nas requisições de criação e atualização.
//uso o tipo record para criar DTOs imutáveis e concisos pois o record é ideal para representar dados 
//que não mudam após a criação, como os dados de um endereço.
public record CriarEnderecoRequest(
    string Cep,
    string Logradouro,
    string Numero,
    string Bairro,
    string Cidade,
    string Estado,
    string? Complemento,
    Guid? PessoaFisicaId,
    Guid? PessoaJuridicaId
);

public record AtualizarEnderecoRequest(
    string Cep,
    string Logradouro,
    string Numero,
    string Bairro,
    string Cidade,
    string Estado,
    string? Complemento
);
