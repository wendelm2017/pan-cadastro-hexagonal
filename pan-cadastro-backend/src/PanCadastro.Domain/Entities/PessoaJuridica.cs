using PanCadastro.Domain.Exceptions;
using PanCadastro.Domain.ValueObjects;

namespace PanCadastro.Domain.Entities;

// A entidade Pessoa Jurídica também usa Rich Domain Model com validações embutidas.
public class PessoaJuridica : BaseEntity
{
    public string RazaoSocial { get; private set; } = string.Empty;
    public string NomeFantasia { get; private set; } = string.Empty;
    public CNPJ Cnpj { get; private set; } = null!;
    public DateTime DataAbertura { get; private set; }
    public string Email { get; private set; } = string.Empty;
    public string? Telefone { get; private set; }
    public string? InscricaoEstadual { get; private set; }

    private readonly List<Endereco> _enderecos = new();
    public IReadOnlyCollection<Endereco> Enderecos => _enderecos.AsReadOnly();

    protected PessoaJuridica() { }

    public static PessoaJuridica Criar(
        string razaoSocial,
        string nomeFantasia,
        string cnpj,
        DateTime dataAbertura,
        string email,
        string? telefone = null,
        string? inscricaoEstadual = null)
    {
        var empresa = new PessoaJuridica();
        empresa.SetRazaoSocial(razaoSocial);
        empresa.SetNomeFantasia(nomeFantasia);
        empresa.Cnpj = CNPJ.Criar(cnpj);
        empresa.SetDataAbertura(dataAbertura);
        empresa.SetEmail(email);
        empresa.Telefone = telefone;
        empresa.InscricaoEstadual = inscricaoEstadual;

        return empresa;
    }

    public void Atualizar(
        string razaoSocial,
        string nomeFantasia,
        DateTime dataAbertura,
        string email,
        string? telefone = null,
        string? inscricaoEstadual = null)
    {
        SetRazaoSocial(razaoSocial);
        SetNomeFantasia(nomeFantasia);
        SetDataAbertura(dataAbertura);
        SetEmail(email);
        Telefone = telefone;
        InscricaoEstadual = inscricaoEstadual;
        MarcarAtualizado();
    }

    public void AdicionarEndereco(Endereco endereco)
    {
        endereco.VincularPessoaJuridica(Id);
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

    // Regras de Negócio

    private void SetRazaoSocial(string razaoSocial)
    {
        if (string.IsNullOrWhiteSpace(razaoSocial))
            throw new DomainException("Razão Social é obrigatória.");

        if (razaoSocial.Length < 2 || razaoSocial.Length > 300)
            throw new DomainException("Razão Social deve ter entre 2 e 300 caracteres.");

        RazaoSocial = razaoSocial.Trim();
    }

    private void SetNomeFantasia(string nomeFantasia)
    {
        if (string.IsNullOrWhiteSpace(nomeFantasia))
            throw new DomainException("Nome Fantasia é obrigatório.");

        if (nomeFantasia.Length < 2 || nomeFantasia.Length > 300)
            throw new DomainException("Nome Fantasia deve ter entre 2 e 300 caracteres.");

        NomeFantasia = nomeFantasia.Trim();
    }

    private void SetDataAbertura(DateTime dataAbertura)
    {
        if (dataAbertura > DateTime.Today)
            throw new DomainException("Data de abertura não pode ser futura.");

        DataAbertura = dataAbertura;
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
