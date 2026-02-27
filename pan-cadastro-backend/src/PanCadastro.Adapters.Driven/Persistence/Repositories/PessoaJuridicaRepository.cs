using Microsoft.EntityFrameworkCore;
using PanCadastro.Adapters.Driven.Persistence.Context;
using PanCadastro.Domain.Entities;
using PanCadastro.Domain.Ports.Out;

namespace PanCadastro.Adapters.Driven.Persistence.Repositories;

// O repository de Pessoa Jurídica é similar ao de Pessoa Física, mas sem a 
//lógica de validação de CPF. Ele tem operações CRUD básicas, consultas por CNPJ, e inclui os endereços relacionados quando necessário. Ele se comunica diretamente com o DbContext para acessar a tabela de pessoas jurídicas e seus endereços no banco de dados. Usei aqui adapter para mapear o contrato do Domain
// (IPessoaJuridicaRepository) para as operações de acesso a dados com EF Core.
public class PessoaJuridicaRepository : IPessoaJuridicaRepository
{
    private readonly PanCadastroDbContext _context;

    public PessoaJuridicaRepository(PanCadastroDbContext context)
    {
        _context = context;
    }

    public async Task<PessoaJuridica?> ObterPorIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.PessoasJuridicas
            .FirstOrDefaultAsync(p => p.Id == id, ct);
    }

    public async Task<PessoaJuridica?> ObterComEnderecosAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.PessoasJuridicas
            .Include(p => p.Enderecos)
            .FirstOrDefaultAsync(p => p.Id == id, ct);
    }

    public async Task<IEnumerable<PessoaJuridica>> ObterTodosAsync(CancellationToken ct = default)
    {
        return await _context.PessoasJuridicas
            .Include(p => p.Enderecos)
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public async Task<PessoaJuridica> AdicionarAsync(PessoaJuridica entity, CancellationToken ct = default)
    {
        await _context.PessoasJuridicas.AddAsync(entity, ct);
        await _context.SaveChangesAsync(ct);
        return entity;
    }

    public async Task AtualizarAsync(PessoaJuridica entity, CancellationToken ct = default)
    {
        _context.PessoasJuridicas.Update(entity);
        await _context.SaveChangesAsync(ct);
    }

    public async Task RemoverAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await _context.PessoasJuridicas
            .Include(p => p.Enderecos)
            .FirstOrDefaultAsync(p => p.Id == id, ct);

        if (entity is not null)
        {
            _context.PessoasJuridicas.Remove(entity);
            await _context.SaveChangesAsync(ct);
        }
    }

    public async Task<bool> ExisteAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.PessoasJuridicas.AnyAsync(p => p.Id == id, ct);
    }

    public async Task<PessoaJuridica?> ObterPorCnpjAsync(string cnpj, CancellationToken ct = default)
    {
        var apenasDigitos = new string(cnpj.Where(char.IsDigit).ToArray());
        return await _context.PessoasJuridicas
            .FirstOrDefaultAsync(p => p.Cnpj.Numero == apenasDigitos, ct);
    }

    public async Task<bool> CnpjExisteAsync(string cnpj, CancellationToken ct = default)
    {
        var apenasDigitos = new string(cnpj.Where(char.IsDigit).ToArray());
        return await _context.PessoasJuridicas
            .AnyAsync(p => p.Cnpj.Numero == apenasDigitos, ct);
    }
}
