using System.ComponentModel.DataAnnotations;
using Franquias.Api.Models.Enums;

namespace Franquias.Api.DTOs;

public record MovimentarEstoqueDto(
    [Required(ErrorMessage = "A unidade franqueada é obrigatória")]
    int UnidadeFranqueadaId,

    [Required(ErrorMessage = "O produto é obrigatório")]
    int ProdutoServicoId,

    [Required(ErrorMessage = "O tipo de movimentação é obrigatório")]
    TipoMovimentacao Tipo,

    [Range(1, 100000, ErrorMessage = "A quantidade deve ser maior que zero")]
    int Quantidade,

    string Observacao
);

public record DefinirEstoqueMinimoDto(
    [Range(0, 100000, ErrorMessage = "A quantidade mínima não pode ser negativa")]
    int QuantidadeMinima
);

public record EstoqueResponseDto(
    int Id,
    int UnidadeFranqueadaId,
    string NomeUnidade,
    int ProdutoServicoId,
    string NomeProduto,
    string CodigoSku,
    int Quantidade,
    int QuantidadeMinima,
    bool EstoqueCritico,
    DateTime UltimaAtualizacao
);

public record MovimentacaoResponseDto(
    int Id,
    int UnidadeFranqueadaId,
    string NomeUnidade,
    int ProdutoServicoId,
    string NomeProduto,
    string Tipo,
    int Quantidade,
    string Observacao,
    DateTime DataMovimentacao,
    string? NomeUsuario
);
