using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PanCadastro.Application.DTOs.Requests;
using PanCadastro.Application.DTOs.Responses;
using PanCadastro.Domain.Ports.In;

namespace PanCadastro.Adapters.Driving.Controllers;

// O controller de Endereços é responsável por expor as operações relacionadas a endereços via API REST.
// Ele recebe as requisições HTTP, valida os dados de entrada, e delega a lógica de negócio para o 
//serviço de endereços (IEnderecoService). O controller também usa AutoMapper para mapear entre os DTOs de request/response e as entidades do Domain. 
//Ele retorna respostas padronizadas usando ApiResponse<T> para consistência na API.
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class EnderecosController : ControllerBase
{
    private readonly IEnderecoService _service;
    private readonly IMapper _mapper;

    public EnderecosController(IEnderecoService service, IMapper mapper)
    {
        _service = service;
        _mapper = mapper;
    }

    // Obtém todos os endereços cadastrados.  
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<EnderecoResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ObterTodos(CancellationToken ct)
    {
        var enderecos = await _service.ObterTodosAsync(ct);
        var response = _mapper.Map<IEnumerable<EnderecoResponse>>(enderecos);
        return Ok(ApiResponse<IEnumerable<EnderecoResponse>>.Ok(response));
    }

    // Obtém um endereço por ID.        
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<EnderecoResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<EnderecoResponse>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterPorId(Guid id, CancellationToken ct)
    {
        var endereco = await _service.ObterPorIdAsync(id, ct);
        var response = _mapper.Map<EnderecoResponse>(endereco);
        return Ok(ApiResponse<EnderecoResponse>.Ok(response));
    }

    // Consulta CEP via ViaCEP API.
    [HttpGet("cep/{cep}")]
    [ProducesResponseType(typeof(ApiResponse<ViaCepResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<ViaCepResponseDto>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ConsultarCep(string cep, CancellationToken ct)
    {
        var viaCep = await _service.ConsultarCepAsync(cep, ct);

        if (viaCep is null)
            return NotFound(ApiResponse<ViaCepResponseDto>.Erro($"CEP {cep} não encontrado."));

        var response = _mapper.Map<ViaCepResponseDto>(viaCep);
        return Ok(ApiResponse<ViaCepResponseDto>.Ok(response));
    }

    // Obtém endereços relacionados a uma pessoa física.
    [HttpGet("pessoa-fisica/{pessoaFisicaId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<EnderecoResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ObterPorPessoaFisica(Guid pessoaFisicaId, CancellationToken ct)
    {
        var enderecos = await _service.ObterPorPessoaFisicaAsync(pessoaFisicaId, ct);
        var response = _mapper.Map<IEnumerable<EnderecoResponse>>(enderecos);
        return Ok(ApiResponse<IEnumerable<EnderecoResponse>>.Ok(response));
    }

    // Obtém endereços relacionados a uma pessoa jurídica.
    [HttpGet("pessoa-juridica/{pessoaJuridicaId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<EnderecoResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ObterPorPessoaJuridica(Guid pessoaJuridicaId, CancellationToken ct)
    {
        var enderecos = await _service.ObterPorPessoaJuridicaAsync(pessoaJuridicaId, ct);
        var response = _mapper.Map<IEnumerable<EnderecoResponse>>(enderecos);
        return Ok(ApiResponse<IEnumerable<EnderecoResponse>>.Ok(response));
    }

    // Cria um novo endereço. O request deve conter os dados do endereço e o ID da pessoa física ou jurídica relacionada.
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<EnderecoResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<EnderecoResponse>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Criar([FromBody] CriarEnderecoRequest request, CancellationToken ct)
    {
        var endereco = await _service.CriarAsync(
            request.Cep, request.Logradouro, request.Numero,
            request.Bairro, request.Cidade, request.Estado,
            request.Complemento, request.PessoaFisicaId,
            request.PessoaJuridicaId, ct);

        var response = _mapper.Map<EnderecoResponse>(endereco);
        return CreatedAtAction(nameof(ObterPorId), new { id = endereco.Id },
            ApiResponse<EnderecoResponse>.Ok(response, "Endereço criado com sucesso."));
    }

    // Atualiza um endereço existente. O request deve conter os dados do endereço a serem atualizados.
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<EnderecoResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<EnderecoResponse>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Atualizar(Guid id, [FromBody] AtualizarEnderecoRequest request, CancellationToken ct)
    {
        var endereco = await _service.AtualizarAsync(
            id, request.Cep, request.Logradouro, request.Numero,
            request.Bairro, request.Cidade, request.Estado,
            request.Complemento, ct);

        var response = _mapper.Map<EnderecoResponse>(endereco);
        return Ok(ApiResponse<EnderecoResponse>.Ok(response, "Endereço atualizado com sucesso."));
    }

    // Remove um endereço por ID.
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Remover(Guid id, CancellationToken ct)
    {
        await _service.RemoverAsync(id, ct);
        return NoContent();
    }
}
