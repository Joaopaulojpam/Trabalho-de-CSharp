using Franquias.Api.DTOs;
using Franquias.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Franquias.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
public class FornecedoresController : ControllerBase
{
    private readonly IProdutoService _produtoService;

    public FornecedoresController(IProdutoService produtoService)
    {
        _produtoService = produtoService;
    }

    /// <summary>
    /// Lista fornecedores cadastrados com filtros por status e busca por nome/CNPJ.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<FornecedorResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Listar([FromQuery] bool? ativo = null, [FromQuery] string? termoBusca = null)
    {
        var fornecedores = await _produtoService.ListarFornecedoresAsync(ativo, termoBusca);
        return Ok(fornecedores);
    }

    /// <summary>
    /// Obtém detalhes de um fornecedor por ID.
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(FornecedorResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterPorId(int id)
    {
        var f = await _produtoService.ObterFornecedorPorIdAsync(id);
        if (f == null)
        {
            return NotFound(new { mensagem = $"Fornecedor com ID {id} não encontrado." });
        }
        return Ok(f);
    }

    /// <summary>
    /// Cadastra um novo fornecedor homologado pela franqueadora (Acesso restrito: Administrador).
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Administrador")]
    [ProducesResponseType(typeof(FornecedorResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Criar([FromBody] CriarFornecedorDto request)
    {
        try
        {
            var f = await _produtoService.CriarFornecedorAsync(request);
            return CreatedAtAction(nameof(ObterPorId), new { id = f.Id }, f);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    /// <summary>
    /// Atualiza dados de um fornecedor existente (Acesso restrito: Administrador).
    /// </summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Administrador")]
    [ProducesResponseType(typeof(FornecedorResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Atualizar(int id, [FromBody] AtualizarFornecedorDto request)
    {
        var f = await _produtoService.AtualizarFornecedorAsync(id, request);
        if (f == null)
        {
            return NotFound(new { mensagem = $"Fornecedor com ID {id} não encontrado." });
        }
        return Ok(f);
    }

    /// <summary>
    /// Ativa ou inativa um fornecedor (Acesso restrito: Administrador).
    /// </summary>
    [HttpPatch("{id:int}/status")]
    [Authorize(Roles = "Administrador")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AlternarStatus(int id)
    {
        var sucesso = await _produtoService.AlternarStatusFornecedorAsync(id);
        if (!sucesso)
        {
            return NotFound(new { mensagem = $"Fornecedor com ID {id} não encontrado." });
        }
        return Ok(new { mensagem = "Status do fornecedor alterado com sucesso." });
    }
}
