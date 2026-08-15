using System.ComponentModel.DataAnnotations;
using Franquias.Api.Models.Enums;

namespace Franquias.Api.DTOs;

public record LoginRequestDto(
    [Required(ErrorMessage = "O e-mail é obrigatório")]
    [EmailAddress(ErrorMessage = "Formato de e-mail inválido")]
    string Email,

    [Required(ErrorMessage = "A senha é obrigatória")]
    string Senha
);

public record LoginResponseDto(
    string Token,
    int UsuarioId,
    string Nome,
    string Email,
    string Perfil,
    int? UnidadeId,
    string? NomeUnidade,
    DateTime Expiracao
);

public record UsuarioLogadoDto(
    int Id,
    string Nome,
    string Email,
    string Perfil,
    int? UnidadeFranqueadaId
);
