using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PanCadastro.Application.DTOs.Requests;
using PanCadastro.Application.DTOs.Responses;
using PanCadastro.Domain.Ports.In;

namespace PanCadastro.Adapters.Driving.Controllers;

// CRUD de Pessoa Física
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class PessoasFisicasController : ControllerBase
{
    private readonly IPessoaFisicaService _service;
    private readonly IMapper _mapper;

    public PessoasFisicasController(IPessoaFisicaService service, IMapper mapper)
    {
        _service = service;
        _mapper = mapper;
    }

    // Lista todas as Pessoas Físicas.
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<PessoaFisicaResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ObterTodos(CancellationToken ct)
    {
        var pessoas = await _service.ObterTodosAsync(ct);
        var response = _mapper.Map<IEnumerable<PessoaFisicaResponse>>(pessoas);
        return Ok(ApiResponse<IEnumerable<PessoaFisicaResponse>>.Ok(response));
    }

    // Obtém uma Pessoa Física por ID.
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<PessoaFisicaResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<PessoaFisicaResponse>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterPorId(Guid id, CancellationToken ct)
    {
        var pessoa = await _service.ObterPorIdAsync(id, ct);
        var response = _mapper.Map<PessoaFisicaResponse>(pessoa);
        return Ok(ApiResponse<PessoaFisicaResponse>.Ok(response));
    }

    // Cria uma nova Pessoa Física.
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<PessoaFisicaResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<PessoaFisicaResponse>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Criar([FromBody] CriarPessoaFisicaRequest request, CancellationToken ct)
    {
        var pessoa = await _service.CriarAsync(
            request.Nome, request.Cpf, request.DataNascimento,
            request.Email, request.Telefone, ct);

        var response = _mapper.Map<PessoaFisicaResponse>(pessoa);
        return CreatedAtAction(nameof(ObterPorId), new { id = pessoa.Id },
            ApiResponse<PessoaFisicaResponse>.Ok(response, "Pessoa Física criada com sucesso."));
    }

    // Atualiza uma Pessoa Física existente.
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<PessoaFisicaResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<PessoaFisicaResponse>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Atualizar(Guid id, [FromBody] AtualizarPessoaFisicaRequest request, CancellationToken ct)
    {
        var pessoa = await _service.AtualizarAsync(
            id, request.Nome, request.DataNascimento,
            request.Email, request.Telefone, ct);

        var response = _mapper.Map<PessoaFisicaResponse>(pessoa);
        return Ok(ApiResponse<PessoaFisicaResponse>.Ok(response, "Pessoa Física atualizada com sucesso."));
    }

    // Remove uma Pessoa Física.
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Remover(Guid id, CancellationToken ct)
    {
        await _service.RemoverAsync(id, ct);
        return NoContent();
    }
}
