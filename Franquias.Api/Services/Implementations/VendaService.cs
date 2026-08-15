using Franquias.Api.Data;
using Franquias.Api.DTOs;
using Franquias.Api.Models;
using Franquias.Api.Models.Enums;
using Franquias.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Franquias.Api.Services.Implementations;

public class VendaService : IVendaService
{
    private readonly AppDbContext _context;

    public VendaService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<VendaResponseDto>> ListarVendasAsync(int? unidadeId = null, DateTime? dataInicio = null, DateTime? dataFim = null)
    {
        var query = _context.Vendas
            .Include(v => v.UnidadeFranqueada)
            .Include(v => v.Usuario)
            .Include(v => v.Itens)
                .ThenInclude(i => i.ProdutoServico)
            .AsQueryable();

        if (unidadeId.HasValue)
        {
            query = query.Where(v => v.UnidadeFranqueadaId == unidadeId.Value);
        }

        if (dataInicio.HasValue)
        {
            var inicioUtc = DateTime.SpecifyKind(dataInicio.Value.Date, DateTimeKind.Utc);
            query = query.Where(v => v.DataVenda >= inicioUtc);
        }

        if (dataFim.HasValue)
        {
            var fimUtc = DateTime.SpecifyKind(dataFim.Value.Date.AddDays(1).AddTicks(-1), DateTimeKind.Utc);
            query = query.Where(v => v.DataVenda <= fimUtc);
        }

        return await query
            .OrderByDescending(v => v.DataVenda)
            .Select(v => new VendaResponseDto(
                v.Id,
                v.UnidadeFranqueadaId,
                v.UnidadeFranqueada.Nome,
                v.UsuarioId,
                v.Usuario != null ? v.Usuario.Nome : null,
                v.DataVenda,
                v.ValorTotal,
                v.Observacao,
                v.Status.ToString(),
                v.Itens.Select(i => new ItemVendaResponseDto(
                    i.Id,
                    i.ProdutoServicoId,
                    i.ProdutoServico.Nome,
                    i.ProdutoServico.CodigoSku,
                    i.Quantidade,
                    i.PrecoUnitario,
                    i.Subtotal
                )).ToList()
            ))
            .ToListAsync();
    }

    public async Task<VendaResponseDto?> ObterVendaPorIdAsync(int id)
    {
        var v = await _context.Vendas
            .Include(v => v.UnidadeFranqueada)
            .Include(v => v.Usuario)
            .Include(v => v.Itens)
                .ThenInclude(i => i.ProdutoServico)
            .FirstOrDefaultAsync(v => v.Id == id);

        if (v == null) return null;

        return new VendaResponseDto(
            v.Id,
            v.UnidadeFranqueadaId,
            v.UnidadeFranqueada.Nome,
            v.UsuarioId,
            v.Usuario?.Nome,
            v.DataVenda,
            v.ValorTotal,
            v.Observacao,
            v.Status.ToString(),
            v.Itens.Select(i => new ItemVendaResponseDto(
                i.Id,
                i.ProdutoServicoId,
                i.ProdutoServico.Nome,
                i.ProdutoServico.CodigoSku,
                i.Quantidade,
                i.PrecoUnitario,
                i.Subtotal
            )).ToList()
        );
    }

