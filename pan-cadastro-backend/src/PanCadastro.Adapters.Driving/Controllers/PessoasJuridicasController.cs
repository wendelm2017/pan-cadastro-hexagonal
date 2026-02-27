using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PanCadastro.Application.DTOs.Requests;
using PanCadastro.Application.DTOs.Responses;
using PanCadastro.Domain.Ports.In;

namespace PanCadastro.Adapters.Driving.Controllers;

// O controller de Pessoas Jurídicas é responsável por expor as operações relacionadas a pessoas jurídicas via API REST.
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class PessoasJuridicasController : ControllerBase
{
    private readonly IPessoaJuridicaService _service;
    private readonly IMapper _mapper;

    public PessoasJuridicasController(IPessoaJuridicaService service, IMapper mapper)
    {
        _service = service;
        _mapper = mapper;
    }

    // Lista todas as Pessoas Jurídicas.
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<PessoaJuridicaResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ObterTodos(CancellationToken ct)
    {
        var empresas = await _service.ObterTodosAsync(ct);
        var response = _mapper.Map<IEnumerable<PessoaJuridicaResponse>>(empresas);
        return Ok(ApiResponse<IEnumerable<PessoaJuridicaResponse>>.Ok(response));
    }

    // Obtém uma Pessoa Jurídica por ID.
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<PessoaJuridicaResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<PessoaJuridicaResponse>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterPorId(Guid id, CancellationToken ct)
    {
        var empresa = await _service.ObterPorIdAsync(id, ct);
        var response = _mapper.Map<PessoaJuridicaResponse>(empresa);
        return Ok(ApiResponse<PessoaJuridicaResponse>.Ok(response));
    }

    // Cria uma nova Pessoa Jurídica.
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<PessoaJuridicaResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<PessoaJuridicaResponse>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Criar([FromBody] CriarPessoaJuridicaRequest request, CancellationToken ct)
    {
        var empresa = await _service.CriarAsync(
            request.RazaoSocial, request.NomeFantasia, request.Cnpj,
            request.DataAbertura, request.Email, request.Telefone,
            request.InscricaoEstadual, ct);

        var response = _mapper.Map<PessoaJuridicaResponse>(empresa);
        return CreatedAtAction(nameof(ObterPorId), new { id = empresa.Id },
            ApiResponse<PessoaJuridicaResponse>.Ok(response, "Pessoa Jurídica criada com sucesso."));
    }

    // Atualiza uma Pessoa Jurídica existente.
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<PessoaJuridicaResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<PessoaJuridicaResponse>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Atualizar(Guid id, [FromBody] AtualizarPessoaJuridicaRequest request, CancellationToken ct)
    {
        var empresa = await _service.AtualizarAsync(
            id, request.RazaoSocial, request.NomeFantasia,
            request.DataAbertura, request.Email, request.Telefone,
            request.InscricaoEstadual, ct);

        var response = _mapper.Map<PessoaJuridicaResponse>(empresa);
        return Ok(ApiResponse<PessoaJuridicaResponse>.Ok(response, "Pessoa Jurídica atualizada com sucesso."));
    }

    // Remove uma Pessoa Jurídica.
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Remover(Guid id, CancellationToken ct)
    {
        await _service.RemoverAsync(id, ct);
        return NoContent();
    }
}
