using PanCadastro.Adapters.Driving.Middlewares;
using PanCadastro.CrossCutting;
using Serilog;
using System.Text.Json;

// essa parte é a Composition Root da aplicação, onde configuro a injeção de dependências,
// logging, middlewares e a pipeline do ASP.NET Core.
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/pan-cadastro-.log", rollingInterval: RollingInterval.Day)
    .CreateBootstrapLogger();

try
{
    Log.Information("Iniciando PanCadastro API...");

    var builder = WebApplication.CreateBuilder(args);

    // === Serilog ===
    builder.Host.UseSerilog((ctx, lc) => lc
        .ReadFrom.Configuration(ctx.Configuration)
        .WriteTo.Console()
        .WriteTo.File("logs/pan-cadastro-.log", rollingInterval: RollingInterval.Day));

    // === Controllers + JSON ===
    builder.Services.AddControllers()
        .AddJsonOptions(opts =>
        {
            opts.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            opts.JsonSerializerOptions.DefaultIgnoreCondition =
                System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
        });

    // === Swagger ===
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
        {
            Title = "PanCadastro API",
            Version = "v1",
            Description = "API de Cadastro - Pessoa Fisica, Pessoa Juridica e Endereco. " +
                         "Arquitetura Hexagonal com .NET 8."
        });
    });

    // === Composition Root (CrossCutting) ===
    builder.Services.AddPanCadastro(builder.Configuration);

    // === CORS (Angular frontend) ===
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAngular", policy =>
            policy.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod());
    });

    // === HealthChecks ===
    builder.Services.AddHealthChecks();

    var app = builder.Build();

    // === Global Exception Middleware (Chain of Responsibility Pattern) ===
    app.UseMiddleware<GlobalExceptionMiddleware>();

    // === Pipeline ===
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "PanCadastro API v1"));
    }

    app.UseSerilogRequestLogging();
    app.UseCors("AllowAngular");
    app.MapControllers();
    app.MapHealthChecks("/health");

    // === Auto-migrate + Seed (Development) ===
    if (app.Environment.IsDevelopment())
    {
        await PanCadastro.CrossCutting.DatabaseSeeder.SeedAsync(app.Services);
    }

    Log.Information("PanCadastro API iniciada");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Aplicacao encerrada inesperadamente.");
}
finally
{
    Log.CloseAndFlush();
}

// essa classe Program é apenas um marcador para o arquivo Program.cs, 
// onde a aplicação é configurada e iniciada, mas é necessario para testes de integracao
// com WebApplicationFactory.
public partial class Program { }
