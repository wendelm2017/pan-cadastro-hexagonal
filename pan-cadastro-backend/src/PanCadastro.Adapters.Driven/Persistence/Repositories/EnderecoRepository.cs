using Microsoft.EntityFrameworkCore;
using PanCadastro.Adapters.Driven.Persistence.Context;
using PanCadastro.Domain.Entities;
using PanCadastro.Domain.Ports.Out;

namespace PanCadastro.Adapters.Driven.Persistence.Repositories;

//o repository de Endereço é mais simples, sem lógica de negócio complexa, 
//apenas operações CRUD básicas e consultas por pessoa física/jurídica. 
//Ele se comunica diretamente com o DbContext para acessar a tabela de 
//endereços no banco de dados.
public class EnderecoRepository : IEnderecoRepository
{
    private readonly PanCadastroDbContext _context;

    public EnderecoRepository(PanCadastroDbContext context)
    {
        _context = context;
    }

    public async Task<Endereco?> ObterPorIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.Enderecos.FirstOrDefaultAsync(e => e.Id == id, ct);
    }

    public async Task<IEnumerable<Endereco>> ObterTodosAsync(CancellationToken ct = default)
    {
        return await _context.Enderecos.AsNoTracking().ToListAsync(ct);
    }

    public async Task<Endereco> AdicionarAsync(Endereco entity, CancellationToken ct = default)
    {
        await _context.Enderecos.AddAsync(entity, ct);
        await _context.SaveChangesAsync(ct);
        return entity;
    }

    public async Task AtualizarAsync(Endereco entity, CancellationToken ct = default)
    {
        _context.Enderecos.Update(entity);
        await _context.SaveChangesAsync(ct);
    }

    public async Task RemoverAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await _context.Enderecos.FirstOrDefaultAsync(e => e.Id == id, ct);
        if (entity is not null)
        {
            _context.Enderecos.Remove(entity);
            await _context.SaveChangesAsync(ct);
        }
    }

    public async Task<bool> ExisteAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.Enderecos.AnyAsync(e => e.Id == id, ct);
    }

    public async Task<IEnumerable<Endereco>> ObterPorPessoaFisicaAsync(Guid pessoaFisicaId, CancellationToken ct = default)
    {
        return await _context.Enderecos
            .Where(e => e.PessoaFisicaId == pessoaFisicaId)
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<Endereco>> ObterPorPessoaJuridicaAsync(Guid pessoaJuridicaId, CancellationToken ct = default)
    {
        return await _context.Enderecos
            .Where(e => e.PessoaJuridicaId == pessoaJuridicaId)
            .AsNoTracking()
            .ToListAsync(ct);
    }
}
