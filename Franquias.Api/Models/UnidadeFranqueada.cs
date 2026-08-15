namespace Franquias.Api.Models;

public class UnidadeFranqueada
{
    public int Id { get; set; }
    public int FranqueadoraId { get; set; }
    public Franqueadora Franqueadora { get; set; } = null!;

    public string Nome { get; set; } = string.Empty;
    public string RazaoSocial { get; set; } = string.Empty;
    public string CNPJ { get; set; } = string.Empty;
    public string ResponsavelNome { get; set; } = string.Empty;
    public string ResponsavelEmail { get; set; } = string.Empty;
    public string ResponsavelTelefone { get; set; } = string.Empty;
    public string Cidade { get; set; } = string.Empty;
    public string UF { get; set; } = string.Empty;
    public string Endereco { get; set; } = string.Empty;
    public DateTime DataInicio { get; set; } = DateTime.UtcNow;
    public decimal PercentualRoyalty { get; set; } = 5.0m; // Ex: 5% padrão
    public bool Ativo { get; set; } = true;

    public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
    public ICollection<Estoque> Estoques { get; set; } = new List<Estoque>();
    public ICollection<MovimentacaoEstoque> MovimentacoesEstoque { get; set; } = new List<MovimentacaoEstoque>();
    public ICollection<Venda> Vendas { get; set; } = new List<Venda>();
    public ICollection<Royalty> Royalties { get; set; } = new List<Royalty>();
    public ICollection<ChamadoSuporte> Chamados { get; set; } = new List<ChamadoSuporte>();
}
