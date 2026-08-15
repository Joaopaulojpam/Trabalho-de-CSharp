using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Franquias.Api.Configurations;
using Franquias.Api.Data;
using Franquias.Api.DTOs;
using Franquias.Api.Models;
using Franquias.Api.Models.Enums;
using Franquias.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Franquias.Api.Services.Implementations;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly JwtSettings _jwtSettings;

    public AuthService(AppDbContext context, IOptions<JwtSettings> jwtSettings)
    {
        _context = context;
        _jwtSettings = jwtSettings.Value;
    }

    public async Task<LoginResponseDto?> AutenticarAsync(LoginRequestDto request)
    {
        var usuario = await _context.Usuarios
            .Include(u => u.UnidadeFranqueada)
            .FirstOrDefaultAsync(u => u.Email.ToLower() == request.Email.ToLower());

        if (usuario == null || !usuario.Ativo)
        {
            return null;
        }

        bool senhaValida = BCrypt.Net.BCrypt.Verify(request.Senha, usuario.SenhaHash);
        if (!senhaValida)
        {
            return null;
        }

        var expiracao = DateTime.UtcNow.AddHours(_jwtSettings.ExpirationInHours);
        var token = GerarTokenJwt(usuario);

        return new LoginResponseDto(
            token,
            usuario.Id,
            usuario.Nome,
            usuario.Email,
            usuario.Perfil.ToString(),
            usuario.UnidadeFranqueadaId,
            usuario.UnidadeFranqueada?.Nome,
            expiracao
        );
    }

    public async Task<UsuarioResponseDto> RegistrarUsuarioAsync(CriarUsuarioDto request)
    {
        // Regra de Negócio: Não permitir e-mail duplicado
        bool emailExiste = await _context.Usuarios.AnyAsync(u => u.Email.ToLower() == request.Email.ToLower());
        if (emailExiste)
        {
            throw new InvalidOperationException($"Já existe um usuário cadastrado com o e-mail '{request.Email}'.");
        }

        if (request.UnidadeFranqueadaId.HasValue)
        {
            bool unidadeExiste = await _context.UnidadesFranqueadas.AnyAsync(u => u.Id == request.UnidadeFranqueadaId.Value);
            if (!unidadeExiste)
            {
                throw new InvalidOperationException("Unidade franqueada informada não foi encontrada.");
            }
        }

        var usuario = new Usuario
        {
            Nome = request.Nome,
            Email = request.Email,
            SenhaHash = BCrypt.Net.BCrypt.HashPassword(request.Senha),
            Perfil = request.Perfil,
            UnidadeFranqueadaId = request.UnidadeFranqueadaId,
            Ativo = true,
            DataCadastro = DateTime.UtcNow
        };

        await _context.Usuarios.AddAsync(usuario);
        await _context.SaveChangesAsync();

        var unidade = usuario.UnidadeFranqueadaId.HasValue
            ? await _context.UnidadesFranqueadas.FindAsync(usuario.UnidadeFranqueadaId.Value)
            : null;

        return new UsuarioResponseDto(
            usuario.Id,
            usuario.Nome,
            usuario.Email,
            usuario.Perfil.ToString(),
            usuario.Ativo,
            usuario.DataCadastro,
            usuario.UnidadeFranqueadaId,
            unidade?.Nome
        );
    }

    public string GerarTokenJwt(Usuario usuario)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_jwtSettings.Secret);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new(ClaimTypes.Name, usuario.Nome),
            new(ClaimTypes.Email, usuario.Email),
            new(ClaimTypes.Role, usuario.Perfil.ToString())
        };

        if (usuario.UnidadeFranqueadaId.HasValue)
        {
            claims.Add(new Claim("UnidadeId", usuario.UnidadeFranqueadaId.Value.ToString()));
        }

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(_jwtSettings.ExpirationInHours),
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public async Task<List<UsuarioResponseDto>> ListarUsuariosAsync()
    {
        return await _context.Usuarios
            .Include(u => u.UnidadeFranqueada)
            .OrderBy(u => u.Nome)
            .Select(u => new UsuarioResponseDto(
                u.Id,
                u.Nome,
                u.Email,
                u.Perfil.ToString(),
                u.Ativo,
                u.DataCadastro,
                u.UnidadeFranqueadaId,
                u.UnidadeFranqueada != null ? u.UnidadeFranqueada.Nome : null
            ))
            .ToListAsync();
    }

    public async Task<UsuarioResponseDto?> ObterUsuarioPorIdAsync(int id)
    {
        var u = await _context.Usuarios
            .Include(u => u.UnidadeFranqueada)
            .FirstOrDefaultAsync(u => u.Id == id);

        if (u == null) return null;

        return new UsuarioResponseDto(
            u.Id,
            u.Nome,
            u.Email,
            u.Perfil.ToString(),
            u.Ativo,
            u.DataCadastro,
            u.UnidadeFranqueadaId,
            u.UnidadeFranqueada?.Nome
        );
    }

    public async Task<UsuarioResponseDto?> AtualizarUsuarioAsync(int id, AtualizarUsuarioDto request)
    {
        var usuario = await _context.Usuarios
            .Include(u => u.UnidadeFranqueada)
            .FirstOrDefaultAsync(u => u.Id == id);

        if (usuario == null) return null;

        bool emailExisteOutro = await _context.Usuarios
            .AnyAsync(u => u.Email.ToLower() == request.Email.ToLower() && u.Id != id);

        if (emailExisteOutro)
        {
            throw new InvalidOperationException($"O e-mail '{request.Email}' já está em uso por outro usuário.");
        }

        usuario.Nome = request.Nome;
        usuario.Email = request.Email;
        usuario.Perfil = request.Perfil;
        usuario.UnidadeFranqueadaId = request.UnidadeFranqueadaId;
        usuario.Ativo = request.Ativo;

        if (!string.IsNullOrWhiteSpace(request.NovaSenha))
        {
            usuario.SenhaHash = BCrypt.Net.BCrypt.HashPassword(request.NovaSenha);
        }

        await _context.SaveChangesAsync();

        return new UsuarioResponseDto(
            usuario.Id,
            usuario.Nome,
            usuario.Email,
            usuario.Perfil.ToString(),
            usuario.Ativo,
            usuario.DataCadastro,
            usuario.UnidadeFranqueadaId,
            usuario.UnidadeFranqueada?.Nome
        );
    }

    public async Task<bool> AlternarStatusUsuarioAsync(int id)
    {
        var usuario = await _context.Usuarios.FindAsync(id);
        if (usuario == null) return false;

        usuario.Ativo = !usuario.Ativo;
        await _context.SaveChangesAsync();
        return true;
    }
}
