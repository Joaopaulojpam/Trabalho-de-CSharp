using System.Security.Claims;
using Franquias.Api.DTOs;
using Franquias.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Franquias.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Realiza a autenticação do usuário e retorna o token JWT.
    /// </summary>
    /// <param name="request">Credenciais de e-mail e senha</param>
    /// <returns>Token JWT e dados do perfil</returns>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        var response = await _authService.AutenticarAsync(request);
        if (response == null)
        {
            return Unauthorized(new { mensagem = "E-mail ou senha inválidos, ou usuário inativo." });
        }

        return Ok(response);
    }

    /// <summary>
    /// Retorna as informações do usuário autenticado no momento através do token JWT.
    /// </summary>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(UsuarioLogadoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult Me()
    {
        var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var nomeClaim = User.FindFirst(ClaimTypes.Name)?.Value;
        var emailClaim = User.FindFirst(ClaimTypes.Email)?.Value;
        var perfilClaim = User.FindFirst(ClaimTypes.Role)?.Value;
        var unidadeClaim = User.FindFirst("UnidadeId")?.Value;

        if (string.IsNullOrEmpty(idClaim))
        {
            return Unauthorized();
        }

        int? unidadeId = int.TryParse(unidadeClaim, out int parsedId) ? parsedId : null;

        var usuario = new UsuarioLogadoDto(
            int.Parse(idClaim),
            nomeClaim ?? string.Empty,
            emailClaim ?? string.Empty,
            perfilClaim ?? string.Empty,
            unidadeId
        );

        return Ok(usuario);
    }
}
