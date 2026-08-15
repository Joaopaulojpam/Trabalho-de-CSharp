using System.Security.Claims;
using Franquias.Api.DTOs;
using Franquias.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Franquias.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
public class VendasController : ControllerBase
{
    private readonly IVendaService _vendaService;

    public VendasController(IVendaService vendaService)
    {
        _vendaService = vendaService;
    }

    /// <summary>
    /// Consulta vendas realizadas com filtros opcionais por unidade e intervalo de datas.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<VendaResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Listar(
        [FromQuery] int? unidadeId = null,
        [FromQuery] DateTime? dataInicio = null,
        [FromQuery] DateTime? dataFim = null)
    {
        var vendas = await _vendaService.ListarVendasAsync(unidadeId, dataInicio, dataFim);
        return Ok(vendas);
    }

    /// <summary>
    /// Obtém os detalhes completos de uma venda por ID, incluindo todos os itens e subtotais.
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(VendaResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterPorId(int id)
    {
        var venda = await _vendaService.ObterVendaPorIdAsync(id);
        if (venda == null)
        {
            return NotFound(new { mensagem = $"Venda com ID {id} não encontrada." });
        }
        return Ok(venda);
    }

    /// <summary>
    /// Registra uma nova venda, calculando automaticamente totais e subtotais e deduzindo o estoque da unidade.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Administrador,Gestor,Operador")]
    [ProducesResponseType(typeof(VendaResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RegistrarVenda([FromBody] CriarVendaDto request)
    {
        try
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int? userId = int.TryParse(userIdStr, out int id) ? id : null;

            var venda = await _vendaService.RegistrarVendaAsync(request, userId);
            return CreatedAtAction(nameof(ObterPorId), new { id = venda.Id }, venda);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    /// <summary>
    /// Cancela uma venda realizada e estorna as quantidades dos itens de volta para o estoque.
    /// </summary>
    [HttpPost("{id:int}/cancelar")]
    [Authorize(Roles = "Administrador,Gestor")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CancelarVenda(int id)
    {
        var sucesso = await _vendaService.CancelarVendaAsync(id);
        if (!sucesso)
        {
            return BadRequest(new { mensagem = $"Venda com ID {id} não foi encontrada ou já se encontra cancelada." });
        }
        return Ok(new { mensagem = "Venda cancelada com sucesso e estoque estornado." });
    }
}
