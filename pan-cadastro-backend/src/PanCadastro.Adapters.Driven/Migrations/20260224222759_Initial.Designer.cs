using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PanCadastro.Adapters.Driven.Persistence.Context;

#nullable disable

namespace PanCadastro.Adapters.Driven.Migrations
{
    [DbContext(typeof(PanCadastroDbContext))]
    [Migration("20260224222759_Initial")]
    partial class Initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.11")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("PanCadastro.Domain.Entities.Endereco", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("Ativo")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(true);

                    b.Property<DateTime?>("AtualizadoEm")
                        .HasColumnType("datetime2");

                    b.Property<string>("Bairro")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<string>("Cidade")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<string>("Complemento")
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<DateTime>("CriadoEm")
                        .HasColumnType("datetime2");

                    b.Property<string>("Estado")
                        .IsRequired()
                        .HasMaxLength(2)
                        .HasColumnType("nvarchar(2)");

                    b.Property<string>("Logradouro")
                        .IsRequired()
                        .HasMaxLength(300)
                        .HasColumnType("nvarchar(300)");

                    b.Property<string>("Numero")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<Guid?>("PessoaFisicaId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("PessoaJuridicaId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("PessoaFisicaId");

                    b.HasIndex("PessoaJuridicaId");

                    b.ToTable("Enderecos", (string)null);
                });

            modelBuilder.Entity("PanCadastro.Domain.Entities.PessoaFisica", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("Ativo")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(true);

                    b.Property<DateTime?>("AtualizadoEm")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("CriadoEm")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DataNascimento")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<string>("Nome")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<string>("Telefone")
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.HasKey("Id");

                    b.ToTable("PessoasFisicas", (string)null);
                });

            modelBuilder.Entity("PanCadastro.Domain.Entities.PessoaJuridica", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("Ativo")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(true);

                    b.Property<DateTime?>("AtualizadoEm")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("CriadoEm")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DataAbertura")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<string>("InscricaoEstadual")
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<string>("NomeFantasia")
                        .IsRequired()
                        .HasMaxLength(300)
                        .HasColumnType("nvarchar(300)");

                    b.Property<string>("RazaoSocial")
                        .IsRequired()
                        .HasMaxLength(300)
                        .HasColumnType("nvarchar(300)");

                    b.Property<string>("Telefone")
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.HasKey("Id");

                    b.ToTable("PessoasJuridicas", (string)null);
                });

            modelBuilder.Entity("PanCadastro.Domain.Entities.Endereco", b =>
                {
                    b.HasOne("PanCadastro.Domain.Entities.PessoaFisica", null)
                        .WithMany("Enderecos")
                        .HasForeignKey("PessoaFisicaId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PanCadastro.Domain.Entities.PessoaJuridica", null)
                        .WithMany("Enderecos")
                        .HasForeignKey("PessoaJuridicaId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.OwnsOne("PanCadastro.Domain.ValueObjects.CEP", "Cep", b1 =>
                        {
                            b1.Property<Guid>("EnderecoId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<string>("Numero")
                                .IsRequired()
                                .HasMaxLength(8)
                                .HasColumnType("nvarchar(8)")
                                .HasColumnName("Cep");

                            b1.HasKey("EnderecoId");

                            b1.HasIndex("Numero");

                            b1.ToTable("Enderecos");

                            b1.WithOwner()
                                .HasForeignKey("EnderecoId");
                        });

                    b.Navigation("Cep")
                        .IsRequired();
                });

            modelBuilder.Entity("PanCadastro.Domain.Entities.PessoaFisica", b =>
                {
                    b.OwnsOne("PanCadastro.Domain.ValueObjects.CPF", "Cpf", b1 =>
                        {
                            b1.Property<Guid>("PessoaFisicaId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<string>("Numero")
                                .IsRequired()
                                .HasMaxLength(11)
                                .HasColumnType("nvarchar(11)")
                                .HasColumnName("Cpf");

                            b1.HasKey("PessoaFisicaId");

                            b1.HasIndex("Numero")
                                .IsUnique();

                            b1.ToTable("PessoasFisicas");

                            b1.WithOwner()
                                .HasForeignKey("PessoaFisicaId");
                        });

                    b.Navigation("Cpf")
                        .IsRequired();
                });

            modelBuilder.Entity("PanCadastro.Domain.Entities.PessoaJuridica", b =>
                {
                    b.OwnsOne("PanCadastro.Domain.ValueObjects.CNPJ", "Cnpj", b1 =>
                        {
                            b1.Property<Guid>("PessoaJuridicaId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<string>("Numero")
                                .IsRequired()
                                .HasMaxLength(14)
                                .HasColumnType("nvarchar(14)")
                                .HasColumnName("Cnpj");

                            b1.HasKey("PessoaJuridicaId");

                            b1.HasIndex("Numero")
                                .IsUnique();

                            b1.ToTable("PessoasJuridicas");

                            b1.WithOwner()
                                .HasForeignKey("PessoaJuridicaId");
                        });

                    b.Navigation("Cnpj")
                        .IsRequired();
                });

            modelBuilder.Entity("PanCadastro.Domain.Entities.PessoaFisica", b =>
                {
                    b.Navigation("Enderecos");
                });

            modelBuilder.Entity("PanCadastro.Domain.Entities.PessoaJuridica", b =>
                {
                    b.Navigation("Enderecos");
                });
#pragma warning restore 612, 618
        }
    }
}
