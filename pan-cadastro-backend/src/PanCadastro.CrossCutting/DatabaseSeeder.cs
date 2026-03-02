using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PanCadastro.Adapters.Driven.Persistence.Context;
using PanCadastro.Domain.Entities;

namespace PanCadastro.CrossCutting;

// Seed: aplica migrations e insere dados de demo se o banco tiver vazio
public static class DatabaseSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<PanCadastroDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<PanCadastroDbContext>>();

        try
        {
            logger.LogInformation("Aplicando migrations...");
            await context.Database.MigrateAsync();
            logger.LogInformation("Migrations aplicadas com sucesso.");

            if (await context.PessoasFisicas.AnyAsync())
            {
                logger.LogInformation("Banco já possui dados. Seed ignorado.");
                return;
            }

            logger.LogInformation("Inserindo dados de demonstração...");

            // === Pessoa Física ===
            var pf1 = PessoaFisica.Criar("João da Silva", "52998224725", new DateTime(1990, 5, 15), "joao@email.com", "11999998888");
            var pf2 = PessoaFisica.Criar("Maria Santos", "39053344705", new DateTime(1985, 8, 22), "maria@email.com", "11988887777");

            // Endereços PF
            var endPf1 = Endereco.Criar("01001000", "Praça da Sé", "100", "Sé", "São Paulo", "SP", "Sala 1");
            var endPf2 = Endereco.Criar("22041080", "Rua Barata Ribeiro", "200", "Copacabana", "Rio de Janeiro", "RJ");
            pf1.AdicionarEndereco(endPf1);
            pf2.AdicionarEndereco(endPf2);

            await context.PessoasFisicas.AddRangeAsync(pf1, pf2);

            // === Pessoa Jurídica ===
            var pj1 = PessoaJuridica.Criar("Pan Tecnologia LTDA", "Pan Tech", "11222333000181", new DateTime(2020, 1, 15), "contato@pantech.com", "1133334444", "123456789");

            var endPj1 = Endereco.Criar("04543011", "Av. Engenheiro Luiz Carlos Berrini", "1000", "Cidade Monções", "São Paulo", "SP", "Andar 15");
            pj1.AdicionarEndereco(endPj1);

            await context.PessoasJuridicas.AddAsync(pj1);

            await context.SaveChangesAsync();
            logger.LogInformation("Seed concluído: 2 PFs + 1 PJ com endereços.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao executar seed do banco de dados.");
        }
    }
}
