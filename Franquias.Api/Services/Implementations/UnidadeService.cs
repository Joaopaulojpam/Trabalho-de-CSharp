using Franquias.Api.Data;
using Franquias.Api.DTOs;
using Franquias.Api.Models;
using Franquias.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Franquias.Api.Services.Implementations;

public class UnidadeService : IUnidadeService
{
    private readonly AppDbContext _context;

    public UnidadeService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<UnidadeResponseDto>> ListarAsync(bool? ativo = null, string? termoBusca = null, string? cidade = null, string? uf = null)
    {
        var query = _context.UnidadesFranqueadas.AsQueryable();

        if (ativo.HasValue)
        {
            query = query.Where(u => u.Ativo == ativo.Value);
        }

        if (!string.IsNullOrWhiteSpace(cidade))
        {
            query = query.Where(u => u.Cidade.ToLower().Contains(cidade.ToLower()));
        }

        if (!string.IsNullOrWhiteSpace(uf))
        {
            query = query.Where(u => u.UF.ToLower() == uf.ToLower());
        }

        if (!string.IsNullOrWhiteSpace(termoBusca))
        {
            var termo = termoBusca.ToLower();
            query = query.Where(u =>
                u.Nome.ToLower().Contains(termo) ||
                u.RazaoSocial.ToLower().Contains(termo) ||
                u.CNPJ.Contains(termo) ||
                u.ResponsavelNome.ToLower().Contains(termo) ||
                u.Cidade.ToLower().Contains(termo)
            );
        }

        return await query
            .OrderBy(u => u.Nome)
            .Select(u => new UnidadeResponseDto(
                u.Id,
                u.FranqueadoraId,
                u.Nome,
                u.RazaoSocial,
                u.CNPJ,
                u.ResponsavelNome,
                u.ResponsavelEmail,
                u.ResponsavelTelefone,
                u.Cidade,
                u.UF,
                u.Endereco,
                u.DataInicio,
                u.PercentualRoyalty,
                u.Ativo
            ))
            .ToListAsync();
    }

    public async Task<UnidadeResponseDto?> ObterPorIdAsync(int id)
    {
        var u = await _context.UnidadesFranqueadas.FindAsync(id);
        if (u == null) return null;

        return new UnidadeResponseDto(
            u.Id,
            u.FranqueadoraId,
            u.Nome,
            u.RazaoSocial,
            u.CNPJ,
            u.ResponsavelNome,
            u.ResponsavelEmail,
            u.ResponsavelTelefone,
            u.Cidade,
            u.UF,
            u.Endereco,
            u.DataInicio,
            u.PercentualRoyalty,
            u.Ativo
        );
    }

    public async Task<UnidadeResponseDto> CriarAsync(CriarUnidadeDto request)
    {
        // Regra de Negócio: Não permitir o cadastro de duas unidades com o mesmo CNPJ
        var cnpjLimpo = NormalizarCnpj(request.CNPJ);
        bool cnpjExiste = await _context.UnidadesFranqueadas
            .AnyAsync(u => u.CNPJ.Replace(".", "").Replace("/", "").Replace("-", "") == cnpjLimpo);

        if (cnpjExiste)
        {
            throw new InvalidOperationException($"Já existe uma unidade cadastrada com o CNPJ '{request.CNPJ}'.");
        }

        var franqueadora = await _context.Franqueadoras.FirstOrDefaultAsync();
        int franqueadoraId = franqueadora?.Id ?? request.FranqueadoraId;

        var unidade = new UnidadeFranqueada
        {
            FranqueadoraId = franqueadoraId,
            Nome = request.Nome,
            RazaoSocial = request.RazaoSocial,
            CNPJ = request.CNPJ,
            ResponsavelNome = request.ResponsavelNome,
            ResponsavelEmail = request.ResponsavelEmail,
            ResponsavelTelefone = request.ResponsavelTelefone,
            Cidade = request.Cidade,
            UF = request.UF,
            Endereco = request.Endereco,
            PercentualRoyalty = request.PercentualRoyalty,
            DataInicio = DateTime.UtcNow,
            Ativo = true
        };

        await _context.UnidadesFranqueadas.AddAsync(unidade);
        await _context.SaveChangesAsync();

        // Inicializar estoque zerado para todos os produtos ativos do catálogo
        var produtos = await _context.ProdutosServicos.Where(p => p.Ativo).ToListAsync();
        foreach (var p in produtos)
        {
            await _context.Estoques.AddAsync(new Estoque
            {
                UnidadeFranqueadaId = unidade.Id,
                ProdutoServicoId = p.Id,
                Quantidade = 0,
                QuantidadeMinima = 5,
                UltimaAtualizacao = DateTime.UtcNow
            });
        }
        await _context.SaveChangesAsync();

        return new UnidadeResponseDto(
            unidade.Id,
            unidade.FranqueadoraId,
            unidade.Nome,
            unidade.RazaoSocial,
            unidade.CNPJ,
            unidade.ResponsavelNome,
            unidade.ResponsavelEmail,
            unidade.ResponsavelTelefone,
            unidade.Cidade,
            unidade.UF,
            unidade.Endereco,
            unidade.DataInicio,
            unidade.PercentualRoyalty,
            unidade.Ativo
        );
    }

    public async Task<UnidadeResponseDto?> AtualizarAsync(int id, AtualizarUnidadeDto request)
    {
        var unidade = await _context.UnidadesFranqueadas.FindAsync(id);
        if (unidade == null) return null;

        unidade.Nome = request.Nome;
        unidade.RazaoSocial = request.RazaoSocial;
        unidade.ResponsavelNome = request.ResponsavelNome;
        unidade.ResponsavelEmail = request.ResponsavelEmail;
        unidade.ResponsavelTelefone = request.ResponsavelTelefone;
        unidade.Cidade = request.Cidade;
        unidade.UF = request.UF;
        unidade.Endereco = request.Endereco;
        unidade.PercentualRoyalty = request.PercentualRoyalty;
        unidade.Ativo = request.Ativo;

        await _context.SaveChangesAsync();

        return new UnidadeResponseDto(
            unidade.Id,
            unidade.FranqueadoraId,
            unidade.Nome,
            unidade.RazaoSocial,
            unidade.CNPJ,
            unidade.ResponsavelNome,
            unidade.ResponsavelEmail,
            unidade.ResponsavelTelefone,
            unidade.Cidade,
            unidade.UF,
            unidade.Endereco,
            unidade.DataInicio,
            unidade.PercentualRoyalty,
            unidade.Ativo
        );
    }

    public async Task<bool> AlternarStatusAsync(int id)
    {
        var unidade = await _context.UnidadesFranqueadas.FindAsync(id);
        if (unidade == null) return false;

        unidade.Ativo = !unidade.Ativo;
        await _context.SaveChangesAsync();
        return true;
    }

    private static string NormalizarCnpj(string cnpj)
    {
        return cnpj.Replace(".", "").Replace("/", "").Replace("-", "").Trim();
    }
}
