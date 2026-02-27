using FluentAssertions;
using PanCadastro.Domain.Exceptions;
using PanCadastro.Domain.ValueObjects;

namespace PanCadastro.Domain.Tests.ValueObjects;

// Testes unitários para o Value Object CPF, cobrindo cenários de criação com formatos válidos e inválidos,
// validação de CPF e formatação. Uso o FluentAssertions para asserções mais legíveis e expressivas, garantindo que a lógica de negócio do Value Object seja corretamente implementada
public class CpfTests
{
    [Theory]
    [InlineData("52998224725")]
    [InlineData("529.982.247-25")]
    [InlineData("39053344705")]
    public void Criar_ComCpfValido_DeveCriarComSucesso(string cpf)
    {
        var resultado = CPF.Criar(cpf);

        resultado.Should().NotBeNull();
        resultado.Numero.Should().HaveLength(11);
        resultado.Numero.Should().MatchRegex(@"^\d{11}$");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    [InlineData("1234567890")]
    [InlineData("123456789012")]
    [InlineData("11111111111")]
    [InlineData("22222222222")]
    [InlineData("00000000000")]
    [InlineData("52998224724")]
    [InlineData("abcdefghijk")]
    public void Criar_ComCpfInvalido_DeveLancarDomainException(string? cpf)
    {
        var act = () => CPF.Criar(cpf!);

        act.Should().Throw<DomainException>()
            .WithMessage("*CPF inválido*");
    }

    [Fact]
    public void Formatado_DeveRetornarComMascara()
    {
        var cpf = CPF.Criar("52998224725");

        cpf.Formatado.Should().Be("529.982.247-25");
    }

    [Fact]
    public void Equals_ComMesmoNumero_DeveSerIgual()
    {
        var cpf1 = CPF.Criar("52998224725");
        var cpf2 = CPF.Criar("529.982.247-25");

        cpf1.Should().Be(cpf2);
        (cpf1 == cpf2).Should().BeTrue();
        cpf1.GetHashCode().Should().Be(cpf2.GetHashCode());
    }

    [Fact]
    public void Equals_ComNumerosDiferentes_NaoDeveSerIgual()
    {
        var cpf1 = CPF.Criar("52998224725");
        var cpf2 = CPF.Criar("39053344705");

        cpf1.Should().NotBe(cpf2);
        (cpf1 != cpf2).Should().BeTrue();
    }

    [Fact]
    public void Validar_ComCpfValido_DeveRetornarTrue()
    {
        CPF.Validar("52998224725").Should().BeTrue();
        CPF.Validar("39053344705").Should().BeTrue();
    }

    [Fact]
    public void Validar_ComCpfInvalido_DeveRetornarFalse()
    {
        CPF.Validar("11111111111").Should().BeFalse();
        CPF.Validar("12345").Should().BeFalse();
        CPF.Validar("").Should().BeFalse();
    }

    [Fact]
    public void ToString_DeveRetornarFormatado()
    {
        var cpf = CPF.Criar("52998224725");
        cpf.ToString().Should().Be("529.982.247-25");
    }
}
