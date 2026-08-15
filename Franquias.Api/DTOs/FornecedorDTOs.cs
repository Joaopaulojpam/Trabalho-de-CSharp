using System.ComponentModel.DataAnnotations;

namespace Franquias.Api.DTOs;

public record CriarFornecedorDto(
    [Required(ErrorMessage = "A razão social é obrigatória")]
    string RazaoSocial,

    [Required(ErrorMessage = "O nome fantasia é obrigatório")]
    string NomeFantasia,

    [Required(ErrorMessage = "O CNPJ é obrigatório")]
    string CNPJ,

    [EmailAddress(ErrorMessage = "E-mail inválido")]
    string Email,

    string Telefone,
    string Endereco
);

public record AtualizarFornecedorDto(
    [Required(ErrorMessage = "A razão social é obrigatória")]
    string RazaoSocial,

    [Required(ErrorMessage = "O nome fantasia é obrigatório")]
    string NomeFantasia,

    [EmailAddress(ErrorMessage = "E-mail inválido")]
    string Email,

    string Telefone,
    string Endereco,
    bool Ativo
);

public record FornecedorResponseDto(
    int Id,
    string RazaoSocial,
    string NomeFantasia,
    string CNPJ,
    string Email,
    string Telefone,
    string Endereco,
    bool Ativo,
    DateTime DataCadastro
);
