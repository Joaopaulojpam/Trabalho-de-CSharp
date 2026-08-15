namespace Franquias.Api.Models;

public class ItemVenda
{
    public int Id { get; set; }
    public int VendaId { get; set; }
    public Venda Venda { get; set; } = null!;

    public int ProdutoServicoId { get; set; }
    public ProdutoServico ProdutoServico { get; set; } = null!;

    public int Quantidade { get; set; }
    public decimal PrecoUnitario { get; set; }
    public decimal Subtotal { get; set; }
}
