using Franquias.Api.Models.Enums;

namespace Franquias.Api.Models;

public class ChamadoSuporte
{
    public int Id { get; set; }
    public int UnidadeFranqueadaId { get; set; }
    public UnidadeFranqueada UnidadeFranqueada { get; set; } = null!;

    public int? UsuarioAberturaId { get; set; }
    public Usuario? UsuarioAbertura { get; set; }

    public string Titulo { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public string Categoria { get; set; } = string.Empty; // Ex: Sistema, Suprimentos, Operacional, Financeiro
    public PrioridadeChamado Prioridade { get; set; } = PrioridadeChamado.Media;
    public StatusChamado Status { get; set; } = StatusChamado.Aberto;
    public DateTime DataAbertura { get; set; } = DateTime.UtcNow;
    public DateTime? DataFechamento { get; set; }
    public string? RespostaSolucao { get; set; }
}
