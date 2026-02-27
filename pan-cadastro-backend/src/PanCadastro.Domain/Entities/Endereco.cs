using PanCadastro.Domain.ValueObjects;

namespace PanCadastro.Domain.Entities;

//Essa e a entidade de Endereço onde eu herdo a classe BaseEntity.
//Ela também tem chaves estrangeiras para vincular a uma pessoa física ou jurídica,
// permitindo flexibilidade no modelo. O uso de Value Object para o CEP garante 
//que o formato do CEP seja sempre válido. Apliuco o principio SOLID de responsabilidade única
public class Endereco : BaseEntity
{
    public CEP Cep { get; private set; } = null!;
    public string Logradouro { get; private set; } = string.Empty;
    public string Numero { get; private set; } = string.Empty;
    public string? Complemento { get; private set; }
    public string Bairro { get; private set; } = string.Empty;
    public string Cidade { get; private set; } = string.Empty;
    public string Estado { get; private set; } = string.Empty;

    // FK — pode pertencer a PF ou PJ (nullable)
    public Guid? PessoaFisicaId { get; private set; }
    public Guid? PessoaJuridicaId { get; private set; }

    protected Endereco() { }

    public static Endereco Criar(
        string cep,
        string logradouro,
        string numero,
        string bairro,
        string cidade,
        string estado,
        string? complemento = null,
        Guid? pessoaFisicaId = null,
        Guid? pessoaJuridicaId = null)
    {
        var endereco = new Endereco
        {
            Cep = CEP.Criar(cep),
            Logradouro = logradouro ?? throw new ArgumentNullException(nameof(logradouro)),
            Numero = numero ?? throw new ArgumentNullException(nameof(numero)),
            Bairro = bairro ?? throw new ArgumentNullException(nameof(bairro)),
            Cidade = cidade ?? throw new ArgumentNullException(nameof(cidade)),
            Estado = estado ?? throw new ArgumentNullException(nameof(estado)),
            Complemento = complemento,
            PessoaFisicaId = pessoaFisicaId,
            PessoaJuridicaId = pessoaJuridicaId
        };

        return endereco;
    }

    public void Atualizar(
        string cep,
        string logradouro,
        string numero,
        string bairro,
        string cidade,
        string estado,
        string? complemento = null)
    {
        Cep = CEP.Criar(cep);
        Logradouro = logradouro ?? throw new ArgumentNullException(nameof(logradouro));
        Numero = numero ?? throw new ArgumentNullException(nameof(numero));
        Bairro = bairro ?? throw new ArgumentNullException(nameof(bairro));
        Cidade = cidade ?? throw new ArgumentNullException(nameof(cidade));
        Estado = estado ?? throw new ArgumentNullException(nameof(estado));
        Complemento = complemento;
        MarcarAtualizado();
    }

    // Preenche endereço a partir dos dados do ViaCEP.
    public void PreencherPorViaCep(string logradouro, string bairro, string cidade, string estado)
    {
        if (!string.IsNullOrWhiteSpace(logradouro)) Logradouro = logradouro;
        if (!string.IsNullOrWhiteSpace(bairro)) Bairro = bairro;
        if (!string.IsNullOrWhiteSpace(cidade)) Cidade = cidade;
        if (!string.IsNullOrWhiteSpace(estado)) Estado = estado;
        MarcarAtualizado();
    }

    public void VincularPessoaFisica(Guid pessoaFisicaId)
    {
        PessoaFisicaId = pessoaFisicaId;
        PessoaJuridicaId = null;
        MarcarAtualizado();
    }

    public void VincularPessoaJuridica(Guid pessoaJuridicaId)
    {
        PessoaJuridicaId = pessoaJuridicaId;
        PessoaFisicaId = null;
        MarcarAtualizado();
    }
}
