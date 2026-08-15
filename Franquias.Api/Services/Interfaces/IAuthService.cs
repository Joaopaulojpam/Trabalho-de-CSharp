using Franquias.Api.DTOs;
using Franquias.Api.Models;

namespace Franquias.Api.Services.Interfaces;

public interface IAuthService
{
    Task<LoginResponseDto?> AutenticarAsync(LoginRequestDto request);
    Task<UsuarioResponseDto> RegistrarUsuarioAsync(CriarUsuarioDto request);
    string GerarTokenJwt(Usuario usuario);
    Task<List<UsuarioResponseDto>> ListarUsuariosAsync();
    Task<UsuarioResponseDto?> ObterUsuarioPorIdAsync(int id);
    Task<UsuarioResponseDto?> AtualizarUsuarioAsync(int id, AtualizarUsuarioDto request);
    Task<bool> AlternarStatusUsuarioAsync(int id);
}
