using Franquias.Api.DTOs;
using Franquias.Api.Models.Enums;
using Franquias.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Franquias.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
public class ProdutosController : ControllerBase
{
    private readonly IProdutoService _produtoService;

    public ProdutosController(IProdutoService produtoService)
    {
        _produtoService = produtoService;
    }

    /// <summary>
    /// Lista produtos e serviços do catálogo com filtros por categoria, status, termo e tipo.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<ProdutoResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Listar(
        [FromQuery] int? categoriaId = null,
        [FromQuery] bool? ativo = null,
        [FromQuery] string? termoBusca = null,
        [FromQuery] TipoProdutoServico? tipo = null)
    {
        var produtos = await _produtoService.ListarAsync(categoriaId, ativo, termoBusca, tipo);
        return Ok(produtos);
    }

    /// <summary>
    /// Obtém detalhes de um produto ou serviço por ID.
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ProdutoResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterPorId(int id)
    {
        var produto = await _produtoService.ObterPorIdAsync(id);
        if (produto == null)
        {
            return NotFound(new { mensagem = $"Produto com ID {id} não encontrado." });
        }
        return Ok(produto);
    }

    /// <summary>
    /// Cadastra um novo produto ou serviço no catálogo padronizado (Acesso restrito: Administrador).
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Administrador")]
    [ProducesResponseType(typeof(ProdutoResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Criar([FromBody] CriarProdutoDto request)
    {
        try
        {
            var produto = await _produtoService.CriarAsync(request);
            return CreatedAtAction(nameof(ObterPorId), new { id = produto.Id }, produto);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    /// <summary>
    /// Atualiza dados de um produto ou serviço existente (Acesso restrito: Administrador).
    /// </summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Administrador")]
    [ProducesResponseType(typeof(ProdutoResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Atualizar(int id, [FromBody] AtualizarProdutoDto request)
    {
        try
        {
            var produto = await _produtoService.AtualizarAsync(id, request);
            if (produto == null)
            {
                return NotFound(new { mensagem = $"Produto com ID {id} não encontrado." });
            }
            return Ok(produto);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    /// <summary>
    /// Ativa ou inativa um produto/serviço no catálogo (Acesso restrito: Administrador).
    /// </summary>
    [HttpPatch("{id:int}/status")]
    [Authorize(Roles = "Administrador")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AlternarStatus(int id)
    {
        var sucesso = await _produtoService.AlternarStatusAsync(id);
        if (!sucesso)
        {
            return NotFound(new { mensagem = $"Produto com ID {id} não encontrado." });
        }
        return Ok(new { mensagem = "Status do produto alterado com sucesso." });
    }
}
