using Franquias.Api.Data;
using Franquias.Api.DTOs;
using Franquias.Api.Models;
using Franquias.Api.Models.Enums;
using Franquias.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Franquias.Api.Services.Implementations;

public class ProdutoService : IProdutoService
{
    private readonly AppDbContext _context;

    public ProdutoService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<ProdutoResponseDto>> ListarAsync(int? categoriaId = null, bool? ativo = null, string? termoBusca = null, TipoProdutoServico? tipo = null)
    {
        var query = _context.ProdutosServicos
            .Include(p => p.Categoria)
            .Include(p => p.Fornecedor)
            .AsQueryable();

        if (categoriaId.HasValue)
        {
            query = query.Where(p => p.CategoriaId == categoriaId.Value);
        }

        if (ativo.HasValue)
        {
            query = query.Where(p => p.Ativo == ativo.Value);
        }

        if (tipo.HasValue)
        {
            query = query.Where(p => p.Tipo == tipo.Value);
        }

        if (!string.IsNullOrWhiteSpace(termoBusca))
        {
            var termo = termoBusca.ToLower();
            query = query.Where(p =>
                p.Nome.ToLower().Contains(termo) ||
                p.CodigoSku.ToLower().Contains(termo) ||
                p.Descricao.ToLower().Contains(termo) ||
                p.Categoria.Nome.ToLower().Contains(termo)
            );
        }

        return await query
            .OrderBy(p => p.Nome)
            .Select(p => new ProdutoResponseDto(
                p.Id,
                p.Nome,
                p.CodigoSku,
                p.Descricao,
                p.PrecoBase,
                p.Tipo.ToString(),
                p.Ativo,
                p.CategoriaId,
                p.Categoria.Nome,
                p.FornecedorId,
                p.Fornecedor != null ? p.Fornecedor.NomeFantasia : null,
                p.DataCadastro
            ))
            .ToListAsync();
    }

    public async Task<ProdutoResponseDto?> ObterPorIdAsync(int id)
    {
        var p = await _context.ProdutosServicos
            .Include(p => p.Categoria)
            .Include(p => p.Fornecedor)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (p == null) return null;

        return new ProdutoResponseDto(
            p.Id,
            p.Nome,
            p.CodigoSku,
            p.Descricao,
            p.PrecoBase,
            p.Tipo.ToString(),
            p.Ativo,
            p.CategoriaId,
            p.Categoria.Nome,
            p.FornecedorId,
            p.Fornecedor?.NomeFantasia,
            p.DataCadastro
        );
    }

    public async Task<ProdutoResponseDto> CriarAsync(CriarProdutoDto request)
    {
        bool skuExiste = await _context.ProdutosServicos
            .AnyAsync(p => p.CodigoSku.ToLower() == request.CodigoSku.ToLower());

        if (skuExiste)
        {
            throw new InvalidOperationException($"Já existe um produto ou serviço com o código SKU '{request.CodigoSku}'.");
        }

        var categoriaExiste = await _context.Categorias.AnyAsync(c => c.Id == request.CategoriaId);
        if (!categoriaExiste)
        {
            throw new InvalidOperationException("Categoria informada não existe.");
        }

        if (request.FornecedorId.HasValue)
        {
            var fornecedorExiste = await _context.Fornecedores.AnyAsync(f => f.Id == request.FornecedorId.Value);
            if (!fornecedorExiste)
            {
                throw new InvalidOperationException("Fornecedor informado não existe.");
            }
        }

        var produto = new ProdutoServico
        {
            Nome = request.Nome,
            CodigoSku = request.CodigoSku.ToUpper(),
            Descricao = request.Descricao,
            PrecoBase = request.PrecoBase,
            Tipo = request.Tipo,
            CategoriaId = request.CategoriaId,
            FornecedorId = request.FornecedorId,
            Ativo = true,
            DataCadastro = DateTime.UtcNow
        };

        await _context.ProdutosServicos.AddAsync(produto);
        await _context.SaveChangesAsync();

        // Se for um produto físico, criar linha de estoque em todas as unidades existentes
        if (produto.Tipo == TipoProdutoServico.Produto)
        {
            var unidades = await _context.UnidadesFranqueadas.ToListAsync();
            foreach (var u in unidades)
            {
                await _context.Estoques.AddAsync(new Estoque
                {
                    UnidadeFranqueadaId = u.Id,
                    ProdutoServicoId = produto.Id,
                    Quantidade = 0,
                    QuantidadeMinima = 5,
                    UltimaAtualizacao = DateTime.UtcNow
                });
            }
            await _context.SaveChangesAsync();
        }

        return (await ObterPorIdAsync(produto.Id))!;
    }

