using Franquias.Api.DTOs;

namespace Franquias.Api.Services.Interfaces;

public interface IUnidadeService
{
    Task<List<UnidadeResponseDto>> ListarAsync(bool? ativo = null, string? termoBusca = null, string? cidade = null, string? uf = null);
    Task<UnidadeResponseDto?> ObterPorIdAsync(int id);
    Task<UnidadeResponseDto> CriarAsync(CriarUnidadeDto request);
    Task<UnidadeResponseDto?> AtualizarAsync(int id, AtualizarUnidadeDto request);
    Task<bool> AlternarStatusAsync(int id);
}
