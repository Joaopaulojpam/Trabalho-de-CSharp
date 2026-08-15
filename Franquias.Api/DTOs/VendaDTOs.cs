using System.ComponentModel.DataAnnotations;

namespace Franquias.Api.DTOs;

public record ItemVendaRequestDto(
    [Required(ErrorMessage = "O produto é obrigatório")]
    int ProdutoServicoId,

    [Range(1, 10000, ErrorMessage = "A quantidade do item deve ser de no mínimo 1")]
    int Quantidade
);

public record CriarVendaDto(
    [Required(ErrorMessage = "A unidade é obrigatória")]
    int UnidadeFranqueadaId,

    [Required(ErrorMessage = "A venda deve conter pelo menos um item")]
    [MinLength(1, ErrorMessage = "A venda deve possuir pelo menos um item")]
    List<ItemVendaRequestDto> Itens,

    string Observacao = ""
);

public record ItemVendaResponseDto(
    int Id,
    int ProdutoServicoId,
    string NomeProduto,
    string CodigoSku,
    int Quantidade,
    decimal PrecoUnitario,
    decimal Subtotal
);

public record VendaResponseDto(
    int Id,
    int UnidadeFranqueadaId,
    string NomeUnidade,
    int? UsuarioId,
    string? NomeUsuario,
    DateTime DataVenda,
    decimal ValorTotal,
    string Observacao,
    string Status,
    List<ItemVendaResponseDto> Itens
);
