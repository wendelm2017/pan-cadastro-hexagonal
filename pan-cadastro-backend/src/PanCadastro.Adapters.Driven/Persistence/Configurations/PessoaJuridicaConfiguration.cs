using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PanCadastro.Domain.Entities;

namespace PanCadastro.Adapters.Driven.Persistence.Configurations;

public class PessoaJuridicaConfiguration : IEntityTypeConfiguration<PessoaJuridica>
{
    public void Configure(EntityTypeBuilder<PessoaJuridica> builder)
    {
        builder.ToTable("PessoasJuridicas");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.RazaoSocial)
            .IsRequired()
            .HasMaxLength(300);

        builder.Property(p => p.NomeFantasia)
            .IsRequired()
            .HasMaxLength(300);

        builder.OwnsOne(p => p.Cnpj, cnpj =>
        {
            cnpj.Property(c => c.Numero)
                .HasColumnName("Cnpj")
                .IsRequired()
                .HasMaxLength(14);

            cnpj.HasIndex(c => c.Numero).IsUnique();
        });

        builder.Property(p => p.DataAbertura)
            .IsRequired();

        builder.Property(p => p.Email)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.Telefone)
            .HasMaxLength(20);

        builder.Property(p => p.InscricaoEstadual)
            .HasMaxLength(20);

        builder.Property(p => p.Ativo)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(p => p.CriadoEm)
            .IsRequired();

        builder.HasMany(p => p.Enderecos)
            .WithOne()
            .HasForeignKey(e => e.PessoaJuridicaId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(p => p.Enderecos)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
