using Franquias.Api.DTOs;

namespace Franquias.Api.Services.Interfaces;

public interface IVendaService
{
    Task<List<VendaResponseDto>> ListarVendasAsync(int? unidadeId = null, DateTime? dataInicio = null, DateTime? dataFim = null);
    Task<VendaResponseDto?> ObterVendaPorIdAsync(int id);
    Task<VendaResponseDto> RegistrarVendaAsync(CriarVendaDto request, int? usuarioId = null);
    Task<bool> CancelarVendaAsync(int id);
}
