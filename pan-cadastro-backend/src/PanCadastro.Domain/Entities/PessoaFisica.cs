using PanCadastro.Domain.Exceptions;
using PanCadastro.Domain.ValueObjects;

namespace PanCadastro.Domain.Entities;

// PF com validações no domínio. CPF como Value Object.
public class PessoaFisica : BaseEntity
{
    public string Nome { get; private set; } = string.Empty;
    public CPF Cpf { get; private set; } = null!;
    public DateTime DataNascimento { get; private set; }
    public string Email { get; private set; } = string.Empty;
    public string? Telefone { get; private set; }

    private readonly List<Endereco> _enderecos = new();
    public IReadOnlyCollection<Endereco> Enderecos => _enderecos.AsReadOnly();

    protected PessoaFisica() { }

    public static PessoaFisica Criar(
        string nome,
        string cpf,
        DateTime dataNascimento,
        string email,
        string? telefone = null)
    {
        var pessoa = new PessoaFisica();
        pessoa.SetNome(nome);
        pessoa.Cpf = CPF.Criar(cpf);
        pessoa.SetDataNascimento(dataNascimento);
        pessoa.SetEmail(email);
        pessoa.Telefone = telefone;

        return pessoa;
    }

    public void Atualizar(
        string nome,
        DateTime dataNascimento,
        string email,
        string? telefone = null)
    {
        SetNome(nome);
        SetDataNascimento(dataNascimento);
        SetEmail(email);
        Telefone = telefone;
        MarcarAtualizado();
    }

    public void AdicionarEndereco(Endereco endereco)
    {
        endereco.VincularPessoaFisica(Id);
        _enderecos.Add(endereco);
        MarcarAtualizado();
    }

    public void RemoverEndereco(Guid enderecoId)
    {
        var endereco = _enderecos.FirstOrDefault(e => e.Id == enderecoId)
            ?? throw new NotFoundException(nameof(Endereco), enderecoId);

        _enderecos.Remove(endereco);
        MarcarAtualizado();
    }

    // --- Regras de Negócio ---
    private void SetNome(string nome)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new DomainException("Nome é obrigatório.");

        if (nome.Length < 2 || nome.Length > 200)
            throw new DomainException("Nome deve ter entre 2 e 200 caracteres.");

        Nome = nome.Trim();
    }

    private void SetDataNascimento(DateTime dataNascimento)
    {
        if (dataNascimento >= DateTime.Today)
            throw new DomainException("Data de nascimento deve ser anterior à data atual.");

        var idade = DateTime.Today.Year - dataNascimento.Year;
        if (dataNascimento.Date > DateTime.Today.AddYears(-idade)) idade--;

        if (idade > 150)
            throw new DomainException("Data de nascimento inválida.");

        DataNascimento = dataNascimento;
    }

    private void SetEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new DomainException("E-mail é obrigatório.");

        if (!email.Contains('@') || !email.Contains('.'))
            throw new DomainException("E-mail inválido.");

        Email = email.Trim().ToLowerInvariant();
    }
}
