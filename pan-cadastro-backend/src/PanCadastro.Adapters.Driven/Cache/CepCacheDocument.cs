using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace PanCadastro.Adapters.Driven.Cache;

// documento que representa o cache de cep no mongodb
// o campo ExpiraEm trabalha junto com o ttl index - o proprio mongo limpa os expirados
public class CepCacheDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("cep")]
    public string Cep { get; set; } = string.Empty;

    [BsonElement("logradouro")]
    public string Logradouro { get; set; } = string.Empty;

    [BsonElement("complemento")]
    public string Complemento { get; set; } = string.Empty;

    [BsonElement("bairro")]
    public string Bairro { get; set; } = string.Empty;

    [BsonElement("localidade")]
    public string Localidade { get; set; } = string.Empty;

    [BsonElement("uf")]
    public string Uf { get; set; } = string.Empty;

    [BsonElement("estado")]
    public string Estado { get; set; } = string.Empty;

    [BsonElement("regiao")]
    public string Regiao { get; set; } = string.Empty;

    [BsonElement("ibge")]
    public string Ibge { get; set; } = string.Empty;

    [BsonElement("ddd")]
    public string Ddd { get; set; } = string.Empty;

    [BsonElement("siafi")]
    public string Siafi { get; set; } = string.Empty;

    [BsonElement("criadoEm")]
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;

    // mongo usa esse campo pra saber quando apagar o registro automaticamente
    [BsonElement("expiraEm")]
    public DateTime ExpiraEm { get; set; }
}
