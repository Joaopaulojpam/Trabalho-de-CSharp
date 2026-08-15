using Franquias.Api.Data;
using Franquias.Api.DTOs;
using Franquias.Api.Models;
using Franquias.Api.Models.Enums;
using Franquias.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Franquias.Api.Services.Implementations;

public class EstoqueService : IEstoqueService
{
    private readonly AppDbContext _context;

    public EstoqueService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<EstoqueResponseDto>> ConsultarEstoquePorUnidadeAsync(int unidadeId, bool apenasAbaixoDoMinimo = false)
    {
        var query = _context.Estoques
            .Include(e => e.UnidadeFranqueada)
            .Include(e => e.ProdutoServico)
            .Where(e => e.UnidadeFranqueadaId == unidadeId);

        if (apenasAbaixoDoMinimo)
        {
            query = query.Where(e => e.Quantidade <= e.QuantidadeMinima);
        }

        return await query
            .OrderBy(e => e.ProdutoServico.Nome)
            .Select(e => new EstoqueResponseDto(
                e.Id,
                e.UnidadeFranqueadaId,
                e.UnidadeFranqueada.Nome,
                e.ProdutoServicoId,
                e.ProdutoServico.Nome,
                e.ProdutoServico.CodigoSku,
                e.Quantidade,
                e.QuantidadeMinima,
                e.Quantidade <= e.QuantidadeMinima,
                e.UltimaAtualizacao
            ))
            .ToListAsync();
    }

    public async Task<EstoqueResponseDto?> ObterEstoqueItemAsync(int unidadeId, int produtoId)
    {
        var e = await _context.Estoques
            .Include(e => e.UnidadeFranqueada)
            .Include(e => e.ProdutoServico)
            .FirstOrDefaultAsync(e => e.UnidadeFranqueadaId == unidadeId && e.ProdutoServicoId == produtoId);

        if (e == null) return null;

        return new EstoqueResponseDto(
            e.Id,
            e.UnidadeFranqueadaId,
            e.UnidadeFranqueada.Nome,
            e.ProdutoServicoId,
            e.ProdutoServico.Nome,
            e.ProdutoServico.CodigoSku,
            e.Quantidade,
            e.QuantidadeMinima,
            e.Quantidade <= e.QuantidadeMinima,
            e.UltimaAtualizacao
        );
    }

    public async Task<EstoqueResponseDto> MovimentarEstoqueAsync(MovimentarEstoqueDto request, int? usuarioId = null)
    {
        var unidade = await _context.UnidadesFranqueadas.FindAsync(request.UnidadeFranqueadaId);
        if (unidade == null)
        {
            throw new InvalidOperationException("Unidade franqueada não encontrada.");
        }

        var produto = await _context.ProdutosServicos.FindAsync(request.ProdutoServicoId);
        if (produto == null)
        {
            throw new InvalidOperationException("Produto não encontrado.");
        }

        var estoque = await _context.Estoques
            .FirstOrDefaultAsync(e => e.UnidadeFranqueadaId == request.UnidadeFranqueadaId && e.ProdutoServicoId == request.ProdutoServicoId);

        if (estoque == null)
        {
            estoque = new Estoque
            {
                UnidadeFranqueadaId = request.UnidadeFranqueadaId,
                ProdutoServicoId = request.ProdutoServicoId,
                Quantidade = 0,
                QuantidadeMinima = 5,
                UltimaAtualizacao = DateTime.UtcNow
            };
            await _context.Estoques.AddAsync(estoque);
        }

        // Regra de Negócio Obrigatória: O estoque não poderá ficar negativo após movimentação
        int novaQuantidade = estoque.Quantidade;

        switch (request.Tipo)
        {
            case TipoMovimentacao.Entrada:
                novaQuantidade += request.Quantidade;
                break;

            case TipoMovimentacao.Saida:
            case TipoMovimentacao.Venda:
                if (estoque.Quantidade < request.Quantidade)
                {
                    throw new InvalidOperationException(
                        $"Saldo de estoque insuficiente para o produto '{produto.Nome}'. Saldo atual: {estoque.Quantidade}, Quantidade solicitada: {request.Quantidade}."
                    );
                }
                novaQuantidade -= request.Quantidade;
                break;

            case TipoMovimentacao.Ajuste:
                if (request.Quantidade < 0)
                {
                    throw new InvalidOperationException("A quantidade no ajuste não pode ser negativa.");
                }
                novaQuantidade = request.Quantidade;
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(request.Tipo), "Tipo de movimentação inválido.");
        }

        estoque.Quantidade = novaQuantidade;
        estoque.UltimaAtualizacao = DateTime.UtcNow;

        // Registrar auditoria de movimentação
        var mov = new MovimentacaoEstoque
        {
            UnidadeFranqueadaId = request.UnidadeFranqueadaId,
            ProdutoServicoId = request.ProdutoServicoId,
            Tipo = request.Tipo,
            Quantidade = request.Quantidade,
            Observacao = request.Observacao,
            DataMovimentacao = DateTime.UtcNow,
            UsuarioId = usuarioId
        };

        await _context.MovimentacoesEstoque.AddAsync(mov);
        await _context.SaveChangesAsync();

        return (await ObterEstoqueItemAsync(request.UnidadeFranqueadaId, request.ProdutoServicoId))!;
    }

    public async Task<EstoqueResponseDto?> AtualizarEstoqueMinimoAsync(int unidadeId, int produtoId, int quantidadeMinima)
    {
        var estoque = await _context.Estoques
            .FirstOrDefaultAsync(e => e.UnidadeFranqueadaId == unidadeId && e.ProdutoServicoId == produtoId);

        if (estoque == null) return null;

        estoque.QuantidadeMinima = quantidadeMinima;
        estoque.UltimaAtualizacao = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return await ObterEstoqueItemAsync(unidadeId, produtoId);
    }

    public async Task<List<MovimentacaoResponseDto>> ListarHistoricoMovimentacoesAsync(int? unidadeId = null, int? produtoId = null)
    {
        var query = _context.MovimentacoesEstoque
            .Include(m => m.UnidadeFranqueada)
            .Include(m => m.ProdutoServico)
            .Include(m => m.Usuario)
            .AsQueryable();

        if (unidadeId.HasValue) query = query.Where(m => m.UnidadeFranqueadaId == unidadeId.Value);
        if (produtoId.HasValue) query = query.Where(m => m.ProdutoServicoId == produtoId.Value);

        return await query
            .OrderByDescending(m => m.DataMovimentacao)
            .Select(m => new MovimentacaoResponseDto(
                m.Id,
                m.UnidadeFranqueadaId,
                m.UnidadeFranqueada.Nome,
                m.ProdutoServicoId,
                m.ProdutoServico.Nome,
                m.Tipo.ToString(),
                m.Quantidade,
                m.Observacao,
                m.DataMovimentacao,
                m.Usuario != null ? m.Usuario.Nome : null
            ))
            .ToListAsync();
    }
}