    public async Task<ProdutoResponseDto?> AtualizarAsync(int id, AtualizarProdutoDto request)
    {
        var produto = await _context.ProdutosServicos.FindAsync(id);
        if (produto == null) return null;

        bool skuExisteOutro = await _context.ProdutosServicos
            .AnyAsync(p => p.CodigoSku.ToLower() == request.CodigoSku.ToLower() && p.Id != id);

        if (skuExisteOutro)
        {
            throw new InvalidOperationException($"O SKU '{request.CodigoSku}' já está em uso por outro produto.");
        }

        produto.Nome = request.Nome;
        produto.CodigoSku = request.CodigoSku.ToUpper();
        produto.Descricao = request.Descricao;
        produto.PrecoBase = request.PrecoBase;
        produto.Tipo = request.Tipo;
        produto.CategoriaId = request.CategoriaId;
        produto.FornecedorId = request.FornecedorId;
        produto.Ativo = request.Ativo;

        await _context.SaveChangesAsync();
        return await ObterPorIdAsync(id);
    }

    public async Task<bool> AlternarStatusAsync(int id)
    {
        var produto = await _context.ProdutosServicos.FindAsync(id);
        if (produto == null) return false;

        produto.Ativo = !produto.Ativo;
        await _context.SaveChangesAsync();
        return true;
    }

    // ================= CATEGORIAS =================
    public async Task<List<CategoriaResponseDto>> ListarCategoriasAsync(bool? ativo = null)
    {
        var query = _context.Categorias.AsQueryable();
        if (ativo.HasValue) query = query.Where(c => c.Ativo == ativo.Value);

        return await query
            .OrderBy(c => c.Nome)
            .Select(c => new CategoriaResponseDto(c.Id, c.Nome, c.Descricao, c.Ativo))
            .ToListAsync();
    }

    public async Task<CategoriaResponseDto?> ObterCategoriaPorIdAsync(int id)
    {
        var c = await _context.Categorias.FindAsync(id);
        if (c == null) return null;
        return new CategoriaResponseDto(c.Id, c.Nome, c.Descricao, c.Ativo);
    }

    public async Task<CategoriaResponseDto> CriarCategoriaAsync(CriarCategoriaDto request)
    {
        var cat = new Categoria
        {
            Nome = request.Nome,
            Descricao = request.Descricao,
            Ativo = true
        };
        await _context.Categorias.AddAsync(cat);
        await _context.SaveChangesAsync();
        return new CategoriaResponseDto(cat.Id, cat.Nome, cat.Descricao, cat.Ativo);
    }

    public async Task<CategoriaResponseDto?> AtualizarCategoriaAsync(int id, AtualizarCategoriaDto request)
    {
        var cat = await _context.Categorias.FindAsync(id);
        if (cat == null) return null;

        cat.Nome = request.Nome;
        cat.Descricao = request.Descricao;
        cat.Ativo = request.Ativo;
        await _context.SaveChangesAsync();

        return new CategoriaResponseDto(cat.Id, cat.Nome, cat.Descricao, cat.Ativo);
    }

