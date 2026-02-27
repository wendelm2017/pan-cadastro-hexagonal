using System.Net;
using System.Text.Json;
using PanCadastro.Application.DTOs.Responses;
using PanCadastro.Domain.Exceptions;

namespace PanCadastro.Adapters.Driving.Middlewares;

// aqui eu criei um middleware global de tratamento de exceções.
// é um Pattern de Chain of Responsibility queintercepta exceções antes da response.
// Traduz exceções de domínio em HTTP status codes mais adequados.
public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    // O construtor recebe o próximo middleware na pipeline e um logger para registrar erros.
    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    //aqui eu uso o método InvokeAsync para capturar exceções lançadas durante o processamento da requisição.
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    // O método HandleExceptionAsync é responsável por mapear as exceções para status
    // codes e mensagens apropriadas.    
    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, message) = exception switch
        {
            NotFoundException ex => (HttpStatusCode.NotFound, ex.Message),
            DomainException ex => (HttpStatusCode.BadRequest, ex.Message),
            ArgumentNullException ex => (HttpStatusCode.BadRequest, $"Parâmetro obrigatório: {ex.ParamName}"),
            ArgumentException ex => (HttpStatusCode.BadRequest, ex.Message),
            _ => (HttpStatusCode.InternalServerError, "Ocorreu um erro interno no servidor.")
        };

        if (statusCode == HttpStatusCode.InternalServerError)
        {
            _logger.LogError(exception, "Erro não tratado: {Message}", exception.Message);
        }
        else
        {
            _logger.LogWarning("Exceção de domínio: {StatusCode} - {Message}", statusCode, message);
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var response = ApiResponse<object>.Erro(message);
        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(json);
    }
}
