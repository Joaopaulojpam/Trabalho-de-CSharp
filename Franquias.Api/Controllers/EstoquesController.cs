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
public class EstoquesController : ControllerBase
{
    private readonly IEstoqueService _estoqueService;

    public EstoquesController(IEstoqueService estoqueService)
    {
        _estoqueService = estoqueService;
    }

    /// <summary>
    /// Consulta o saldo de estoque da unidade, com opção de listar apenas itens abaixo do estoque mínimo.
    /// </summary>
    /// <param name="unidadeId">ID da Unidade Franqueada</param>
    /// <param name="apenasCriticos">Se true, retorna apenas itens cujo saldo está no limite ou abaixo do estoque mínimo</param>
    [HttpGet("unidade/{unidadeId:int}")]
    [ProducesResponseType(typeof(List<EstoqueResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ConsultarEstoque(int unidadeId, [FromQuery] bool apenasCriticos = false)
    {
        var estoques = await _estoqueService.ConsultarEstoquePorUnidadeAsync(unidadeId, apenasCriticos);
        return Ok(estoques);
    }

    /// <summary>
    /// Registra movimentação de estoque (Entrada, Saída, Ajuste) garantindo a regra de não permitir saldo negativo.
    /// </summary>
    [HttpPost("movimentar")]
    [Authorize(Roles = "Administrador,Gestor,Operador")]
    [ProducesResponseType(typeof(EstoqueResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Movimentar([FromBody] MovimentarEstoqueDto request)
    {
        try
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int? userId = int.TryParse(userIdStr, out int id) ? id : null;

            var resultado = await _estoqueService.MovimentarEstoqueAsync(request, userId);
            return Ok(resultado);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    /// <summary>
    /// Altera a quantidade mínima de segurança de um produto em uma unidade franqueada.
    /// </summary>
    [HttpPut("unidade/{unidadeId:int}/produto/{produtoId:int}/minimo")]
    [Authorize(Roles = "Administrador,Gestor")]
    [ProducesResponseType(typeof(EstoqueResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DefinirEstoqueMinimo(int unidadeId, int produtoId, [FromBody] DefinirEstoqueMinimoDto request)
    {
        var estoque = await _estoqueService.AtualizarEstoqueMinimoAsync(unidadeId, produtoId, request.QuantidadeMinima);
        if (estoque == null)
        {
            return NotFound(new { mensagem = "Registro de estoque não encontrado para esta unidade e produto." });
        }
        return Ok(estoque);
    }

    /// <summary>
    /// Consulta o histórico completo de auditoria das movimentações de estoque.
    /// </summary>
    [HttpGet("movimentacoes")]
    [ProducesResponseType(typeof(List<MovimentacaoResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> HistoricoMovimentacoes([FromQuery] int? unidadeId = null, [FromQuery] int? produtoId = null)
    {
        var historico = await _estoqueService.ListarHistoricoMovimentacoesAsync(unidadeId, produtoId);
        return Ok(historico);
    }
}
