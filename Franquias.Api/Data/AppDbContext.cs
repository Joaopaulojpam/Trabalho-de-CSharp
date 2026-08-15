using Microsoft.EntityFrameworkCore;
using Franquias.Api.Models;

namespace Franquias.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Franqueadora> Franqueadoras => Set<Franqueadora>();
    public DbSet<UnidadeFranqueada> UnidadesFranqueadas => Set<UnidadeFranqueada>();
    public DbSet<Categoria> Categorias => Set<Categoria>();
    public DbSet<ProdutoServico> ProdutosServicos => Set<ProdutoServico>();
    public DbSet<Fornecedor> Fornecedores => Set<Fornecedor>();
    public DbSet<Estoque> Estoques => Set<Estoque>();
    public DbSet<MovimentacaoEstoque> MovimentacoesEstoque => Set<MovimentacaoEstoque>();
    public DbSet<Venda> Vendas => Set<Venda>();
    public DbSet<ItemVenda> ItensVenda => Set<ItemVenda>();
    public DbSet<Royalty> Royalties => Set<Royalty>();
    public DbSet<ChamadoSuporte> ChamadosSuporte => Set<ChamadoSuporte>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Regra de Negócio: E-mail de usuário único
        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.Nome).IsRequired().HasMaxLength(150);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(150);
            entity.Property(e => e.SenhaHash).IsRequired();

            entity.HasOne(e => e.UnidadeFranqueada)
                  .WithMany(u => u.Usuarios)
                  .HasForeignKey(e => e.UnidadeFranqueadaId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        // Franqueadora
        modelBuilder.Entity<Franqueadora>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.CNPJ).IsUnique();
            entity.Property(e => e.RazaoSocial).IsRequired().HasMaxLength(200);
            entity.Property(e => e.NomeFantasia).IsRequired().HasMaxLength(150);
            entity.Property(e => e.CNPJ).IsRequired().HasMaxLength(20);
        });

        // Regra de Negócio: CNPJ de Unidade Franqueada único
        modelBuilder.Entity<UnidadeFranqueada>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.CNPJ).IsUnique();
            entity.Property(e => e.Nome).IsRequired().HasMaxLength(150);
            entity.Property(e => e.RazaoSocial).IsRequired().HasMaxLength(200);
            entity.Property(e => e.CNPJ).IsRequired().HasMaxLength(20);
            entity.Property(e => e.PercentualRoyalty).HasPrecision(5, 2);

            entity.HasOne(e => e.Franqueadora)
                  .WithMany(f => f.Unidades)
                  .HasForeignKey(e => e.FranqueadoraId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Categoria
        modelBuilder.Entity<Categoria>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nome).IsRequired().HasMaxLength(100);
        });

        // Fornecedor
        modelBuilder.Entity<Fornecedor>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.CNPJ).IsUnique();
            entity.Property(e => e.RazaoSocial).IsRequired().HasMaxLength(200);
            entity.Property(e => e.NomeFantasia).IsRequired().HasMaxLength(150);
            entity.Property(e => e.CNPJ).IsRequired().HasMaxLength(20);
        });

        // ProdutoServico
        modelBuilder.Entity<ProdutoServico>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.CodigoSku).IsUnique();
            entity.Property(e => e.Nome).IsRequired().HasMaxLength(150);
            entity.Property(e => e.CodigoSku).IsRequired().HasMaxLength(50);
            entity.Property(e => e.PrecoBase).HasPrecision(12, 2);

            entity.HasOne(e => e.Categoria)
                  .WithMany(c => c.Produtos)
                  .HasForeignKey(e => e.CategoriaId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Fornecedor)
                  .WithMany(f => f.ProdutosFornecidos)
                  .HasForeignKey(e => e.FornecedorId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        // Estoque
        modelBuilder.Entity<Estoque>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.UnidadeFranqueadaId, e.ProdutoServicoId }).IsUnique();

            entity.HasOne(e => e.UnidadeFranqueada)
                  .WithMany(u => u.Estoques)
                  .HasForeignKey(e => e.UnidadeFranqueadaId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.ProdutoServico)
                  .WithMany(p => p.Estoques)
                  .HasForeignKey(e => e.ProdutoServicoId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // MovimentacaoEstoque
        modelBuilder.Entity<MovimentacaoEstoque>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasOne(e => e.UnidadeFranqueada)
                  .WithMany(u => u.MovimentacoesEstoque)
                  .HasForeignKey(e => e.UnidadeFranqueadaId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.ProdutoServico)
                  .WithMany(p => p.MovimentacoesEstoque)
                  .HasForeignKey(e => e.ProdutoServicoId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Usuario)
                  .WithMany(u => u.MovimentacoesEstoque)
                  .HasForeignKey(e => e.UsuarioId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        // Venda
        modelBuilder.Entity<Venda>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ValorTotal).HasPrecision(12, 2);

            entity.HasOne(e => e.UnidadeFranqueada)
                  .WithMany(u => u.Vendas)
                  .HasForeignKey(e => e.UnidadeFranqueadaId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Usuario)
                  .WithMany(u => u.Vendas)
                  .HasForeignKey(e => e.UsuarioId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        // ItemVenda
        modelBuilder.Entity<ItemVenda>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.PrecoUnitario).HasPrecision(12, 2);
            entity.Property(e => e.Subtotal).HasPrecision(12, 2);

            entity.HasOne(e => e.Venda)
                  .WithMany(v => v.Itens)
                  .HasForeignKey(e => e.VendaId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.ProdutoServico)
                  .WithMany(p => p.ItensVenda)
                  .HasForeignKey(e => e.ProdutoServicoId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Royalty
        modelBuilder.Entity<Royalty>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.UnidadeFranqueadaId, e.MesReferencia, e.AnoReferencia }).IsUnique();
            entity.Property(e => e.FaturamentoBase).HasPrecision(12, 2);
            entity.Property(e => e.PercentualAplicado).HasPrecision(5, 2);
            entity.Property(e => e.ValorCalculado).HasPrecision(12, 2);

            entity.HasOne(e => e.UnidadeFranqueada)
                  .WithMany(u => u.Royalties)
                  .HasForeignKey(e => e.UnidadeFranqueadaId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // ChamadoSuporte
        modelBuilder.Entity<ChamadoSuporte>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Titulo).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Categoria).IsRequired().HasMaxLength(100);

            entity.HasOne(e => e.UnidadeFranqueada)
                  .WithMany(u => u.Chamados)
                  .HasForeignKey(e => e.UnidadeFranqueadaId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.UsuarioAbertura)
                  .WithMany(u => u.ChamadosAbertos)
                  .HasForeignKey(e => e.UsuarioAberturaId)
                  .OnDelete(DeleteBehavior.SetNull);
        });
    }
}
