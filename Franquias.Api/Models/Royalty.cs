using Franquias.Api.Models.Enums;

namespace Franquias.Api.Models;

public class Royalty
{
    public int Id { get; set; }
    public int UnidadeFranqueadaId { get; set; }
    public UnidadeFranqueada UnidadeFranqueada { get; set; } = null!;

    public int MesReferencia { get; set; }
    public int AnoReferencia { get; set; }
    public decimal FaturamentoBase { get; set; }
    public decimal PercentualAplicado { get; set; }
    public decimal ValorCalculado { get; set; }
    public DateTime DataGeracao { get; set; } = DateTime.UtcNow;
    public DateTime DataVencimento { get; set; }
    public StatusRoyalty Status { get; set; } = StatusRoyalty.Pendente;
    public DateTime? DataPagamento { get; set; }
    public string Observacao { get; set; } = string.Empty;
}
