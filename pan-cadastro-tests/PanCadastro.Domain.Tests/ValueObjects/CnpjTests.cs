using FluentAssertions;
using PanCadastro.Domain.Exceptions;
using PanCadastro.Domain.ValueObjects;

namespace PanCadastro.Domain.Tests.ValueObjects;

// Testes unitários para o Value Object CNPJ, cobrindo cenários de criação com formatos válidos e inválidos,
// validação de CNPJ e formatação. Uso o FluentAssertions para asserções mais legíveis e expressivas, garantindo que a lógica de negócio do Value Object seja corretamente implementada
public class CnpjTests
{
    [Theory]
    [InlineData("11222333000181")]
    [InlineData("11.222.333/0001-81")]
    public void Criar_ComCnpjValido_DeveCriarComSucesso(string cnpj)
    {
        var resultado = CNPJ.Criar(cnpj);

        resultado.Should().NotBeNull();
        resultado.Numero.Should().HaveLength(14);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("1234567890123")]
    [InlineData("123456789012345")]
    [InlineData("11111111111111")]
    [InlineData("11222333000182")]
    public void Criar_ComCnpjInvalido_DeveLancarDomainException(string? cnpj)
    {
        var act = () => CNPJ.Criar(cnpj!);

        act.Should().Throw<DomainException>()
            .WithMessage("*CNPJ inválido*");
    }

    [Fact]
    public void Formatado_DeveRetornarComMascara()
    {
        var cnpj = CNPJ.Criar("11222333000181");

        cnpj.Formatado.Should().Be("11.222.333/0001-81");
    }

    [Fact]
    public void Equals_ComMesmoNumero_DeveSerIgual()
    {
        var cnpj1 = CNPJ.Criar("11222333000181");
        var cnpj2 = CNPJ.Criar("11.222.333/0001-81");

        cnpj1.Should().Be(cnpj2);
        (cnpj1 == cnpj2).Should().BeTrue();
    }

    [Fact]
    public void Validar_ComCnpjValido_DeveRetornarTrue()
    {
        CNPJ.Validar("11222333000181").Should().BeTrue();
    }

    [Fact]
    public void Validar_ComCnpjInvalido_DeveRetornarFalse()
    {
        CNPJ.Validar("11111111111111").Should().BeFalse();
        CNPJ.Validar("").Should().BeFalse();
    }
}
