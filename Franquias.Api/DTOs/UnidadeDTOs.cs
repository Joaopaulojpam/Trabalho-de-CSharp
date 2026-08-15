using System.ComponentModel.DataAnnotations;

namespace Franquias.Api.DTOs;

public record CriarUnidadeDto(
    [Required(ErrorMessage = "O nome da unidade é obrigatório")]
    string Nome,

    [Required(ErrorMessage = "A razão social é obrigatória")]
    string RazaoSocial,

    [Required(ErrorMessage = "O CNPJ é obrigatório")]
    [RegularExpression(@"^\d{2}\.?\d{3}\.?\d{3}\/?\d{4}\-?\d{2}$|^\d{14}$", ErrorMessage = "CNPJ inválido")]
    string CNPJ,

    [Required(ErrorMessage = "O nome do responsável é obrigatório")]
    string ResponsavelNome,

    [Required(ErrorMessage = "O e-mail do responsável é obrigatório")]
    [EmailAddress(ErrorMessage = "E-mail inválido")]
    string ResponsavelEmail,

    string ResponsavelTelefone,
    string Cidade,
    string UF,
    string Endereco,

    [Range(0, 100, ErrorMessage = "O percentual de royalty deve estar entre 0% e 100%")]
    decimal PercentualRoyalty,

    int FranqueadoraId = 1
);

public record AtualizarUnidadeDto(
    [Required(ErrorMessage = "O nome da unidade é obrigatório")]
    string Nome,

    [Required(ErrorMessage = "A razão social é obrigatória")]
    string RazaoSocial,

    [Required(ErrorMessage = "O nome do responsável é obrigatório")]
    string ResponsavelNome,

    [Required(ErrorMessage = "O e-mail do responsável é obrigatório")]
    [EmailAddress(ErrorMessage = "E-mail inválido")]
    string ResponsavelEmail,

    string ResponsavelTelefone,
    string Cidade,
    string UF,
    string Endereco,

    [Range(0, 100, ErrorMessage = "O percentual de royalty deve estar entre 0% e 100%")]
    decimal PercentualRoyalty,

    bool Ativo
);

public record UnidadeResponseDto(
    int Id,
    int FranqueadoraId,
    string Nome,
    string RazaoSocial,
    string CNPJ,
    string ResponsavelNome,
    string ResponsavelEmail,
    string ResponsavelTelefone,
    string Cidade,
    string UF,
    string Endereco,
    DateTime DataInicio,
    decimal PercentualRoyalty,
    bool Ativo
);
