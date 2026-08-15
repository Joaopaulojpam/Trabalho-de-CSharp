using Franquias.Api.DTOs;
using Franquias.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Franquias.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
public class RelatoriosController : ControllerBase
{
    private readonly IRelatorioService _relatorioService;

    public RelatoriosController(IRelatorioService relatorioService)
    {
        _relatorioService = relatorioService;
    }

    /// <summary>
    /// Relatório de faturamento e royalties por unidade em um período específico.
    /// </summary>
    [HttpGet("faturamento-unidades")]
    [ProducesResponseType(typeof(List<FaturamentoUnidadeRelatorioDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> FaturamentoPorUnidades([FromQuery] DateTime? dataInicio = null, [FromQuery] DateTime? dataFim = null)
    {
        var relatorio = await _relatorioService.ObterFaturamentoPorUnidadesAsync(dataInicio, dataFim);
        return Ok(relatorio);
    }

    /// <summary>
    /// Ranking de unidades franqueadas ordenadas por faturamento decrescente no período.
    /// </summary>
    [HttpGet("ranking-unidades")]
    [ProducesResponseType(typeof(List<RankingUnidadeDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> RankingUnidades([FromQuery] DateTime? dataInicio = null, [FromQuery] DateTime? dataFim = null)
    {
        var ranking = await _relatorioService.ObterRankingUnidadesAsync(dataInicio, dataFim);
        return Ok(ranking);
    }

    /// <summary>
    /// Relatório dos produtos e serviços mais vendidos na rede.
    /// </summary>
    [HttpGet("produtos-mais-vendidos")]
    [ProducesResponseType(typeof(List<ProdutoMaisVendidoDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ProdutosMaisVendidos(
        [FromQuery] int top = 10,
        [FromQuery] DateTime? dataInicio = null,
        [FromQuery] DateTime? dataFim = null)
    {
        var produtos = await _relatorioService.ObterProdutosMaisVendidosAsync(top, dataInicio, dataFim);
        return Ok(produtos);
    }

    /// <summary>
    /// Relatório geral de todos os itens com estoque crítico (abaixo ou igual ao estoque mínimo) em todas as unidades.
    /// </summary>
    [HttpGet("estoque-critico")]
    [ProducesResponseType(typeof(List<EstoqueResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> EstoqueCritico()
    {
        var criticos = await _relatorioService.ObterTodosEstoquesCriticosAsync();
        return Ok(criticos);
    }

    /// <summary>
    /// Indicadores gerais e consolidados para o dashboard da franqueadora (total de unidades, produtos, chamados, faturamento global).
    /// </summary>
    [HttpGet("indicadores-gerais")]
    [ProducesResponseType(typeof(IndicadoresGeraisDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> IndicadoresGerais()
    {
        var indicadores = await _relatorioService.ObterIndicadoresGeraisAsync();
        return Ok(indicadores);
    }
}
