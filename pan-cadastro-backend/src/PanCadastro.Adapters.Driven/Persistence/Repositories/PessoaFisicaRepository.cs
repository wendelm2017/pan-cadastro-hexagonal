using Microsoft.EntityFrameworkCore;
using PanCadastro.Adapters.Driven.Persistence.Context;
using PanCadastro.Domain.Entities;
using PanCadastro.Domain.Ports.Out;

namespace PanCadastro.Adapters.Driven.Persistence.Repositories;

// O repository de Pessoa Física tem mais lógica, como validação de CPF único,
// consultas com include de endereços, e operações de remoção que cascata para os
// endereços relacionados. Usei aqui adapter para mapear o contrato do Domain (IPessoaFisicaRepository)
// para as operações de acesso a dados com EF Core.
public class PessoaFisicaRepository : IPessoaFisicaRepository
{
    private readonly PanCadastroDbContext _context;

    public PessoaFisicaRepository(PanCadastroDbContext context)
    {
        _context = context;
    }

    public async Task<PessoaFisica?> ObterPorIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.PessoasFisicas
            .FirstOrDefaultAsync(p => p.Id == id, ct);
    }

    public async Task<PessoaFisica?> ObterComEnderecosAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.PessoasFisicas
            .Include(p => p.Enderecos)
            .FirstOrDefaultAsync(p => p.Id == id, ct);
    }

    public async Task<IEnumerable<PessoaFisica>> ObterTodosAsync(CancellationToken ct = default)
    {
        return await _context.PessoasFisicas
            .Include(p => p.Enderecos)
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public async Task<PessoaFisica> AdicionarAsync(PessoaFisica entity, CancellationToken ct = default)
    {
        await _context.PessoasFisicas.AddAsync(entity, ct);
        await _context.SaveChangesAsync(ct);
        return entity;
    }

    public async Task AtualizarAsync(PessoaFisica entity, CancellationToken ct = default)
    {
        _context.PessoasFisicas.Update(entity);
        await _context.SaveChangesAsync(ct);
    }

    public async Task RemoverAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await _context.PessoasFisicas
            .Include(p => p.Enderecos)
            .FirstOrDefaultAsync(p => p.Id == id, ct);

        if (entity is not null)
        {
            _context.PessoasFisicas.Remove(entity);
            await _context.SaveChangesAsync(ct);
        }
    }

    public async Task<bool> ExisteAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.PessoasFisicas.AnyAsync(p => p.Id == id, ct);
    }

    public async Task<PessoaFisica?> ObterPorCpfAsync(string cpf, CancellationToken ct = default)
    {
        var apenasDigitos = new string(cpf.Where(char.IsDigit).ToArray());
        return await _context.PessoasFisicas
            .FirstOrDefaultAsync(p => p.Cpf.Numero == apenasDigitos, ct);
    }

    public async Task<bool> CpfExisteAsync(string cpf, CancellationToken ct = default)
    {
        var apenasDigitos = new string(cpf.Where(char.IsDigit).ToArray());
        return await _context.PessoasFisicas
            .AnyAsync(p => p.Cpf.Numero == apenasDigitos, ct);
    }
}
