using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using PanCadastro.Application.Services;
using PanCadastro.Domain.Entities;
using PanCadastro.Domain.Exceptions;
using PanCadastro.Domain.Ports.Out;

namespace PanCadastro.Application.Tests.Services;
// Testes unitários para a camada de serviço de Pessoa Física, utilizando Moq para simular
// o repositório e o logger. Os testes cobrem os principais cenários de criação, 
// obtenção por ID, obtenção de todos, atualização e remoção de pessoas físicas.
//Uso o AAA (Arrange-Act-Assert) para estruturar os testes, 
//garantindo que cada teste seja claro e focado em um cenário específico.
public class PessoaFisicaServiceTests
{
    private readonly Mock<IPessoaFisicaRepository> _repositoryMock;
    private readonly Mock<ILogger<PessoaFisicaService>> _loggerMock;
    private readonly PessoaFisicaService _service;

    private const string CpfValido = "52998224725";
    private const string NomeValido = "Wendel Machado";
    private const string EmailValido = "wendel@email.com";
    private static readonly DateTime DataNascimentoValida = new(1974, 5, 15);

    public PessoaFisicaServiceTests()
    {
        _repositoryMock = new Mock<IPessoaFisicaRepository>();
        _loggerMock = new Mock<ILogger<PessoaFisicaService>>();
        _service = new PessoaFisicaService(_repositoryMock.Object, _loggerMock.Object);
    }

    // CRIAR 
    [Fact]
    public async Task CriarAsync_ComDadosValidos_DeveCriarERetornar()
    {
        _repositoryMock.Setup(r => r.CpfExisteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _repositoryMock.Setup(r => r.AdicionarAsync(It.IsAny<PessoaFisica>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((PessoaFisica p, CancellationToken _) => p);

        var result = await _service.CriarAsync(NomeValido, CpfValido, DataNascimentoValida, EmailValido, "11999998888");

        result.Should().NotBeNull();
        result.Nome.Should().Be(NomeValido);
        result.Cpf.Numero.Should().Be(CpfValido);
        _repositoryMock.Verify(r => r.AdicionarAsync(It.IsAny<PessoaFisica>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CriarAsync_ComCpfDuplicado_DeveLancarDomainException()
    {
        _repositoryMock.Setup(r => r.CpfExisteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var act = () => _service.CriarAsync(NomeValido, CpfValido, DataNascimentoValida, EmailValido, null);

        await act.Should().ThrowAsync<DomainException>()
            .WithMessage("*já está cadastrado*");
        _repositoryMock.Verify(r => r.AdicionarAsync(It.IsAny<PessoaFisica>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    // OBTER POR ID

    [Fact]
    public async Task ObterPorIdAsync_ComIdExistente_DeveRetornarPessoa()
    {
        var pessoa = PessoaFisica.Criar(NomeValido, CpfValido, DataNascimentoValida, EmailValido);
        _repositoryMock.Setup(r => r.ObterComEnderecosAsync(pessoa.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pessoa);

        var result = await _service.ObterPorIdAsync(pessoa.Id);

        result.Should().NotBeNull();
        result.Id.Should().Be(pessoa.Id);
    }

    [Fact]
    public async Task ObterPorIdAsync_ComIdInexistente_DeveLancarNotFoundException()
    {
        _repositoryMock.Setup(r => r.ObterComEnderecosAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((PessoaFisica?)null);

        var act = () => _service.ObterPorIdAsync(Guid.NewGuid());

        await act.Should().ThrowAsync<NotFoundException>();
    }

    // OBTER TODOS

    [Fact]
    public async Task ObterTodosAsync_DeveRetornarLista()
    {
        var pessoas = new List<PessoaFisica>
        {
            PessoaFisica.Criar("Pessoa Um", "52998224725", new DateTime(1990, 1, 1), "um@email.com"),
            PessoaFisica.Criar("Pessoa Dois", "39053344705", new DateTime(1985, 6, 15), "dois@email.com")
        };
        _repositoryMock.Setup(r => r.ObterTodosAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(pessoas);

        var result = await _service.ObterTodosAsync();

        result.Should().HaveCount(2);
    }

    // ATUALIZAR

    [Fact]
    public async Task AtualizarAsync_ComIdExistente_DeveAtualizarERetornar()
    {
        var pessoa = PessoaFisica.Criar(NomeValido, CpfValido, DataNascimentoValida, EmailValido);
        _repositoryMock.Setup(r => r.ObterPorIdAsync(pessoa.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pessoa);

        var result = await _service.AtualizarAsync(pessoa.Id, "Novo Nome", new DateTime(1980, 1, 1), "novo@email.com", "11988887777");

        result.Nome.Should().Be("Novo Nome");
        result.Email.Should().Be("novo@email.com");
        _repositoryMock.Verify(r => r.AtualizarAsync(It.IsAny<PessoaFisica>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AtualizarAsync_ComIdInexistente_DeveLancarNotFoundException()
    {
        _repositoryMock.Setup(r => r.ObterPorIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((PessoaFisica?)null);

        var act = () => _service.AtualizarAsync(Guid.NewGuid(), "Nome", DateTime.Today.AddYears(-30), "email@test.com", null);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    // REMOVER

    [Fact]
    public async Task RemoverAsync_ComIdExistente_DeveRemover()
    {
        var id = Guid.NewGuid();
        _repositoryMock.Setup(r => r.ExisteAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        await _service.RemoverAsync(id);

        _repositoryMock.Verify(r => r.RemoverAsync(id, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RemoverAsync_ComIdInexistente_DeveLancarNotFoundException()
    {
        _repositoryMock.Setup(r => r.ExisteAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var act = () => _service.RemoverAsync(Guid.NewGuid());

        await act.Should().ThrowAsync<NotFoundException>();
        _repositoryMock.Verify(r => r.RemoverAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
