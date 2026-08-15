using Franquias.Api.Data;
using Franquias.Api.DTOs;
using Franquias.Api.Models.Enums;
using Franquias.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Franquias.Api.Services.Implementations;

public class RelatorioService : IRelatorioService
{
    private readonly AppDbContext _context;

    public RelatorioService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<FaturamentoUnidadeRelatorioDto>> ObterFaturamentoPorUnidadesAsync(DateTime? dataInicio, DateTime? dataFim)
    {
        var unidades = await _context.UnidadesFranqueadas.ToListAsync();

        var queryVendas = _context.Vendas.Where(v => v.Status == StatusVenda.Concluida).AsQueryable();

        if (dataInicio.HasValue)
        {
            var inicioUtc = DateTime.SpecifyKind(dataInicio.Value.Date, DateTimeKind.Utc);
            queryVendas = queryVendas.Where(v => v.DataVenda >= inicioUtc);
        }

        if (dataFim.HasValue)
        {
            var fimUtc = DateTime.SpecifyKind(dataFim.Value.Date.AddDays(1).AddTicks(-1), DateTimeKind.Utc);
            queryVendas = queryVendas.Where(v => v.DataVenda <= fimUtc);
        }

        var vendasAgrupadas = await queryVendas
            .GroupBy(v => v.UnidadeFranqueadaId)
            .Select(g => new
            {
                UnidadeId = g.Key,
                TotalVendas = g.Count(),
                TotalFaturamento = g.Sum(v => v.ValorTotal)
            })
            .ToListAsync();

        var resultado = new List<FaturamentoUnidadeRelatorioDto>();

        foreach (var u in unidades)
        {
            var grupo = vendasAgrupadas.FirstOrDefault(g => g.UnidadeId == u.Id);
            decimal faturamento = grupo?.TotalFaturamento ?? 0;
            int totalVendas = grupo?.TotalVendas ?? 0;
            decimal royalties = Math.Round(faturamento * (u.PercentualRoyalty / 100m), 2);

            resultado.Add(new FaturamentoUnidadeRelatorioDto(
                u.Id,
                u.Nome,
                u.Cidade,
                u.UF,
                totalVendas,
                faturamento,
                royalties
            ));
        }

        return resultado.OrderByDescending(r => r.TotalFaturamento).ToList();
    }

    public async Task<List<RankingUnidadeDto>> ObterRankingUnidadesAsync(DateTime? dataInicio, DateTime? dataFim)
    {
        var faturamentos = await ObterFaturamentoPorUnidadesAsync(dataInicio, dataFim);

        return faturamentos
            .OrderByDescending(f => f.TotalFaturamento)
            .Select((f, index) => new RankingUnidadeDto(
                index + 1,
                f.UnidadeId,
                f.NomeUnidade,
                f.Cidade,
                f.UF,
                f.TotalVendas,
                f.TotalFaturamento
            ))
            .ToList();
    }

    public async Task<List<ProdutoMaisVendidoDto>> ObterProdutosMaisVendidosAsync(int top = 10, DateTime? dataInicio = null, DateTime? dataFim = null)
    {
        var queryItens = _context.ItensVenda
            .Include(i => i.Venda)
            .Include(i => i.ProdutoServico)
                .ThenInclude(p => p.Categoria)
            .Where(i => i.Venda.Status == StatusVenda.Concluida)
            .AsQueryable();

        if (dataInicio.HasValue)
        {
            var inicioUtc = DateTime.SpecifyKind(dataInicio.Value.Date, DateTimeKind.Utc);
            queryItens = queryItens.Where(i => i.Venda.DataVenda >= inicioUtc);
        }

        if (dataFim.HasValue)
        {
            var fimUtc = DateTime.SpecifyKind(dataFim.Value.Date.AddDays(1).AddTicks(-1), DateTimeKind.Utc);
            queryItens = queryItens.Where(i => i.Venda.DataVenda <= fimUtc);
        }

        var agrupados = await queryItens
            .GroupBy(i => new { i.ProdutoServicoId, i.ProdutoServico.Nome, i.ProdutoServico.CodigoSku, CategoriaNome = i.ProdutoServico.Categoria.Nome })
            .Select(g => new ProdutoMaisVendidoDto(
                g.Key.ProdutoServicoId,
                g.Key.Nome,
                g.Key.CodigoSku,
                g.Key.CategoriaNome,
                g.Sum(i => i.Quantidade),
                g.Sum(i => i.Subtotal)
            ))
            .OrderByDescending(p => p.QuantidadeTotalVendida)
            .Take(top)
            .ToListAsync();

        return agrupados;
    }

    public async Task<List<EstoqueResponseDto>> ObterTodosEstoquesCriticosAsync()
    {
        return await _context.Estoques
            .Include(e => e.UnidadeFranqueada)
            .Include(e => e.ProdutoServico)
            .Where(e => e.Quantidade <= e.QuantidadeMinima)
            .OrderBy(e => e.UnidadeFranqueada.Nome)
            .ThenBy(e => e.Quantidade)
            .Select(e => new EstoqueResponseDto(
                e.Id,
                e.UnidadeFranqueadaId,
                e.UnidadeFranqueada.Nome,
                e.ProdutoServicoId,
                e.ProdutoServico.Nome,
                e.ProdutoServico.CodigoSku,
                e.Quantidade,
                e.QuantidadeMinima,
                true,
                e.UltimaAtualizacao
            ))
            .ToListAsync();
    }

    public async Task<IndicadoresGeraisDto> ObterIndicadoresGeraisAsync()
    {
        int unidadesAtivas = await _context.UnidadesFranqueadas.CountAsync(u => u.Ativo);
        int unidadesInativas = await _context.UnidadesFranqueadas.CountAsync(u => !u.Ativo);
        int totalProdutos = await _context.ProdutosServicos.CountAsync(p => p.Ativo);
        int estoquesCriticos = await _context.Estoques.CountAsync(e => e.Quantidade <= e.QuantidadeMinima);
        int chamadosAbertos = await _context.ChamadosSuporte.CountAsync(c => c.Status == StatusChamado.Aberto);
        int chamadosEmAtendimento = await _context.ChamadosSuporte.CountAsync(c => c.Status == StatusChamado.EmAtendimento);
        decimal faturamentoTotal = await _context.Vendas.Where(v => v.Status == StatusVenda.Concluida).SumAsync(v => v.ValorTotal);
        decimal royaltiesTotal = await _context.Royalties.SumAsync(r => r.ValorCalculado);

        return new IndicadoresGeraisDto(
            unidadesAtivas,
            unidadesInativas,
            totalProdutos,
            estoquesCriticos,
            chamadosAbertos,
            chamadosEmAtendimento,
            faturamentoTotal,
            royaltiesTotal
        );
    }
}
