using System.ComponentModel.DataAnnotations;
using Franquias.Api.Models.Enums;

namespace Franquias.Api.DTOs;

public record CriarChamadoDto(
    [Required(ErrorMessage = "A unidade franqueada é obrigatória")]
    int UnidadeFranqueadaId,

    [Required(ErrorMessage = "O título é obrigatório")]
    string Titulo,

    [Required(ErrorMessage = "A descrição é obrigatória")]
    string Descricao,

    [Required(ErrorMessage = "A categoria do chamado é obrigatória")]
    string Categoria,

    PrioridadeChamado Prioridade
);

public record AtualizarStatusChamadoDto(
    [Required(ErrorMessage = "O status é obrigatório")]
    StatusChamado Status,

    string? RespostaSolucao
);

public record ChamadoResponseDto(
    int Id,
    int UnidadeFranqueadaId,
    string NomeUnidade,
    int? UsuarioAberturaId,
    string? NomeUsuarioAbertura,
    string Titulo,
    string Descricao,
    string Categoria,
    string Prioridade,
    string Status,
    DateTime DataAbertura,
    DateTime? DataFechamento,
    string? RespostaSolucao
);
