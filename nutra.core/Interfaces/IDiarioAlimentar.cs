using Nutra.Enum;
using Nutra.Models;
using Nutra.Models.Dtos;

namespace Nutra.Interfaces;

public interface IDiarioAlimentar
{
    // --- Registro de consumo ---
    Task<RetornoPadrao<RegistroConsumoResultadoDto>> RegistrarConsumoAsync(string userId, RegistroConsumoDto dto);
    Task<RetornoPadrao<List<RegistroConsumoResultadoDto>>> RegistrarConsumoLoteAsync(string userId, RegistroConsumoLoteDto dto);
    Task<RetornoPadrao> ExcluirRegistroAsync(string userId, long registroId);

    // --- Fotos de refeição ---
    Task<RetornoPadrao<FotoRefeicaoResultadoDto>> AdicionarFotoRefeicaoAsync(string userId, FotoRefeicaoDto dto);
    Task<RetornoPadrao> RemoverFotoRefeicaoAsync(string userId, int fotoId);
    Task<RetornoPadrao<List<FotoRefeicaoResultadoDto>>> ListarFotosDoDiaAsync(string userId, DateTime data);

    // --- Consulta diária (planejado vs consumido) ---
    Task<RetornoPadrao<DiarioDiaDto>> ObterDiarioDoDiaAsync(string userId, DateTime? data = null);
    Task<RetornoPadrao<List<DiarioDiaDto>>> ObterDiarioPorPeriodoAsync(string userId, DateTime dataInicio, DateTime dataFim);

    // --- Relatório de aderência ---
    Task<RetornoPadrao<RelatorioAdesaoDto>> GerarRelatorioAdesaoAsync(string userId, DateTime dataInicio, DateTime dataFim);
    Task<RetornoPadrao<RelatorioAdesaoDto>> GerarRelatorioAdesaoPacienteAsync(string profissionalUserId, string pacienteUserId, DateTime dataInicio, DateTime dataFim);
}
