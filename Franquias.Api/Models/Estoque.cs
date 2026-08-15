namespace Franquias.Api.Models;

public class Estoque
{
    public int Id { get; set; }
    public int UnidadeFranqueadaId { get; set; }
    public UnidadeFranqueada UnidadeFranqueada { get; set; } = null!;

    public int ProdutoServicoId { get; set; }
    public ProdutoServico ProdutoServico { get; set; } = null!;

    public int Quantidade { get; set; }
    public int QuantidadeMinima { get; set; } = 5;
    public DateTime UltimaAtualizacao { get; set; } = DateTime.UtcNow;
}
