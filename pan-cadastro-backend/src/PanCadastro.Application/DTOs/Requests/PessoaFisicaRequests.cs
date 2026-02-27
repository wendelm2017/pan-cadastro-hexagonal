namespace PanCadastro.Application.DTOs.Requests;

//Dto específico para envio de dados de pessoa física nas requisições de criação e atualização.
//uso o tipo record para criar DTOs imutáveis e concisos pois o record é ideal para representar dados 
//que não mudam após a criação, como os dados de uma pessoa física.
public record CriarPessoaFisicaRequest(
    string Nome,
    string Cpf,
    DateTime DataNascimento,
    string Email,
    string? Telefone
);

public record AtualizarPessoaFisicaRequest(
    string Nome,
    DateTime DataNascimento,
    string Email,
    string? Telefone
);
