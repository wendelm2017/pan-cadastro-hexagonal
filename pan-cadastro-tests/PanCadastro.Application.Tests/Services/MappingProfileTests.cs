using AutoMapper;
using PanCadastro.Application.Mappings;

namespace PanCadastro.Application.Tests.Services;

//teste para validar o perfil de mapeamento do AutoMapper, garantindo que as configurações
//definidas no DomainToDtoProfile sejam válidas e não contenham erros. Ele utiliza o método
//AssertConfigurationIsValid para verificar se todas as regras de mapeamento estão corretas
public class MappingProfileTests
{
    [Fact]
    public void DomainToDtoProfile_DeveSerValido()
    {
        var config = new MapperConfiguration(cfg =>
            cfg.AddProfile<DomainToDtoProfile>());

        config.AssertConfigurationIsValid();
    }
}
