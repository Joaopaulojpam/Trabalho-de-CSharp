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
public class RoyaltiesController : ControllerBase
{
    private readonly IRoyaltyService _royaltyService;

    public RoyaltiesController(IRoyaltyService royaltyService)
    {
        _royaltyService = royaltyService;
    }

    /// <summary>
    /// Consulta os registros de royalties com filtros por unidade, mês, ano e status.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<RoyaltyResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Listar(
        [FromQuery] int? unidadeId = null,
        [FromQuery] int? mes = null,
        [FromQuery] int? ano = null,
        [FromQuery] StatusRoyalty? status = null)
    {
        var royalties = await _royaltyService.ListarAsync(unidadeId, mes, ano, status);
        return Ok(royalties);
    }

    /// <summary>
    /// Obtém detalhes de um lançamento de royalty por ID.
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(RoyaltyResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterPorId(int id)
    {
        var royalty = await _royaltyService.ObterPorIdAsync(id);
        if (royalty == null)
        {
            return NotFound(new { mensagem = $"Lançamento de royalty com ID {id} não encontrado." });
        }
        return Ok(royalty);
    }

    /// <summary>
    /// Calcula e gera ou recalcula o valor de royalty de uma unidade com base no faturamento do mês e percentual configurado.
    /// </summary>
    [HttpPost("gerar")]
    [Authorize(Roles = "Administrador,Gestor")]
    [ProducesResponseType(typeof(RoyaltyResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GerarOuRecalcular([FromBody] GerarRoyaltyDto request)
    {
        try
        {
            var royalty = await _royaltyService.GerarOuRecalcularAsync(request);
            return Ok(royalty);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    /// <summary>
    /// Registra a liquidação/pagamento do royalty pela unidade franqueada à franqueadora.
    /// </summary>
    [HttpPatch("{id:int}/pagar")]
    [Authorize(Roles = "Administrador,Gestor")]
    [ProducesResponseType(typeof(RoyaltyResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RegistrarPagamento(int id, [FromBody] RegistrarPagamentoRoyaltyDto request)
    {
        var royalty = await _royaltyService.RegistrarPagamentoAsync(id, request);
        if (royalty == null)
        {
            return NotFound(new { mensagem = $"Lançamento de royalty com ID {id} não encontrado." });
        }
        return Ok(royalty);
    }

    /// <summary>
    /// Retorna um resumo consolidado de apuração de royalties da rede para um mês e ano específicos.
    /// </summary>
    [HttpGet("resumo")]
    [ProducesResponseType(typeof(ResumoRoyaltiesDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> ObterResumo([FromQuery] int mes, [FromQuery] int ano)
    {
        var resumo = await _royaltyService.ObterResumoAsync(mes, ano);
        return Ok(resumo);
    }
}
