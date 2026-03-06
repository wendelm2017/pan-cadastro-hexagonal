using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using PanCadastro.Adapters.Driven.Cache;
using PanCadastro.Adapters.Driven.ExternalServices;
using PanCadastro.Adapters.Driven.Persistence.Context;
using PanCadastro.Adapters.Driven.Persistence.Repositories;
using PanCadastro.Application.Mappings;
using PanCadastro.Application.Services;
using PanCadastro.Domain.Ports.In;
using PanCadastro.Domain.Ports.Out;

namespace PanCadastro.CrossCutting;

// composition root - aqui conecto cada port ao seu adapter
// esse e o unico lugar que conhece todas as implementacoes concretas
public static class DependencyInjection
{
    public static IServiceCollection AddPanCadastro(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // === SQL Server (EF Core) ===
        services.AddDbContext<PanCadastroDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("SqlServer"),
                b => b.MigrationsAssembly(typeof(PanCadastroDbContext).Assembly.FullName)));

        // === Repositories (Driven Adapters → Ports Out) ===
        services.AddScoped<IPessoaFisicaRepository, PessoaFisicaRepository>();
        services.AddScoped<IPessoaJuridicaRepository, PessoaJuridicaRepository>();
        services.AddScoped<IEnderecoRepository, EnderecoRepository>();

        // === Services (Application → Ports In) ===
        services.AddScoped<IPessoaFisicaService, PessoaFisicaService>();
        services.AddScoped<IPessoaJuridicaService, PessoaJuridicaService>();
        services.AddScoped<IEnderecoService, EnderecoService>();

        // === MongoDB (cache de cep) ===
        var mongoConnectionString = configuration.GetValue<string>("MongoDB:ConnectionString") ?? "mongodb://localhost:27017";
        services.AddSingleton<IMongoClient>(new MongoClient(mongoConnectionString));
        services.AddSingleton<ICepCache, MongoCepCacheAdapter>();

        // === ViaCEP com cache (decorator pattern) ===
        // registro o httpclient pro viacep e monto o decorator que adiciona cache por cima
        services.AddHttpClient("ViaCepClient", client =>
        {
            client.BaseAddress = new Uri("https://viacep.com.br/ws/");
            client.Timeout = TimeSpan.FromSeconds(10);
        });

        services.AddScoped<IViaCepClient>(sp =>
        {
            var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
            var httpClient = httpClientFactory.CreateClient("ViaCepClient");
            var viaCepLogger = sp.GetRequiredService<ILogger<ViaCepClient>>();
            var innerClient = new ViaCepClient(httpClient, viaCepLogger);

            var cache = sp.GetRequiredService<ICepCache>();
            var cachedLogger = sp.GetRequiredService<ILogger<CachedViaCepClient>>();
            return new CachedViaCepClient(innerClient, cache, cachedLogger);
        });

        // === AutoMapper ===
        services.AddAutoMapper(typeof(DomainToDtoProfile).Assembly);

        return services;
    }
}
