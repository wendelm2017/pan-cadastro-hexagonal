using FluentAssertions;
using PanCadastro.Domain.Exceptions;
using PanCadastro.Domain.ValueObjects;

namespace PanCadastro.Domain.Tests.ValueObjects;
// Testes unitários para o Value Object CEP, 
//cobrindo cenários de criação com formatos válidos e inválidos,
public class CepTests
{
    [Theory]
    [InlineData("01001000")]
    [InlineData("01001-000")]
    public void Criar_ComCepValido_DeveCriarComSucesso(string cep)
    {
        var resultado = CEP.Criar(cep);

        resultado.Should().NotBeNull();
        resultado.Numero.Should().HaveLength(8);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("1234567")]
    [InlineData("123456789")]
    public void Criar_ComCepInvalido_DeveLancarDomainException(string? cep)
    {
        var act = () => CEP.Criar(cep!);

        act.Should().Throw<DomainException>()
            .WithMessage("*CEP inválido*");
    }

    [Fact]
    public void Formatado_DeveRetornarComMascara()
    {
        var cep = CEP.Criar("01001000");
        cep.Formatado.Should().Be("01001-000");
    }

    [Fact]
    public void Equals_ComMesmoNumero_DeveSerIgual()
    {
        var cep1 = CEP.Criar("01001000");
        var cep2 = CEP.Criar("01001-000");

        cep1.Should().Be(cep2);
        (cep1 == cep2).Should().BeTrue();
    }
}
