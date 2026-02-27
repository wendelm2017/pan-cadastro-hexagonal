using FluentAssertions;
using PanCadastro.Domain.Entities;
using PanCadastro.Domain.Exceptions;

namespace PanCadastro.Domain.Tests.Entities;
// Testes unitários para a entidade Pessoa Física, cobrindo cenários de criação, 
//atualização e desativação. 
//uso o FluentAssertions para asserções mais legíveis e expressivas, 
//garantindo que a lógica de negócio da entidade seja corretamente implementada 
//e que as validações sejam eficazes e estrutura AAA (Arrange-Act-Assert)

public class PessoaFisicaTests
{
    private const string CpfValido = "52998224725";
    private const string NomeValido = "Wendel Machado";
    private const string EmailValido = "wendel@email.com";
    private static readonly DateTime DataNascimentoValida = new(1974, 5, 15);

    [Fact]
    public void Criar_ComDadosValidos_DeveCriarComSucesso()
    {
        var pessoa = PessoaFisica.Criar(NomeValido, CpfValido, DataNascimentoValida, EmailValido, "11999998888");

        pessoa.Should().NotBeNull();
        pessoa.Id.Should().NotBeEmpty();
        pessoa.Nome.Should().Be(NomeValido);
        pessoa.Cpf.Numero.Should().Be(CpfValido);
        pessoa.DataNascimento.Should().Be(DataNascimentoValida);
        pessoa.Email.Should().Be("wendel@email.com");
        pessoa.Telefone.Should().Be("11999998888");
        pessoa.Ativo.Should().BeTrue();
        pessoa.CriadoEm.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Criar_ComNomeVazio_DeveLancarDomainException(string? nome)
    {
        var act = () => PessoaFisica.Criar(nome!, CpfValido, DataNascimentoValida, EmailValido);

        act.Should().Throw<DomainException>()
            .WithMessage("*Nome*obrigatório*");
    }

    [Fact]
    public void Criar_ComNomeCurto_DeveLancarDomainException()
    {
        var act = () => PessoaFisica.Criar("A", CpfValido, DataNascimentoValida, EmailValido);

        act.Should().Throw<DomainException>()
            .WithMessage("*Nome*entre 2 e 200*");
    }

    [Fact]
    public void Criar_ComCpfInvalido_DeveLancarDomainException()
    {
        var act = () => PessoaFisica.Criar(NomeValido, "11111111111", DataNascimentoValida, EmailValido);

        act.Should().Throw<DomainException>()
            .WithMessage("*CPF inválido*");
    }

    [Fact]
    public void Criar_ComDataNascimentoFutura_DeveLancarDomainException()
    {
        var act = () => PessoaFisica.Criar(NomeValido, CpfValido, DateTime.Today.AddDays(1), EmailValido);

        act.Should().Throw<DomainException>()
            .WithMessage("*Data de nascimento*anterior*");
    }

    [Theory]
    [InlineData("")]
    [InlineData("emailsemaroba")]
    [InlineData("email@")]
    public void Criar_ComEmailInvalido_DeveLancarDomainException(string email)
    {
        var act = () => PessoaFisica.Criar(NomeValido, CpfValido, DataNascimentoValida, email);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Atualizar_ComDadosValidos_DeveAtualizarComSucesso()
    {
        var pessoa = PessoaFisica.Criar(NomeValido, CpfValido, DataNascimentoValida, EmailValido);

        pessoa.Atualizar("Novo Nome", new DateTime(1980, 1, 1), "novo@email.com", "11988887777");

        pessoa.Nome.Should().Be("Novo Nome");
        pessoa.Email.Should().Be("novo@email.com");
        pessoa.AtualizadoEm.Should().NotBeNull();
    }

    [Fact]
    public void Desativar_DeveMarcarComoInativo()
    {
        var pessoa = PessoaFisica.Criar(NomeValido, CpfValido, DataNascimentoValida, EmailValido);

        pessoa.Desativar();

        pessoa.Ativo.Should().BeFalse();
        pessoa.AtualizadoEm.Should().NotBeNull();
    }

    [Fact]
    public void Ativar_AposDesativar_DeveReativar()
    {
        var pessoa = PessoaFisica.Criar(NomeValido, CpfValido, DataNascimentoValida, EmailValido);
        pessoa.Desativar();

        pessoa.Ativar();

        pessoa.Ativo.Should().BeTrue();
    }

    [Fact]
    public void AdicionarEndereco_DeveVincularEAdicionar()
    {
        var pessoa = PessoaFisica.Criar(NomeValido, CpfValido, DataNascimentoValida, EmailValido);
        var endereco = Endereco.Criar("01001000", "Rua Teste", "100", "Centro", "Sao Paulo", "SP");

        pessoa.AdicionarEndereco(endereco);

        pessoa.Enderecos.Should().HaveCount(1);
        pessoa.Enderecos.First().PessoaFisicaId.Should().Be(pessoa.Id);
    }

    [Fact]
    public void RemoverEndereco_ComIdInexistente_DeveLancarNotFoundException()
    {
        var pessoa = PessoaFisica.Criar(NomeValido, CpfValido, DataNascimentoValida, EmailValido);

        var act = () => pessoa.RemoverEndereco(Guid.NewGuid());

        act.Should().Throw<NotFoundException>();
    }
}
