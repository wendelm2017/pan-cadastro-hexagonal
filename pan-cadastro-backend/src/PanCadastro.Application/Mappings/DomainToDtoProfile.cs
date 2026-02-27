using AutoMapper;
using PanCadastro.Application.DTOs.Responses;
using PanCadastro.Domain.Entities;
using PanCadastro.Domain.Ports.Out;

namespace PanCadastro.Application.Mappings;

//Esse mapper faz a conversão entre as entidades do domínio e os DTOs de resposta usados na API,
//ele é configurado usando o AutoMapper, que é uma biblioteca popular para mapeamento de objetos em .NET.
// O DomainToDtoProfile define as regras de mapeamento, como
public class DomainToDtoProfile : Profile
{
    public DomainToDtoProfile()
    {
        CreateMap<PessoaFisica, PessoaFisicaResponse>()
            .ForCtorParam("Cpf", opt => opt.MapFrom(src => src.Cpf.Numero))
            .ForCtorParam("CpfFormatado", opt => opt.MapFrom(src => src.Cpf.Formatado))
            .ForCtorParam("Enderecos", opt => opt.MapFrom(src => src.Enderecos));

        CreateMap<PessoaJuridica, PessoaJuridicaResponse>()
            .ForCtorParam("Cnpj", opt => opt.MapFrom(src => src.Cnpj.Numero))
            .ForCtorParam("CnpjFormatado", opt => opt.MapFrom(src => src.Cnpj.Formatado))
            .ForCtorParam("Enderecos", opt => opt.MapFrom(src => src.Enderecos));

        CreateMap<Endereco, EnderecoResponse>()
            .ForCtorParam("Cep", opt => opt.MapFrom(src => src.Cep.Numero))
            .ForCtorParam("CepFormatado", opt => opt.MapFrom(src => src.Cep.Formatado));

        CreateMap<ViaCepResponse, ViaCepResponseDto>();
    }
}