    public async Task<VendaResponseDto> RegistrarVendaAsync(CriarVendaDto request, int? usuarioId = null)
    {
        // Regra de Negócio: Uma venda deverá pertencer a uma única unidade e possuir pelo menos um item
        if (request.Itens == null || request.Itens.Count == 0)
        {
            throw new InvalidOperationException("A venda deve conter pelo menos um item.");
        }

        // Regra de Negócio: Uma unidade inativa não poderá registrar novas vendas
        var unidade = await _context.UnidadesFranqueadas.FindAsync(request.UnidadeFranqueadaId);
        if (unidade == null)
        {
            throw new InvalidOperationException("Unidade franqueada não encontrada.");
        }

        if (!unidade.Ativo)
        {
            throw new InvalidOperationException($"A unidade '{unidade.Nome}' está INATIVA e não pode realizar vendas.");
        }

        var venda = new Venda
        {
            UnidadeFranqueadaId = request.UnidadeFranqueadaId,
            UsuarioId = usuarioId,
            DataVenda = DateTime.UtcNow,
            Observacao = request.Observacao,
            Status = StatusVenda.Concluida,
            Itens = new List<ItemVenda>()
        };

        decimal totalVenda = 0;

        foreach (var itemReq in request.Itens)
        {
            if (itemReq.Quantidade <= 0)
            {
                throw new InvalidOperationException("A quantidade do item deve ser maior que zero.");
            }

            var produto = await _context.ProdutosServicos.FindAsync(itemReq.ProdutoServicoId);
            if (produto == null || !produto.Ativo)
            {
                throw new InvalidOperationException($"Produto ID {itemReq.ProdutoServicoId} não encontrado ou está inativo no catálogo.");
            }

            // Se for produto físico, validar e decrementar estoque
            if (produto.Tipo == TipoProdutoServico.Produto)
            {
                var estoque = await _context.Estoques
                    .FirstOrDefaultAsync(e => e.UnidadeFranqueadaId == request.UnidadeFranqueadaId && e.ProdutoServicoId == produto.Id);

                if (estoque == null || estoque.Quantidade < itemReq.Quantidade)
                {
                    int saldo = estoque?.Quantidade ?? 0;
                    throw new InvalidOperationException(
                        $"Estoque insuficiente para o produto '{produto.Nome}'. Saldo disponível na unidade: {saldo}, Quantidade solicitada: {itemReq.Quantidade}."
                    );
                }

                // Regra de Negócio: Atualização do estoque após confirmação da venda
                estoque.Quantidade -= itemReq.Quantidade;
                estoque.UltimaAtualizacao = DateTime.UtcNow;

                // Auditoria de saída de estoque por venda
                await _context.MovimentacoesEstoque.AddAsync(new MovimentacaoEstoque
                {
                    UnidadeFranqueadaId = request.UnidadeFranqueadaId,
                    ProdutoServicoId = produto.Id,
                    Tipo = TipoMovimentacao.Venda,
                    Quantidade = itemReq.Quantidade,
                    Observacao = $"Baixa automática por venda",
                    DataMovimentacao = DateTime.UtcNow,
                    UsuarioId = usuarioId
                });
            }

            decimal subtotal = itemReq.Quantidade * produto.PrecoBase;
            totalVenda += subtotal;

            venda.Itens.Add(new ItemVenda
            {
                ProdutoServicoId = produto.Id,
                Quantidade = itemReq.Quantidade,
                PrecoUnitario = produto.PrecoBase,
                Subtotal = subtotal
            });
        }

        // Regra de Negócio: O valor total da venda deverá ser calculado a partir dos itens, quantidades e preços
        venda.ValorTotal = totalVenda;

        await _context.Vendas.AddAsync(venda);
        await _context.SaveChangesAsync();

        return (await ObterVendaPorIdAsync(venda.Id))!;
    }

    public async Task<bool> CancelarVendaAsync(int id)
    {
        var venda = await _context.Vendas
            .Include(v => v.Itens)
                .ThenInclude(i => i.ProdutoServico)
            .FirstOrDefaultAsync(v => v.Id == id);

        if (venda == null || venda.Status == StatusVenda.Cancelada)
        {
            return false;
        }

        // Devolver estoque dos produtos
        foreach (var item in venda.Itens)
        {
            if (item.ProdutoServico.Tipo == TipoProdutoServico.Produto)
            {
                var estoque = await _context.Estoques
                    .FirstOrDefaultAsync(e => e.UnidadeFranqueadaId == venda.UnidadeFranqueadaId && e.ProdutoServicoId == item.ProdutoServicoId);

                if (estoque != null)
                {
                    estoque.Quantidade += item.Quantidade;
                    estoque.UltimaAtualizacao = DateTime.UtcNow;

                    await _context.MovimentacoesEstoque.AddAsync(new MovimentacaoEstoque
                    {
                        UnidadeFranqueadaId = venda.UnidadeFranqueadaId,
                        ProdutoServicoId = item.ProdutoServicoId,
                        Tipo = TipoMovimentacao.Ajuste,
                        Quantidade = item.Quantidade,
                        Observacao = $"Estorno de estoque por cancelamento da venda #{venda.Id}",
                        DataMovimentacao = DateTime.UtcNow
                    });
                }
            }
        }

        venda.Status = StatusVenda.Cancelada;
        await _context.SaveChangesAsync();
        return true;
    }
}
