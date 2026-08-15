using Franquias.Api.DTOs;
using Franquias.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Franquias.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
public class CategoriasController : ControllerBase
{
    private readonly IProdutoService _produtoService;

    public CategoriasController(IProdutoService produtoService)
    {
        _produtoService = produtoService;
    }

    /// <summary>
    /// Lista todas as categorias de produtos e serviços.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<CategoriaResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Listar([FromQuery] bool? ativo = null)
    {
        var categorias = await _produtoService.ListarCategoriasAsync(ativo);
        return Ok(categorias);
    }

    /// <summary>
    /// Obtém detalhes de uma categoria por ID.
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(CategoriaResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterPorId(int id)
    {
        var cat = await _produtoService.ObterCategoriaPorIdAsync(id);
        if (cat == null)
        {
            return NotFound(new { mensagem = $"Categoria com ID {id} não encontrada." });
        }
        return Ok(cat);
    }

    /// <summary>
    /// Cadastra uma nova categoria de catálogo (Acesso restrito: Administrador).
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Administrador")]
    [ProducesResponseType(typeof(CategoriaResponseDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> Criar([FromBody] CriarCategoriaDto request)
    {
        var cat = await _produtoService.CriarCategoriaAsync(request);
        return CreatedAtAction(nameof(ObterPorId), new { id = cat.Id }, cat);
    }

    /// <summary>
    /// Atualiza uma categoria existente (Acesso restrito: Administrador).
    /// </summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Administrador")]
    [ProducesResponseType(typeof(CategoriaResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Atualizar(int id, [FromBody] AtualizarCategoriaDto request)
    {
        var cat = await _produtoService.AtualizarCategoriaAsync(id, request);
        if (cat == null)
        {
            return NotFound(new { mensagem = $"Categoria com ID {id} não encontrada." });
        }
        return Ok(cat);
    }
}
