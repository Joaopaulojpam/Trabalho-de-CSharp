namespace Franquias.Api.Models;

public class Fornecedor
{
    public int Id { get; set; }
    public string RazaoSocial { get; set; } = string.Empty;
    public string NomeFantasia { get; set; } = string.Empty;
    public string CNPJ { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty;
    public string Endereco { get; set; } = string.Empty;
    public bool Ativo { get; set; } = true;
    public DateTime DataCadastro { get; set; } = DateTime.UtcNow;

    public ICollection<ProdutoServico> ProdutosFornecidos { get; set; } = new List<ProdutoServico>();
}
