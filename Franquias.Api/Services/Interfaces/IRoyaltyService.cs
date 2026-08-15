using Franquias.Api.DTOs;
using Franquias.Api.Models.Enums;

namespace Franquias.Api.Services.Interfaces;

public interface IRoyaltyService
{
    Task<List<RoyaltyResponseDto>> ListarAsync(int? unidadeId = null, int? mes = null, int? ano = null, StatusRoyalty? status = null);
    Task<RoyaltyResponseDto?> ObterPorIdAsync(int id);
    Task<RoyaltyResponseDto> GerarOuRecalcularAsync(GerarRoyaltyDto request);
    Task<RoyaltyResponseDto?> RegistrarPagamentoAsync(int id, RegistrarPagamentoRoyaltyDto request);
    Task<ResumoRoyaltiesDto> ObterResumoAsync(int mes, int ano);
}
