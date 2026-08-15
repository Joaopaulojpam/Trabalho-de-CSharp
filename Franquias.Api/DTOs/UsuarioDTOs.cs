using System.ComponentModel.DataAnnotations;
using Franquias.Api.Models.Enums;

namespace Franquias.Api.DTOs;

public record CriarUsuarioDto(
    [Required(ErrorMessage = "O nome é obrigatório")]
    string Nome,

    [Required(ErrorMessage = "O e-mail é obrigatório")]
    [EmailAddress(ErrorMessage = "E-mail inválido")]
    string Email,

    [Required(ErrorMessage = "A senha é obrigatória")]
    [MinLength(6, ErrorMessage = "A senha deve ter pelo menos 6 caracteres")]
    string Senha,

    [Required(ErrorMessage = "O perfil é obrigatório")]
    PerfilUsuario Perfil,

    int? UnidadeFranqueadaId
);

public record AtualizarUsuarioDto(
    [Required(ErrorMessage = "O nome é obrigatório")]
    string Nome,

    [Required(ErrorMessage = "O e-mail é obrigatório")]
    [EmailAddress(ErrorMessage = "E-mail inválido")]
    string Email,

    string? NovaSenha,

    [Required(ErrorMessage = "O perfil é obrigatório")]
    PerfilUsuario Perfil,

    int? UnidadeFranqueadaId,

    bool Ativo
);

public record UsuarioResponseDto(
    int Id,
    string Nome,
    string Email,
    string Perfil,
    bool Ativo,
    DateTime DataCadastro,
    int? UnidadeFranqueadaId,
    string? NomeUnidade
);
