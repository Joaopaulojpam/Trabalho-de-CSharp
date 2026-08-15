using System.ComponentModel.DataAnnotations;

namespace Franquias.Api.DTOs;

public record CriarCategoriaDto(
    [Required(ErrorMessage = "O nome da categoria é obrigatório")]
    string Nome,
    string Descricao
);

public record AtualizarCategoriaDto(
    [Required(ErrorMessage = "O nome da categoria é obrigatório")]
    string Nome,
    string Descricao,
    bool Ativo
);

public record CategoriaResponseDto(
    int Id,
    string Nome,
    string Descricao,
    bool Ativo
);
