using Franquias.Api.DTOs;
using Franquias.Api.Models.Enums;

namespace Franquias.Api.Services.Interfaces;

public interface IChamadoService
{
    Task<List<ChamadoResponseDto>> ListarAsync(int? unidadeId = null, StatusChamado? status = null, PrioridadeChamado? prioridade = null);
    Task<ChamadoResponseDto?> ObterPorIdAsync(int id);
    Task<ChamadoResponseDto> AbrirChamadoAsync(CriarChamadoDto request, int? usuarioId = null);
    Task<ChamadoResponseDto?> AtualizarStatusAsync(int id, AtualizarStatusChamadoDto request);
    Task<Dictionary<string, int>> ContarChamadosPorStatusAsync();
}
