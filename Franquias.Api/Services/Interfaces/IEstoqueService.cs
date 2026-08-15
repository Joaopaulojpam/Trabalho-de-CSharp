using Franquias.Api.DTOs;

namespace Franquias.Api.Services.Interfaces;

public interface IEstoqueService
{
    Task<List<EstoqueResponseDto>> ConsultarEstoquePorUnidadeAsync(int unidadeId, bool apenasAbaixoDoMinimo = false);
    Task<EstoqueResponseDto?> ObterEstoqueItemAsync(int unidadeId, int produtoId);
    Task<EstoqueResponseDto> MovimentarEstoqueAsync(MovimentarEstoqueDto request, int? usuarioId = null);
    Task<EstoqueResponseDto?> AtualizarEstoqueMinimoAsync(int unidadeId, int produtoId, int quantidadeMinima);
    Task<List<MovimentacaoResponseDto>> ListarHistoricoMovimentacoesAsync(int? unidadeId = null, int? produtoId = null);
}
