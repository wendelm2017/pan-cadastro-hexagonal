using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PanCadastro.Domain.Entities;

namespace PanCadastro.Adapters.Driven.Persistence.Configurations;

public class PessoaFisicaConfiguration : IEntityTypeConfiguration<PessoaFisica>
{
    public void Configure(EntityTypeBuilder<PessoaFisica> builder)
    {
        builder.ToTable("PessoasFisicas");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Nome)
            .IsRequired()
            .HasMaxLength(200);

        // Value Object CPF → owned type mapeado como coluna
        builder.OwnsOne(p => p.Cpf, cpf =>
        {
            cpf.Property(c => c.Numero)
                .HasColumnName("Cpf")
                .IsRequired()
                .HasMaxLength(11);

            cpf.HasIndex(c => c.Numero).IsUnique();
        });

        builder.Property(p => p.DataNascimento)
            .IsRequired();

        builder.Property(p => p.Email)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.Telefone)
            .HasMaxLength(20);

        builder.Property(p => p.Ativo)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(p => p.CriadoEm)
            .IsRequired();

        // Relacionamento: PF → muitos Endereços
        builder.HasMany(p => p.Enderecos)
            .WithOne()
            .HasForeignKey(e => e.PessoaFisicaId)
            .OnDelete(DeleteBehavior.Cascade);

        // Ignora o campo privado _enderecos para EF Core usar navigation
        builder.Navigation(p => p.Enderecos)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
