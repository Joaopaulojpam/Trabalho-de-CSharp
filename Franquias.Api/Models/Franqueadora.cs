namespace Franquias.Api.Models;

public class Franqueadora
{
    public int Id { get; set; }
    public string RazaoSocial { get; set; } = string.Empty;
    public string NomeFantasia { get; set; } = string.Empty;
    public string CNPJ { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty;
    public string Endereco { get; set; } = string.Empty;
    public DateTime DataFundacao { get; set; } = DateTime.UtcNow;

    public ICollection<UnidadeFranqueada> Unidades { get; set; } = new List<UnidadeFranqueada>();
}
