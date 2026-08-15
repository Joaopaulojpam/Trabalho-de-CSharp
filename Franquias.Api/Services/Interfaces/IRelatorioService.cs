using Franquias.Api.DTOs;

namespace Franquias.Api.Services.Interfaces;

public interface IRelatorioService
{
    Task<List<FaturamentoUnidadeRelatorioDto>> ObterFaturamentoPorUnidadesAsync(DateTime? dataInicio, DateTime? dataFim);
    Task<List<RankingUnidadeDto>> ObterRankingUnidadesAsync(DateTime? dataInicio, DateTime? dataFim);
    Task<List<ProdutoMaisVendidoDto>> ObterProdutosMaisVendidosAsync(int top = 10, DateTime? dataInicio = null, DateTime? dataFim = null);
    Task<List<EstoqueResponseDto>> ObterTodosEstoquesCriticosAsync();
    Task<IndicadoresGeraisDto> ObterIndicadoresGeraisAsync();
}
