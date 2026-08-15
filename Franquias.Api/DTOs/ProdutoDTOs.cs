using System.ComponentModel.DataAnnotations;
using Franquias.Api.Models.Enums;

namespace Franquias.Api.DTOs;

public record CriarProdutoDto(
    [Required(ErrorMessage = "O nome do produto é obrigatório")]
    string Nome,

    [Required(ErrorMessage = "O código SKU é obrigatório")]
    string CodigoSku,

    string Descricao,

    [Range(0.01, 1000000.00, ErrorMessage = "O preço base deve ser maior que zero")]
    decimal PrecoBase,

    TipoProdutoServico Tipo,

    [Required(ErrorMessage = "A categoria é obrigatória")]
    int CategoriaId,

    int? FornecedorId
);

public record AtualizarProdutoDto(
    [Required(ErrorMessage = "O nome do produto é obrigatório")]
    string Nome,

    [Required(ErrorMessage = "O código SKU é obrigatório")]
    string CodigoSku,

    string Descricao,

    [Range(0.01, 1000000.00, ErrorMessage = "O preço base deve ser maior que zero")]
    decimal PrecoBase,

    TipoProdutoServico Tipo,

    [Required(ErrorMessage = "A categoria é obrigatória")]
    int CategoriaId,

    int? FornecedorId,

    bool Ativo
);

public record ProdutoResponseDto(
    int Id,
    string Nome,
    string CodigoSku,
    string Descricao,
    decimal PrecoBase,
    string Tipo,
    bool Ativo,
    int CategoriaId,
    string NomeCategoria,
    int? FornecedorId,
    string? NomeFornecedor,
    DateTime DataCadastro
);
