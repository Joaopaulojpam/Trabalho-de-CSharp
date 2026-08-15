using Franquias.Api.Models.Enums;

namespace Franquias.Api.Models;

public class Usuario
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string SenhaHash { get; set; } = string.Empty;
    public PerfilUsuario Perfil { get; set; } = PerfilUsuario.Operador;
    public bool Ativo { get; set; } = true;
    public DateTime DataCadastro { get; set; } = DateTime.UtcNow;

    // Vínculo opcional com Unidade (gestores e operadores são vinculados à sua unidade)
    public int? UnidadeFranqueadaId { get; set; }
    public UnidadeFranqueada? UnidadeFranqueada { get; set; }

    public ICollection<Venda> Vendas { get; set; } = new List<Venda>();
    public ICollection<ChamadoSuporte> ChamadosAbertos { get; set; } = new List<ChamadoSuporte>();
    public ICollection<MovimentacaoEstoque> MovimentacoesEstoque { get; set; } = new List<MovimentacaoEstoque>();
}
