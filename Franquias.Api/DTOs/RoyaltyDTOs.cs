using System.ComponentModel.DataAnnotations;

namespace Franquias.Api.DTOs;

public record GerarRoyaltyDto(
    [Required(ErrorMessage = "A unidade franqueada é obrigatória")]
    int UnidadeFranqueadaId,

    [Range(1, 12, ErrorMessage = "Mês deve estar entre 1 e 12")]
    int MesReferencia,

    [Range(2020, 2100, ErrorMessage = "Ano inválido")]
    int AnoReferencia
);

public record RegistrarPagamentoRoyaltyDto(
    DateTime DataPagamento,
    string Observacao = ""
);

public record RoyaltyResponseDto(
    int Id,
    int UnidadeFranqueadaId,
    string NomeUnidade,
    int MesReferencia,
    int AnoReferencia,
    decimal FaturamentoBase,
    decimal PercentualAplicado,
    decimal ValorCalculado,
    DateTime DataGeracao,
    DateTime DataVencimento,
    string Status,
    DateTime? DataPagamento,
    string Observacao
);
