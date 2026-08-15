using Franquias.Api.Data;
using Franquias.Api.DTOs;
using Franquias.Api.Models;
using Franquias.Api.Models.Enums;
using Franquias.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Franquias.Api.Services.Implementations;

public class RoyaltyService : IRoyaltyService
{
    private readonly AppDbContext _context;

    public RoyaltyService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<RoyaltyResponseDto>> ListarAsync(int? unidadeId = null, int? mes = null, int? ano = null, StatusRoyalty? status = null)
    {
        var query = _context.Royalties
            .Include(r => r.UnidadeFranqueada)
            .AsQueryable();

        if (unidadeId.HasValue) query = query.Where(r => r.UnidadeFranqueadaId == unidadeId.Value);
        if (mes.HasValue) query = query.Where(r => r.MesReferencia == mes.Value);
        if (ano.HasValue) query = query.Where(r => r.AnoReferencia == ano.Value);
        if (status.HasValue) query = query.Where(r => r.Status == status.Value);

        return await query
            .OrderByDescending(r => r.AnoReferencia)
            .ThenByDescending(r => r.MesReferencia)
            .Select(r => new RoyaltyResponseDto(
                r.Id,
                r.UnidadeFranqueadaId,
                r.UnidadeFranqueada.Nome,
                r.MesReferencia,
                r.AnoReferencia,
                r.FaturamentoBase,
                r.PercentualAplicado,
                r.ValorCalculado,
                r.DataGeracao,
                r.DataVencimento,
                r.Status.ToString(),
                r.DataPagamento,
                r.Observacao
            ))
            .ToListAsync();
    }

    public async Task<RoyaltyResponseDto?> ObterPorIdAsync(int id)
    {
        var r = await _context.Royalties
            .Include(r => r.UnidadeFranqueada)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (r == null) return null;

        return new RoyaltyResponseDto(
            r.Id,
            r.UnidadeFranqueadaId,
            r.UnidadeFranqueada.Nome,
            r.MesReferencia,
            r.AnoReferencia,
            r.FaturamentoBase,
            r.PercentualAplicado,
            r.ValorCalculado,
            r.DataGeracao,
            r.DataVencimento,
            r.Status.ToString(),
            r.DataPagamento,
            r.Observacao
        );
    }

    public async Task<RoyaltyResponseDto> GerarOuRecalcularAsync(GerarRoyaltyDto request)
    {
        var unidade = await _context.UnidadesFranqueadas.FindAsync(request.UnidadeFranqueadaId);
        if (unidade == null)
        {
            throw new InvalidOperationException("Unidade franqueada não encontrada.");
        }

        // Data de início e fim do mês de referência
        var dataInicioMes = new DateTime(request.AnoReferencia, request.MesReferencia, 1, 0, 0, 0, DateTimeKind.Utc);
        var dataFimMes = dataInicioMes.AddMonths(1).AddTicks(-1);

        // Somatório das vendas concluídas da unidade no mês
        var faturamentoPeriodo = await _context.Vendas
            .Where(v => v.UnidadeFranqueadaId == request.UnidadeFranqueadaId
                     && v.Status == StatusVenda.Concluida
                     && v.DataVenda >= dataInicioMes
                     && v.DataVenda <= dataFimMes)
            .SumAsync(v => v.ValorTotal);

        // Regra de Negócio: O royalty deverá ser calculado de acordo com o percentual configurado e o faturamento da unidade no período
        decimal percentual = unidade.PercentualRoyalty;
        decimal valorCalculado = Math.Round(faturamentoPeriodo * (percentual / 100m), 2);

        var royaltyExistente = await _context.Royalties
            .FirstOrDefaultAsync(r => r.UnidadeFranqueadaId == request.UnidadeFranqueadaId
                                   && r.MesReferencia == request.MesReferencia
                                   && r.AnoReferencia == request.AnoReferencia);

        if (royaltyExistente != null)
        {
            royaltyExistente.FaturamentoBase = faturamentoPeriodo;
            royaltyExistente.PercentualAplicado = percentual;
            royaltyExistente.ValorCalculado = valorCalculado;
            royaltyExistente.DataGeracao = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return (await ObterPorIdAsync(royaltyExistente.Id))!;
        }

        var novoRoyalty = new Royalty
        {
            UnidadeFranqueadaId = request.UnidadeFranqueadaId,
            MesReferencia = request.MesReferencia,
            AnoReferencia = request.AnoReferencia,
            FaturamentoBase = faturamentoPeriodo,
            PercentualAplicado = percentual,
            ValorCalculado = valorCalculado,
            DataGeracao = DateTime.UtcNow,
            DataVencimento = DateTime.UtcNow.AddDays(10),
            Status = StatusRoyalty.Pendente,
            Observacao = $"Royalty referente ao período {request.MesReferencia:D2}/{request.AnoReferencia}"
        };

        await _context.Royalties.AddAsync(novoRoyalty);
        await _context.SaveChangesAsync();

        return (await ObterPorIdAsync(novoRoyalty.Id))!;
    }

    public async Task<RoyaltyResponseDto?> RegistrarPagamentoAsync(int id, RegistrarPagamentoRoyaltyDto request)
    {
        var royalty = await _context.Royalties.FindAsync(id);
        if (royalty == null) return null;

        royalty.Status = StatusRoyalty.Pago;
        royalty.DataPagamento = DateTime.SpecifyKind(request.DataPagamento, DateTimeKind.Utc);
        if (!string.IsNullOrWhiteSpace(request.Observacao))
        {
            royalty.Observacao = request.Observacao;
        }

        await _context.SaveChangesAsync();
        return await ObterPorIdAsync(id);
    }

    public async Task<ResumoRoyaltiesDto> ObterResumoAsync(int mes, int ano)
    {
        var royalties = await _context.Royalties
            .Where(r => r.MesReferencia == mes && r.AnoReferencia == ano)
            .ToListAsync();

        int totalUnidades = royalties.Count;
        decimal faturamentoTotal = royalties.Sum(r => r.FaturamentoBase);
        decimal royaltiesTotal = royalties.Sum(r => r.ValorCalculado);
        decimal royaltiesPagos = royalties.Where(r => r.Status == StatusRoyalty.Pago).Sum(r => r.ValorCalculado);
        decimal royaltiesPendentes = royalties.Where(r => r.Status != StatusRoyalty.Pago).Sum(r => r.ValorCalculado);

        return new ResumoRoyaltiesDto(
            mes,
            ano,
            totalUnidades,
            faturamentoTotal,
            royaltiesTotal,
            royaltiesPagos,
            royaltiesPendentes
        );
    }
}
