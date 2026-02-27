using FluentAssertions;
using PanCadastro.Domain.Entities;
using PanCadastro.Domain.Exceptions;

namespace PanCadastro.Domain.Tests.Entities;
// Testes unitários para a entidade Endereço, cobrindo cenários de criação, 
//atualização e vinculação com pessoas físicas e jurídicas. 
//uso o FluentAssertions para asserções mais legíveis e expressivas, 
//garantindo que a lógica de negócio da entidade seja corretamente implementada 
//e que as validações sejam eficazes e estrutura AAA (Arrange-Act-Assert)
public class EnderecoTests
{
    [Fact]
    public void Criar_ComDadosValidos_DeveCriarComSucesso()
    {
        var endereco = Endereco.Criar("01001000", "Rua Teste", "100", "Centro", "Sao Paulo", "SP", "Apto 42");

        endereco.Should().NotBeNull();
        endereco.Id.Should().NotBeEmpty();
        endereco.Cep.Numero.Should().Be("01001000");
        endereco.Logradouro.Should().Be("Rua Teste");
        endereco.Numero.Should().Be("100");
        endereco.Bairro.Should().Be("Centro");
        endereco.Cidade.Should().Be("Sao Paulo");
        endereco.Estado.Should().Be("SP");
        endereco.Complemento.Should().Be("Apto 42");
        endereco.Ativo.Should().BeTrue();
    }

    [Fact]
    public void Criar_SemComplemento_DevePermitirNulo()
    {
        var endereco = Endereco.Criar("01001000", "Rua Teste", "100", "Centro", "Sao Paulo", "SP");

        endereco.Complemento.Should().BeNull();
    }

    [Fact]
    public void Criar_ComCepInvalido_DeveLancarDomainException()
    {
        var act = () => Endereco.Criar("123", "Rua Teste", "100", "Centro", "Sao Paulo", "SP");

        act.Should().Throw<DomainException>()
            .WithMessage("*CEP inválido*");
    }

    [Theory]
    [InlineData(null, "100", "Centro", "Sao Paulo", "SP")]
    [InlineData("Rua Teste", null, "Centro", "Sao Paulo", "SP")]
    [InlineData("Rua Teste", "100", null, "Sao Paulo", "SP")]
    [InlineData("Rua Teste", "100", "Centro", null, "SP")]
    [InlineData("Rua Teste", "100", "Centro", "Sao Paulo", null)]
    public void Criar_ComCampoObrigatorioNulo_DeveLancarException(
        string? logradouro, string? numero, string? bairro, string? cidade, string? estado)
    {
        var act = () => Endereco.Criar("01001000", logradouro!, numero!, bairro!, cidade!, estado!);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Atualizar_ComDadosValidos_DeveAtualizarComSucesso()
    {
        var endereco = Endereco.Criar("01001000", "Rua Teste", "100", "Centro", "Sao Paulo", "SP");

        endereco.Atualizar("02002000", "Av Nova", "200", "Jardins", "Sao Paulo", "SP", "Sala 1");

        endereco.Cep.Numero.Should().Be("02002000");
        endereco.Logradouro.Should().Be("Av Nova");
        endereco.Numero.Should().Be("200");
        endereco.AtualizadoEm.Should().NotBeNull();
    }

    [Fact]
    public void VincularPessoaFisica_DeveSetarIdELimparPJ()
    {
        var endereco = Endereco.Criar("01001000", "Rua Teste", "100", "Centro", "Sao Paulo", "SP");
        var pfId = Guid.NewGuid();

        endereco.VincularPessoaFisica(pfId);

        endereco.PessoaFisicaId.Should().Be(pfId);
        endereco.PessoaJuridicaId.Should().BeNull();
    }

    [Fact]
    public void VincularPessoaJuridica_DeveSetarIdELimparPF()
    {
        var endereco = Endereco.Criar("01001000", "Rua Teste", "100", "Centro", "Sao Paulo", "SP");
        var pjId = Guid.NewGuid();

        endereco.VincularPessoaJuridica(pjId);

        endereco.PessoaJuridicaId.Should().Be(pjId);
        endereco.PessoaFisicaId.Should().BeNull();
    }

    [Fact]
    public void PreencherPorViaCep_DevePreencherCamposNaoVazios()
    {
        var endereco = Endereco.Criar("01001000", "Original", "100", "Original", "Original", "SP");

        endereco.PreencherPorViaCep("Rua ViaCep", "Bairro ViaCep", "Cidade ViaCep", "RJ");

        endereco.Logradouro.Should().Be("Rua ViaCep");
        endereco.Bairro.Should().Be("Bairro ViaCep");
        endereco.Cidade.Should().Be("Cidade ViaCep");
        endereco.Estado.Should().Be("RJ");
    }

    [Fact]
    public void Criar_ComVinculoPF_DeveTerPessoaFisicaId()
    {
        var pfId = Guid.NewGuid();
        var endereco = Endereco.Criar("01001000", "Rua Teste", "100", "Centro", "Sao Paulo", "SP",
            pessoaFisicaId: pfId);

        endereco.PessoaFisicaId.Should().Be(pfId);
    }
}
