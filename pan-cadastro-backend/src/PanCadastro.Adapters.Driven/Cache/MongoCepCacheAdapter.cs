using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using PanCadastro.Domain.Ports.Out;

namespace PanCadastro.Adapters.Driven.Cache;

// adapter que implementa o cache de cep usando mongodb
// se o mongo cair, o sistema continua funcionando - cache e conveniencia, nao dependencia
// ttl de 24h porque cep nao muda com frequencia
public class MongoCepCacheAdapter : ICepCache
{
    private readonly IMongoCollection<CepCacheDocument> _collection;
    private readonly ILogger<MongoCepCacheAdapter> _logger;
    private static readonly TimeSpan CacheTtl = TimeSpan.FromHours(24);

    public MongoCepCacheAdapter(IMongoClient mongoClient, ILogger<MongoCepCacheAdapter> logger)
    {
        _logger = logger;
        var database = mongoClient.GetDatabase("PanCadastroCache");
        _collection = database.GetCollection<CepCacheDocument>("cep_cache");

        // ttl index - mongo remove os documentos expirados sozinho, sem precisar de job
        var indexKeys = Builders<CepCacheDocument>.IndexKeys.Ascending(d => d.ExpiraEm);
        _collection.Indexes.CreateOne(new CreateIndexModel<CepCacheDocument>(indexKeys, new CreateIndexOptions { ExpireAfter = TimeSpan.Zero }));

        // indice unico no cep pra busca rapida e evitar duplicata
        var cepIndex = Builders<CepCacheDocument>.IndexKeys.Ascending(d => d.Cep);
        _collection.Indexes.CreateOne(new CreateIndexModel<CepCacheDocument>(cepIndex, new CreateIndexOptions { Unique = true }));
    }

    public async Task<ViaCepResponse?> ObterAsync(string cep, CancellationToken ct = default)
    {
        try
        {
            var doc = await _collection.Find(d => d.Cep == cep).FirstOrDefaultAsync(ct);

            if (doc is null)
            {
                _logger.LogInformation("Cache MISS para CEP: {Cep}", cep);
                return null;
            }

            _logger.LogInformation("Cache HIT para CEP: {Cep}", cep);
            return new ViaCepResponse(
                doc.Cep, doc.Logradouro, doc.Complemento, doc.Bairro,
                doc.Localidade, doc.Uf, doc.Estado, doc.Regiao,
                doc.Ibge, doc.Ddd, doc.Siafi);
        }
        catch (Exception ex)
        {
            // se o mongo tiver fora, loga e segue - nao pode parar o sistema por causa de cache
            _logger.LogWarning(ex, "Falha ao consultar cache pro CEP: {Cep}, seguindo sem cache", cep);
            return null;
        }
    }

    public async Task ArmazenarAsync(string cep, ViaCepResponse response, CancellationToken ct = default)
    {
        try
        {
            var doc = new CepCacheDocument
            {
                Cep = cep,
                Logradouro = response.Logradouro,
                Complemento = response.Complemento,
                Bairro = response.Bairro,
                Localidade = response.Localidade,
                Uf = response.Uf,
                Estado = response.Estado,
                Regiao = response.Regiao,
                Ibge = response.Ibge,
                Ddd = response.Ddd,
                Siafi = response.Siafi,
                CriadoEm = DateTime.UtcNow,
                ExpiraEm = DateTime.UtcNow.Add(CacheTtl)
            };

            // upsert - se ja existe atualiza, se nao insere. evita duplicata no cache
            var filter = Builders<CepCacheDocument>.Filter.Eq(d => d.Cep, cep);
            await _collection.ReplaceOneAsync(filter, doc, new ReplaceOptions { IsUpsert = true }, ct);

            _logger.LogInformation("CEP {Cep} armazenado no cache (expira em {Ttl}h)", cep, CacheTtl.TotalHours);
        }
        catch (Exception ex)
        {
            // mesma logica - se falhar o cache, nao bloqueia a operacao
            _logger.LogWarning(ex, "Falha ao gravar cache pro CEP: {Cep}, operacao nao afetada", cep);
        }
    }
}
