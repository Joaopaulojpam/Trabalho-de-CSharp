using Franquias.Api.Data;
using Franquias.Api.DTOs;
using Franquias.Api.Models;
using Franquias.Api.Models.Enums;
using Franquias.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Franquias.Api.Services.Implementations;

public class ChamadoService : IChamadoService
{
    private readonly AppDbContext _context;

    public ChamadoService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<ChamadoResponseDto>> ListarAsync(int? unidadeId = null, StatusChamado? status = null, PrioridadeChamado? prioridade = null)
    {
        var query = _context.ChamadosSuporte
            .Include(c => c.UnidadeFranqueada)
            .Include(c => c.UsuarioAbertura)
            .AsQueryable();

        if (unidadeId.HasValue) query = query.Where(c => c.UnidadeFranqueadaId == unidadeId.Value);
        if (status.HasValue) query = query.Where(c => c.Status == status.Value);
        if (prioridade.HasValue) query = query.Where(c => c.Prioridade == prioridade.Value);

        return await query
            .OrderByDescending(c => c.DataAbertura)
            .Select(c => new ChamadoResponseDto(
                c.Id,
                c.UnidadeFranqueadaId,
                c.UnidadeFranqueada.Nome,
                c.UsuarioAberturaId,
                c.UsuarioAbertura != null ? c.UsuarioAbertura.Nome : null,
                c.Titulo,
                c.Descricao,
                c.Categoria,
                c.Prioridade.ToString(),
                c.Status.ToString(),
                c.DataAbertura,
                c.DataFechamento,
                c.RespostaSolucao
            ))
            .ToListAsync();
    }

    public async Task<ChamadoResponseDto?> ObterPorIdAsync(int id)
    {
        var c = await _context.ChamadosSuporte
            .Include(c => c.UnidadeFranqueada)
            .Include(c => c.UsuarioAbertura)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (c == null) return null;

        return new ChamadoResponseDto(
            c.Id,
            c.UnidadeFranqueadaId,
            c.UnidadeFranqueada.Nome,
            c.UsuarioAberturaId,
            c.UsuarioAbertura?.Nome,
            c.Titulo,
            c.Descricao,
            c.Categoria,
            c.Prioridade.ToString(),
            c.Status.ToString(),
            c.DataAbertura,
            c.DataFechamento,
            c.RespostaSolucao
        );
    }

    public async Task<ChamadoResponseDto> AbrirChamadoAsync(CriarChamadoDto request, int? usuarioId = null)
    {
        var unidade = await _context.UnidadesFranqueadas.FindAsync(request.UnidadeFranqueadaId);
        if (unidade == null)
        {
            throw new InvalidOperationException("Unidade franqueada não encontrada.");
        }

        var chamado = new ChamadoSuporte
        {
            UnidadeFranqueadaId = request.UnidadeFranqueadaId,
            UsuarioAberturaId = usuarioId,
            Titulo = request.Titulo,
            Descricao = request.Descricao,
            Categoria = request.Categoria,
            Prioridade = request.Prioridade,
            Status = StatusChamado.Aberto,
            DataAbertura = DateTime.UtcNow
        };

        await _context.ChamadosSuporte.AddAsync(chamado);
        await _context.SaveChangesAsync();

        return (await ObterPorIdAsync(chamado.Id))!;
    }

    public async Task<ChamadoResponseDto?> AtualizarStatusAsync(int id, AtualizarStatusChamadoDto request)
    {
        var chamado = await _context.ChamadosSuporte.FindAsync(id);
        if (chamado == null) return null;

        chamado.Status = request.Status;
        if (!string.IsNullOrWhiteSpace(request.RespostaSolucao))
        {
            chamado.RespostaSolucao = request.RespostaSolucao;
        }

        if (request.Status == StatusChamado.Concluido || request.Status == StatusChamado.Cancelado)
        {
            chamado.DataFechamento = DateTime.UtcNow;
        }
        else
        {
            chamado.DataFechamento = null;
        }

        await _context.SaveChangesAsync();
        return await ObterPorIdAsync(id);
    }

    public async Task<Dictionary<string, int>> ContarChamadosPorStatusAsync()
    {
        var agrupado = await _context.ChamadosSuporte
            .GroupBy(c => c.Status)
            .Select(g => new { Status = g.Key.ToString(), Total = g.Count() })
            .ToListAsync();

        return agrupado.ToDictionary(a => a.Status, a => a.Total);
    }
}
