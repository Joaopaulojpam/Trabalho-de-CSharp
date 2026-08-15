using Franquias.Api.DTOs;
using Franquias.Api.Models.Enums;

namespace Franquias.Api.Services.Interfaces;

public interface IProdutoService
{
    Task<List<ProdutoResponseDto>> ListarAsync(int? categoriaId = null, bool? ativo = null, string? termoBusca = null, TipoProdutoServico? tipo = null);
    Task<ProdutoResponseDto?> ObterPorIdAsync(int id);
    Task<ProdutoResponseDto> CriarAsync(CriarProdutoDto request);
    Task<ProdutoResponseDto?> AtualizarAsync(int id, AtualizarProdutoDto request);
    Task<bool> AlternarStatusAsync(int id);

    // Categorias
    Task<List<CategoriaResponseDto>> ListarCategoriasAsync(bool? ativo = null);
    Task<CategoriaResponseDto?> ObterCategoriaPorIdAsync(int id);
    Task<CategoriaResponseDto> CriarCategoriaAsync(CriarCategoriaDto request);
    Task<CategoriaResponseDto?> AtualizarCategoriaAsync(int id, AtualizarCategoriaDto request);

    // Fornecedores
    Task<List<FornecedorResponseDto>> ListarFornecedoresAsync(bool? ativo = null, string? termoBusca = null);
    Task<FornecedorResponseDto?> ObterFornecedorPorIdAsync(int id);
    Task<FornecedorResponseDto> CriarFornecedorAsync(CriarFornecedorDto request);
    Task<FornecedorResponseDto?> AtualizarFornecedorAsync(int id, AtualizarFornecedorDto request);
    Task<bool> AlternarStatusFornecedorAsync(int id);
}
