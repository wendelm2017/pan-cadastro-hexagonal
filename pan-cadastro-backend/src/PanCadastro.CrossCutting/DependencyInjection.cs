using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PanCadastro.Adapters.Driven.ExternalServices;
using PanCadastro.Adapters.Driven.Persistence.Context;
using PanCadastro.Adapters.Driven.Persistence.Repositories;
using PanCadastro.Application.Mappings;
using PanCadastro.Application.Services;
using PanCadastro.Domain.Ports.In;
using PanCadastro.Domain.Ports.Out;

namespace PanCadastro.CrossCutting;

// Esse é um composition root que eu criei para registrar todas as dependências no 
// container de DI. Conecta Ports (interfaces) aos Adapters (implementações),
// seguindo a arquitetura hexagonal.
public static class DependencyInjection
{
    public static IServiceCollection AddPanCadastro(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // === Database (SQL Server via EF Core) ===
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

        // === External Services (Driven Adapters) ===
        services.AddHttpClient<IViaCepClient, ViaCepClient>(client =>
        {
            client.BaseAddress = new Uri("https://viacep.com.br/ws/");
            client.Timeout = TimeSpan.FromSeconds(10);
        });

        // === AutoMapper ===
        services.AddAutoMapper(typeof(DomainToDtoProfile).Assembly);

        return services;
    }
}
