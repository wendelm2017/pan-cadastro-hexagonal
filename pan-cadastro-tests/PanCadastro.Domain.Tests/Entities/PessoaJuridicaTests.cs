using FluentAssertions;
using PanCadastro.Domain.Entities;
using PanCadastro.Domain.Exceptions;

namespace PanCadastro.Domain.Tests.Entities;

// Testes unitários para a entidade Pessoa Jurídica, cobrindo cenários de criação, 
//atualização e desativação. 
//uso o FluentAssertions para asserções mais legíveis e expressivas, 
//garantindo que a lógica de negócio da entidade seja corretamente implementada 
//e que as validações sejam eficazes e estrutura AAA (Arrange-Act-Assert)

public class PessoaJuridicaTests
{
    private const string CnpjValido = "11222333000181";
    private const string RazaoSocialValida = "Empresa Teste LTDA";
    private const string NomeFantasiaValido = "Empresa Teste";
    private const string EmailValido = "contato@empresa.com";
    private static readonly DateTime DataAberturaValida = new(2020, 1, 15);

    [Fact]
    public void Criar_ComDadosValidos_DeveCriarComSucesso()
    {
        var empresa = PessoaJuridica.Criar(
            RazaoSocialValida, NomeFantasiaValido, CnpjValido,
            DataAberturaValida, EmailValido, "1133334444", "123456789");

        empresa.Should().NotBeNull();
        empresa.Id.Should().NotBeEmpty();
        empresa.RazaoSocial.Should().Be(RazaoSocialValida);
        empresa.NomeFantasia.Should().Be(NomeFantasiaValido);
        empresa.Cnpj.Numero.Should().Be(CnpjValido);
        empresa.DataAbertura.Should().Be(DataAberturaValida);
        empresa.Email.Should().Be("contato@empresa.com");
        empresa.InscricaoEstadual.Should().Be("123456789");
        empresa.Ativo.Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Criar_ComRazaoSocialVazia_DeveLancarDomainException(string? razaoSocial)
    {
        var act = () => PessoaJuridica.Criar(razaoSocial!, NomeFantasiaValido, CnpjValido, DataAberturaValida, EmailValido);

        act.Should().Throw<DomainException>()
            .WithMessage("*Razão Social*obrigatória*");
    }

    [Fact]
    public void Criar_ComCnpjInvalido_DeveLancarDomainException()
    {
        var act = () => PessoaJuridica.Criar(RazaoSocialValida, NomeFantasiaValido, "11111111111111", DataAberturaValida, EmailValido);

        act.Should().Throw<DomainException>()
            .WithMessage("*CNPJ inválido*");
    }

    [Fact]
    public void Criar_ComDataAberturaFutura_DeveLancarDomainException()
    {
        var act = () => PessoaJuridica.Criar(RazaoSocialValida, NomeFantasiaValido, CnpjValido, DateTime.Today.AddDays(1), EmailValido);

        act.Should().Throw<DomainException>()
            .WithMessage("*Data de abertura*futura*");
    }

    [Fact]
    public void Atualizar_ComDadosValidos_DeveAtualizarComSucesso()
    {
        var empresa = PessoaJuridica.Criar(RazaoSocialValida, NomeFantasiaValido, CnpjValido, DataAberturaValida, EmailValido);

        empresa.Atualizar("Nova Razao Social LTDA", "Nova Fantasia", new DateTime(2021, 6, 1), "novo@empresa.com");

        empresa.RazaoSocial.Should().Be("Nova Razao Social LTDA");
        empresa.NomeFantasia.Should().Be("Nova Fantasia");
        empresa.AtualizadoEm.Should().NotBeNull();
    }

    [Fact]
    public void AdicionarEndereco_DeveVincularEAdicionar()
    {
        var empresa = PessoaJuridica.Criar(RazaoSocialValida, NomeFantasiaValido, CnpjValido, DataAberturaValida, EmailValido);
        var endereco = Endereco.Criar("01001000", "Av Paulista", "1000", "Bela Vista", "Sao Paulo", "SP");

        empresa.AdicionarEndereco(endereco);

        empresa.Enderecos.Should().HaveCount(1);
        empresa.Enderecos.First().PessoaJuridicaId.Should().Be(empresa.Id);
    }

    [Fact]
    public void RemoverEndereco_ComIdInexistente_DeveLancarNotFoundException()
    {
        var empresa = PessoaJuridica.Criar(RazaoSocialValida, NomeFantasiaValido, CnpjValido, DataAberturaValida, EmailValido);

        var act = () => empresa.RemoverEndereco(Guid.NewGuid());

        act.Should().Throw<NotFoundException>();
    }
}
