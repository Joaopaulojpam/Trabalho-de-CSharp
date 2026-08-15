using Franquias.Api.Models.Enums;

namespace Franquias.Api.Models;

public class ProdutoServico
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string CodigoSku { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public decimal PrecoBase { get; set; }
    public TipoProdutoServico Tipo { get; set; } = TipoProdutoServico.Produto;
    public bool Ativo { get; set; } = true;
    public DateTime DataCadastro { get; set; } = DateTime.UtcNow;

    public int CategoriaId { get; set; }
    public Categoria Categoria { get; set; } = null!;

    public int? FornecedorId { get; set; }
    public Fornecedor? Fornecedor { get; set; }

    public ICollection<Estoque> Estoques { get; set; } = new List<Estoque>();
    public ICollection<ItemVenda> ItensVenda { get; set; } = new List<ItemVenda>();
    public ICollection<MovimentacaoEstoque> MovimentacoesEstoque { get; set; } = new List<MovimentacaoEstoque>();
}
