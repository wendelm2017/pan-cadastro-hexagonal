using Microsoft.EntityFrameworkCore;
using PanCadastro.Domain.Entities;

namespace PanCadastro.Adapters.Driven.Persistence.Context;

// DbContext — usei unit of work com EF Core.
public class PanCadastroDbContext : DbContext
{
    public DbSet<PessoaFisica> PessoasFisicas => Set<PessoaFisica>();
    public DbSet<PessoaJuridica> PessoasJuridicas => Set<PessoaJuridica>();
    public DbSet<Endereco> Enderecos => Set<Endereco>();

    public PanCadastroDbContext(DbContextOptions<PanCadastroDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PanCadastroDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
