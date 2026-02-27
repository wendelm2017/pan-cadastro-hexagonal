using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using PanCadastro.Application.Services;
using PanCadastro.Domain.Entities;
using PanCadastro.Domain.Exceptions;
using PanCadastro.Domain.Ports.Out;

namespace PanCadastro.Application.Tests.Services;

//Aqui eu uso o padrão de testes AAA (Arrange-Act-Assert) para organizar
//cada teste de forma clara e consistente com o uso de Mocks para isolar o serviço
// e FluentAssertions para asserções mais legíveis.  
public class EnderecoServiceTests
{
    private readonly Mock<IEnderecoRepository> _repositoryMock;
    private readonly Mock<IViaCepClient> _viaCepMock;
    private readonly Mock<ILogger<EnderecoService>> _loggerMock;
    private readonly EnderecoService _service;

    public EnderecoServiceTests()
    {
        _repositoryMock = new Mock<IEnderecoRepository>();
        _viaCepMock = new Mock<IViaCepClient>();
        _loggerMock = new Mock<ILogger<EnderecoService>>();
        _service = new EnderecoService(_repositoryMock.Object, _viaCepMock.Object, _loggerMock.Object);
    }

    // CRIAR
    [Fact]
    public async Task CriarAsync_ComDadosValidos_DeveCriarERetornar()
    {
        _repositoryMock.Setup(r => r.AdicionarAsync(It.IsAny<Endereco>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Endereco e, CancellationToken _) => e);

        var result = await _service.CriarAsync("01001000", "Rua Teste", "100", "Centro", "Sao Paulo", "SP", "Apto 1", null, null);

        result.Should().NotBeNull();
        result.Cep.Numero.Should().Be("01001000");
        result.Logradouro.Should().Be("Rua Teste");
        _repositoryMock.Verify(r => r.AdicionarAsync(It.IsAny<Endereco>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CriarAsync_ComPessoaFisicaId_DeveVincular()
    {
        var pfId = Guid.NewGuid();
        _repositoryMock.Setup(r => r.AdicionarAsync(It.IsAny<Endereco>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Endereco e, CancellationToken _) => e);

        var result = await _service.CriarAsync("01001000", "Rua Teste", "100", "Centro", "Sao Paulo", "SP", null, pfId, null);

        result.PessoaFisicaId.Should().Be(pfId);
    }

    // OBTER POR ID 

    [Fact]
    public async Task ObterPorIdAsync_ComIdExistente_DeveRetornarEndereco()
    {
        var endereco = Endereco.Criar("01001000", "Rua Teste", "100", "Centro", "Sao Paulo", "SP");
        _repositoryMock.Setup(r => r.ObterPorIdAsync(endereco.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(endereco);

        var result = await _service.ObterPorIdAsync(endereco.Id);

        result.Should().NotBeNull();
        result.Id.Should().Be(endereco.Id);
    }

    [Fact]
    public async Task ObterPorIdAsync_ComIdInexistente_DeveLancarNotFoundException()
    {
        _repositoryMock.Setup(r => r.ObterPorIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Endereco?)null);

        var act = () => _service.ObterPorIdAsync(Guid.NewGuid());

        await act.Should().ThrowAsync<NotFoundException>();
    }

    // OBTER POR PESSOA

    [Fact]
    public async Task ObterPorPessoaFisicaAsync_DeveRetornarEnderecos()
    {
        var pfId = Guid.NewGuid();
        var enderecos = new List<Endereco>
        {
            Endereco.Criar("01001000", "Rua A", "100", "Centro", "SP", "SP"),
            Endereco.Criar("02002000", "Rua B", "200", "Jardins", "SP", "SP")
        };
        _repositoryMock.Setup(r => r.ObterPorPessoaFisicaAsync(pfId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(enderecos);

        var result = await _service.ObterPorPessoaFisicaAsync(pfId);

        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task ObterPorPessoaJuridicaAsync_DeveRetornarEnderecos()
    {
        var pjId = Guid.NewGuid();
        _repositoryMock.Setup(r => r.ObterPorPessoaJuridicaAsync(pjId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Endereco>());

        var result = await _service.ObterPorPessoaJuridicaAsync(pjId);

        result.Should().BeEmpty();
    }

    // ATUALIZAR

    [Fact]
    public async Task AtualizarAsync_ComIdExistente_DeveAtualizarERetornar()
    {
        var endereco = Endereco.Criar("01001000", "Rua Teste", "100", "Centro", "Sao Paulo", "SP");
        _repositoryMock.Setup(r => r.ObterPorIdAsync(endereco.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(endereco);

        var result = await _service.AtualizarAsync(endereco.Id, "02002000", "Av Nova", "200", "Jardins", "Sao Paulo", "SP", "Sala 1");

        result.Logradouro.Should().Be("Av Nova");
        result.Cep.Numero.Should().Be("02002000");
        _repositoryMock.Verify(r => r.AtualizarAsync(It.IsAny<Endereco>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AtualizarAsync_ComIdInexistente_DeveLancarNotFoundException()
    {
        _repositoryMock.Setup(r => r.ObterPorIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Endereco?)null);

        var act = () => _service.AtualizarAsync(Guid.NewGuid(), "01001000", "Rua", "1", "B", "C", "SP", null);

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
    }

    // CONSULTAR CEP (ViaCEP)

    [Fact]
    public async Task ConsultarCepAsync_ComCepExistente_DeveRetornarDados()
    {
        var viaCepResponse = new ViaCepResponse("01001-000", "Praca da Se", "", "Se", "Sao Paulo", "SP");
        _viaCepMock.Setup(v => v.ConsultarCepAsync("01001000", It.IsAny<CancellationToken>()))
            .ReturnsAsync(viaCepResponse);

        var result = await _service.ConsultarCepAsync("01001000");

        result.Should().NotBeNull();
        result!.Localidade.Should().Be("Sao Paulo");
        result.Uf.Should().Be("SP");
    }

    [Fact]
    public async Task ConsultarCepAsync_ComCepInexistente_DeveRetornarNull()
    {
        _viaCepMock.Setup(v => v.ConsultarCepAsync("99999999", It.IsAny<CancellationToken>()))
            .ReturnsAsync((ViaCepResponse?)null);

        var result = await _service.ConsultarCepAsync("99999999");

        result.Should().BeNull();
    }

    [Fact]
    public async Task ConsultarCepAsync_ComErroViaCep_DeveRetornarNull()
    {
        var viaCepErro = new ViaCepResponse("", "", "", "", "", "", Erro: true);
        _viaCepMock.Setup(v => v.ConsultarCepAsync("00000000", It.IsAny<CancellationToken>()))
            .ReturnsAsync(viaCepErro);

        var result = await _service.ConsultarCepAsync("00000000");

        result.Should().BeNull();
    }
}
