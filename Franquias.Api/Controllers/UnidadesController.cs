using Franquias.Api.DTOs;
using Franquias.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Franquias.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
public class UnidadesController : ControllerBase
{
    private readonly IUnidadeService _unidadeService;

    public UnidadesController(IUnidadeService unidadeService)
    {
        _unidadeService = unidadeService;
    }

    /// <summary>
    /// Lista unidades franqueadas com opções de filtros por status, cidade, UF ou busca por CNPJ, nome e responsável.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<UnidadeResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Listar(
        [FromQuery] bool? ativo = null,
        [FromQuery] string? termoBusca = null,
        [FromQuery] string? cidade = null,
        [FromQuery] string? uf = null)
    {
        var unidades = await _unidadeService.ListarAsync(ativo, termoBusca, cidade, uf);
        return Ok(unidades);
    }

    /// <summary>
    /// Obtém detalhes de uma unidade franqueada específica por ID.
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(UnidadeResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterPorId(int id)
    {
        var unidade = await _unidadeService.ObterPorIdAsync(id);
        if (unidade == null)
        {
            return NotFound(new { mensagem = $"Unidade franqueada com ID {id} não encontrada." });
        }
        return Ok(unidade);
    }

    /// <summary>
    /// Cadastra uma nova unidade franqueada (Acesso restrito: Administrador).
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Administrador")]
    [ProducesResponseType(typeof(UnidadeResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Criar([FromBody] CriarUnidadeDto request)
    {
        try
        {
            var unidade = await _unidadeService.CriarAsync(request);
            return CreatedAtAction(nameof(ObterPorId), new { id = unidade.Id }, unidade);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    /// <summary>
    /// Atualiza os dados de uma unidade franqueada existente (Acesso restrito: Administrador, Gestor).
    /// </summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Administrador,Gestor")]
    [ProducesResponseType(typeof(UnidadeResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Atualizar(int id, [FromBody] AtualizarUnidadeDto request)
    {
        try
        {
            var unidade = await _unidadeService.AtualizarAsync(id, request);
            if (unidade == null)
            {
                return NotFound(new { mensagem = $"Unidade franqueada com ID {id} não encontrada." });
            }
            return Ok(unidade);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    /// <summary>
    /// Ativa ou inativa uma unidade franqueada (Acesso restrito: Administrador).
    /// </summary>
    [HttpPatch("{id:int}/status")]
    [Authorize(Roles = "Administrador")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AlternarStatus(int id)
    {
        var sucesso = await _unidadeService.AlternarStatusAsync(id);
        if (!sucesso)
        {
            return NotFound(new { mensagem = $"Unidade franqueada com ID {id} não encontrada." });
        }
        return Ok(new { mensagem = "Status da unidade alterado com sucesso." });
    }
}
