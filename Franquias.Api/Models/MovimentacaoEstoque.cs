using Franquias.Api.Models.Enums;

namespace Franquias.Api.Models;

public class MovimentacaoEstoque
{
    public int Id { get; set; }
    public int UnidadeFranqueadaId { get; set; }
    public UnidadeFranqueada UnidadeFranqueada { get; set; } = null!;

    public int ProdutoServicoId { get; set; }
    public ProdutoServico ProdutoServico { get; set; } = null!;

    public TipoMovimentacao Tipo { get; set; } = TipoMovimentacao.Entrada;
    public int Quantidade { get; set; }
    public string Observacao { get; set; } = string.Empty;
    public DateTime DataMovimentacao { get; set; } = DateTime.UtcNow;

    public int? UsuarioId { get; set; }
    public Usuario? Usuario { get; set; }
}
