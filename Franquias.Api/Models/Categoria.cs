namespace Franquias.Api.Models;

public class Categoria
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public bool Ativo { get; set; } = true;

    public ICollection<ProdutoServico> Produtos { get; set; } = new List<ProdutoServico>();
}
