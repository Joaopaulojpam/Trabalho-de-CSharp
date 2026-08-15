namespace Franquias.Api.DTOs;

public record FaturamentoUnidadeRelatorioDto(
    int UnidadeId,
    string NomeUnidade,
    string Cidade,
    string UF,
    int TotalVendas,
    decimal TotalFaturamento,
    decimal TotalRoyaltiesDevidos
);

public record RankingUnidadeDto(
    int Posicao,
    int UnidadeId,
    string NomeUnidade,
    string Cidade,
    string UF,
    int QuantidadeVendas,
    decimal FaturamentoTotal
);

public record ProdutoMaisVendidoDto(
    int ProdutoId,
    string NomeProduto,
    string CodigoSku,
    string Categoria,
    int QuantidadeTotalVendida,
    decimal ValorTotalGerado
);

public record ResumoRoyaltiesDto(
    int Mes,
    int Ano,
    int TotalUnidadesApuradas,
    decimal FaturamentoTotalRede,
    decimal TotalRoyaltiesCalculados,
    decimal TotalRoyaltiesPagos,
    decimal TotalRoyaltiesPendentes
);

public record IndicadoresGeraisDto(
    int TotalUnidadesAtivas,
    int TotalUnidadesInativas,
    int TotalProdutosCadastrados,
    int TotalItensEstoqueCritico,
    int TotalChamadosAbertos,
    int TotalChamadosEmAtendimento,
    decimal FaturamentoTotalHistorico,
    decimal RoyaltiesTotalHistorico
);
