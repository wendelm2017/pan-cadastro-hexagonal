using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PanCadastro.Domain.Entities;

namespace PanCadastro.Adapters.Driven.Persistence.Configurations;

public class EnderecoConfiguration : IEntityTypeConfiguration<Endereco>
{
    public void Configure(EntityTypeBuilder<Endereco> builder)
    {
        builder.ToTable("Enderecos");

        builder.HasKey(e => e.Id);

        builder.OwnsOne(e => e.Cep, cep =>
        {
            cep.Property(c => c.Numero)
                .HasColumnName("Cep")
                .IsRequired()
                .HasMaxLength(8);

            cep.HasIndex(c => c.Numero);
        });

        builder.Property(e => e.Logradouro)
            .IsRequired()
            .HasMaxLength(300);

        builder.Property(e => e.Numero)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(e => e.Complemento)
            .HasMaxLength(200);

        builder.Property(e => e.Bairro)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.Cidade)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.Estado)
            .IsRequired()
            .HasMaxLength(2);

        builder.Property(e => e.Ativo)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(e => e.CriadoEm)
            .IsRequired();
    }
}
