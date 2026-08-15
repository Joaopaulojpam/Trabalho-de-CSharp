using System.Security.Claims;
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
public class ChamadosController : ControllerBase
{
    private readonly IChamadoService _chamadoService;

    public ChamadosController(IChamadoService chamadoService)
    {
        _chamadoService = chamadoService;
    }

    /// <summary>
    /// Lista chamados de suporte com filtros opcionais por unidade, status ou prioridade.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<ChamadoResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Listar(
        [FromQuery] int? unidadeId = null,
        [FromQuery] StatusChamado? status = null,
        [FromQuery] PrioridadeChamado? prioridade = null)
    {
        var chamados = await _chamadoService.ListarAsync(unidadeId, status, prioridade);
        return Ok(chamados);
    }

    /// <summary>
    /// Obtém detalhes de um chamado de suporte específico por ID.
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ChamadoResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterPorId(int id)
    {
        var chamado = await _chamadoService.ObterPorIdAsync(id);
        if (chamado == null)
        {
            return NotFound(new { mensagem = $"Chamado com ID {id} não encontrado." });
        }
        return Ok(chamado);
    }

    /// <summary>
    /// Abre um novo chamado ou solicitação entre a unidade franqueada e a franqueadora.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Administrador,Gestor,Operador")]
    [ProducesResponseType(typeof(ChamadoResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Abrir([FromBody] CriarChamadoDto request)
    {
        try
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int? userId = int.TryParse(userIdStr, out int id) ? id : null;

            var chamado = await _chamadoService.AbrirChamadoAsync(request, userId);
            return CreatedAtAction(nameof(ObterPorId), new { id = chamado.Id }, chamado);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    /// <summary>
    /// Atualiza o status de um chamado (ex: EmAtendimento, Concluido, Cancelado) e registra a resposta/solução técnica.
    /// </summary>
    [HttpPatch("{id:int}/status")]
    [Authorize(Roles = "Administrador,Gestor")]
    [ProducesResponseType(typeof(ChamadoResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AtualizarStatus(int id, [FromBody] AtualizarStatusChamadoDto request)
    {
        var chamado = await _chamadoService.AtualizarStatusAsync(id, request);
        if (chamado == null)
        {
            return NotFound(new { mensagem = $"Chamado com ID {id} não encontrado." });
        }
        return Ok(chamado);
    }

    /// <summary>
    /// Retorna a contagem de chamados agrupada por status.
    /// </summary>
    [HttpGet("contagem-status")]
    [ProducesResponseType(typeof(Dictionary<string, int>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ContagemPorStatus()
    {
        var contagem = await _chamadoService.ContarChamadosPorStatusAsync();
        return Ok(contagem);
    }
}