    // ================= FORNECEDORES =================
    public async Task<List<FornecedorResponseDto>> ListarFornecedoresAsync(bool? ativo = null, string? termoBusca = null)
    {
        var query = _context.Fornecedores.AsQueryable();
        if (ativo.HasValue) query = query.Where(f => f.Ativo == ativo.Value);
        if (!string.IsNullOrWhiteSpace(termoBusca))
        {
            var termo = termoBusca.ToLower();
            query = query.Where(f =>
                f.RazaoSocial.ToLower().Contains(termo) ||
                f.NomeFantasia.ToLower().Contains(termo) ||
                f.CNPJ.Contains(termo)
            );
        }

        return await query
            .OrderBy(f => f.NomeFantasia)
            .Select(f => new FornecedorResponseDto(
                f.Id,
                f.RazaoSocial,
                f.NomeFantasia,
                f.CNPJ,
                f.Email,
                f.Telefone,
                f.Endereco,
                f.Ativo,
                f.DataCadastro
            ))
            .ToListAsync();
    }

    public async Task<FornecedorResponseDto?> ObterFornecedorPorIdAsync(int id)
    {
        var f = await _context.Fornecedores.FindAsync(id);
        if (f == null) return null;
        return new FornecedorResponseDto(
            f.Id,
            f.RazaoSocial,
            f.NomeFantasia,
            f.CNPJ,
            f.Email,
            f.Telefone,
            f.Endereco,
            f.Ativo,
            f.DataCadastro
        );
    }

    public async Task<FornecedorResponseDto> CriarFornecedorAsync(CriarFornecedorDto request)
    {
        bool cnpjExiste = await _context.Fornecedores.AnyAsync(f => f.CNPJ == request.CNPJ);
        if (cnpjExiste)
        {
            throw new InvalidOperationException($"Já existe um fornecedor cadastrado com o CNPJ '{request.CNPJ}'.");
        }

        var fornecedor = new Fornecedor
        {
            RazaoSocial = request.RazaoSocial,
            NomeFantasia = request.NomeFantasia,
            CNPJ = request.CNPJ,
            Email = request.Email,
            Telefone = request.Telefone,
            Endereco = request.Endereco,
            Ativo = true,
            DataCadastro = DateTime.UtcNow
        };

        await _context.Fornecedores.AddAsync(fornecedor);
        await _context.SaveChangesAsync();

        return new FornecedorResponseDto(
            fornecedor.Id,
            fornecedor.RazaoSocial,
            fornecedor.NomeFantasia,
            fornecedor.CNPJ,
            fornecedor.Email,
            fornecedor.Telefone,
            fornecedor.Endereco,
            fornecedor.Ativo,
            fornecedor.DataCadastro
        );
    }

    public async Task<FornecedorResponseDto?> AtualizarFornecedorAsync(int id, AtualizarFornecedorDto request)
    {
        var fornecedor = await _context.Fornecedores.FindAsync(id);
        if (fornecedor == null) return null;

        fornecedor.RazaoSocial = request.RazaoSocial;
        fornecedor.NomeFantasia = request.NomeFantasia;
        fornecedor.Email = request.Email;
        fornecedor.Telefone = request.Telefone;
        fornecedor.Endereco = request.Endereco;
        fornecedor.Ativo = request.Ativo;

        await _context.SaveChangesAsync();

        return new FornecedorResponseDto(
            fornecedor.Id,
            fornecedor.RazaoSocial,
            fornecedor.NomeFantasia,
            fornecedor.CNPJ,
            fornecedor.Email,
            fornecedor.Telefone,
            fornecedor.Endereco,
            fornecedor.Ativo,
            fornecedor.DataCadastro
        );
    }

    public async Task<bool> AlternarStatusFornecedorAsync(int id)
    {
        var f = await _context.Fornecedores.FindAsync(id);
        if (f == null) return false;

        f.Ativo = !f.Ativo;
        await _context.SaveChangesAsync();
        return true;
    }
}
