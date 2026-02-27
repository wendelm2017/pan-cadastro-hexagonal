using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using PanCadastro.Application.Services;
using PanCadastro.Domain.Entities;
using PanCadastro.Domain.Exceptions;
using PanCadastro.Domain.Ports.Out;

namespace PanCadastro.Application.Tests.Services;

// Testes unitários para a camada de serviço de Pessoa Jurídica, utilizando Moq para simular
// o repositório e o logger. Os testes cobrem os principais cenários de criação, 
// obtenção por ID, obtenção de todos, atualização e remoção de pessoas jurídicas.
//Uso o AAA (Arrange-Act-Assert) para estruturar os testes, 
//garantindo que cada teste seja claro e focado em um cenário específico.
public class PessoaJuridicaServiceTests
{
    private readonly Mock<IPessoaJuridicaRepository> _repositoryMock;
    private readonly Mock<ILogger<PessoaJuridicaService>> _loggerMock;
    private readonly PessoaJuridicaService _service;

    private const string CnpjValido = "11222333000181";
    private const string RazaoSocial = "Empresa Teste LTDA";
    private const string NomeFantasia = "Empresa Teste";
    private const string EmailValido = "contato@empresa.com";
    private static readonly DateTime DataAberturaValida = new(2020, 1, 15);

    public PessoaJuridicaServiceTests()
    {
        _repositoryMock = new Mock<IPessoaJuridicaRepository>();
        _loggerMock = new Mock<ILogger<PessoaJuridicaService>>();
        _service = new PessoaJuridicaService(_repositoryMock.Object, _loggerMock.Object);
    }

    // CRIAR 
    [Fact]
    public async Task CriarAsync_ComDadosValidos_DeveCriarERetornar()
    {
        _repositoryMock.Setup(r => r.CnpjExisteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _repositoryMock.Setup(r => r.AdicionarAsync(It.IsAny<PessoaJuridica>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((PessoaJuridica p, CancellationToken _) => p);

        var result = await _service.CriarAsync(RazaoSocial, NomeFantasia, CnpjValido, DataAberturaValida, EmailValido, "1133334444", "123456789");

        result.Should().NotBeNull();
        result.RazaoSocial.Should().Be(RazaoSocial);
        result.Cnpj.Numero.Should().Be(CnpjValido);
        _repositoryMock.Verify(r => r.AdicionarAsync(It.IsAny<PessoaJuridica>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CriarAsync_ComCnpjDuplicado_DeveLancarDomainException()
    {
        _repositoryMock.Setup(r => r.CnpjExisteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var act = () => _service.CriarAsync(RazaoSocial, NomeFantasia, CnpjValido, DataAberturaValida, EmailValido, null, null);

        await act.Should().ThrowAsync<DomainException>()
            .WithMessage("*já está cadastrado*");
        _repositoryMock.Verify(r => r.AdicionarAsync(It.IsAny<PessoaJuridica>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    // OBTER POR ID

    [Fact]
    public async Task ObterPorIdAsync_ComIdExistente_DeveRetornarEmpresa()
    {
        var empresa = PessoaJuridica.Criar(RazaoSocial, NomeFantasia, CnpjValido, DataAberturaValida, EmailValido);
        _repositoryMock.Setup(r => r.ObterComEnderecosAsync(empresa.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(empresa);

        var result = await _service.ObterPorIdAsync(empresa.Id);

        result.Should().NotBeNull();
        result.Id.Should().Be(empresa.Id);
    }

    [Fact]
    public async Task ObterPorIdAsync_ComIdInexistente_DeveLancarNotFoundException()
    {
        _repositoryMock.Setup(r => r.ObterComEnderecosAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((PessoaJuridica?)null);

        var act = () => _service.ObterPorIdAsync(Guid.NewGuid());

        await act.Should().ThrowAsync<NotFoundException>();
    }

    // OBTER TODOS

    [Fact]
    public async Task ObterTodosAsync_DeveRetornarLista()
    {
        var empresas = new List<PessoaJuridica>
        {
            PessoaJuridica.Criar("Empresa A LTDA", "Empresa A", "11222333000181", new DateTime(2020, 1, 1), "a@empresa.com"),
            PessoaJuridica.Criar("Empresa B SA", "Empresa B", "33014556000196", new DateTime(2019, 6, 1), "b@empresa.com")
        };
        _repositoryMock.Setup(r => r.ObterTodosAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(empresas);

        var result = await _service.ObterTodosAsync();

        result.Should().HaveCount(2);
    }

    // ATUALIZAR

    [Fact]
    public async Task AtualizarAsync_ComIdExistente_DeveAtualizarERetornar()
    {
        var empresa = PessoaJuridica.Criar(RazaoSocial, NomeFantasia, CnpjValido, DataAberturaValida, EmailValido);
        _repositoryMock.Setup(r => r.ObterPorIdAsync(empresa.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(empresa);

        var result = await _service.AtualizarAsync(empresa.Id, "Nova Razao LTDA", "Nova Fantasia", new DateTime(2021, 6, 1), "novo@empresa.com", null, null);

        result.RazaoSocial.Should().Be("Nova Razao LTDA");
        _repositoryMock.Verify(r => r.AtualizarAsync(It.IsAny<PessoaJuridica>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AtualizarAsync_ComIdInexistente_DeveLancarNotFoundException()
    {
        _repositoryMock.Setup(r => r.ObterPorIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((PessoaJuridica?)null);

        var act = () => _service.AtualizarAsync(Guid.NewGuid(), RazaoSocial, NomeFantasia, DataAberturaValida, EmailValido, null, null);

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
