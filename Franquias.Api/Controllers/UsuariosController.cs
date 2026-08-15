using Franquias.Api.DTOs;
using Franquias.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Franquias.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Administrador")]
[Produces("application/json")]
public class UsuariosController : ControllerBase
{
    private readonly IAuthService _authService;

    public UsuariosController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Lista todos os usuários cadastrados no sistema (Acesso restrito: Administrador).
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<UsuarioResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Listar()
    {
        var usuarios = await _authService.ListarUsuariosAsync();
        return Ok(usuarios);
    }

    /// <summary>
    /// Obtém os detalhes de um usuário por ID (Acesso restrito: Administrador).
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(UsuarioResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterPorId(int id)
    {
        var usuario = await _authService.ObterUsuarioPorIdAsync(id);
        if (usuario == null)
        {
            return NotFound(new { mensagem = $"Usuário com ID {id} não encontrado." });
        }
        return Ok(usuario);
    }

    /// <summary>
    /// Cadastra um novo usuário no sistema (Acesso restrito: Administrador).
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(UsuarioResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Criar([FromBody] CriarUsuarioDto request)
    {
        try
        {
            var usuario = await _authService.RegistrarUsuarioAsync(request);
            return CreatedAtAction(nameof(ObterPorId), new { id = usuario.Id }, usuario);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    /// <summary>
    /// Atualiza os dados de um usuário existente (Acesso restrito: Administrador).
    /// </summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(UsuarioResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Atualizar(int id, [FromBody] AtualizarUsuarioDto request)
    {
        try
        {
            var usuario = await _authService.AtualizarUsuarioAsync(id, request);
            if (usuario == null)
            {
                return NotFound(new { mensagem = $"Usuário com ID {id} não encontrado." });
            }
            return Ok(usuario);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    /// <summary>
    /// Ativa ou inativa um usuário (Soft delete / alternância de status).
    /// </summary>
    [HttpPatch("{id:int}/status")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AlternarStatus(int id)
    {
        var sucesso = await _authService.AlternarStatusUsuarioAsync(id);
        if (!sucesso)
        {
            return NotFound(new { mensagem = $"Usuário com ID {id} não encontrado." });
        }
        return Ok(new { mensagem = "Status do usuário alterado com sucesso." });
    }
}
