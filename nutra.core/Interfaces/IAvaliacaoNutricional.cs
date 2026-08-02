using Nutra.Models;
using Nutra.Models.Dtos;

namespace Nutra.Interfaces;

/// <summary>
/// Interface para o serviço de Avaliação Nutricional (Antropometria + Cálculos).
/// </summary>
public interface IAvaliacaoNutricional
{
    // ===================== AVALIAÇÕES ANTROPOMÉTRICAS =====================

    /// <summary>
    /// Registra uma nova avaliação antropométrica completa para o paciente.
    /// Calcula automaticamente: IMC, RCQ, %gordura (dobras/bio), TMB (3 fórmulas), GET, peso ideal, macros.
    /// </summary>
    Task<AvaliacaoAntropometricaResultadoDto> RegistrarAvaliacaoAsync(string userId, AvaliacaoAntropometricaDto dto);

    /// <summary>
    /// Registra avaliação em nome de um paciente (nutricionista logado).
    /// </summary>
    Task<AvaliacaoAntropometricaResultadoDto> RegistrarAvaliacaoPorProfissionalAsync(
        string profissionalUserId, string pacienteUserId, AvaliacaoAntropometricaDto dto);

    /// <summary>
    /// Obtém avaliação completa por Id.
    /// </summary>
    Task<AvaliacaoAntropometricaResultadoDto> ObterAvaliacaoPorIdAsync(string userId, int avaliacaoId);

    /// <summary>
    /// Lista todas as avaliações do paciente (resumidas, ordenadas por data desc).
    /// </summary>
    Task<List<AvaliacaoResumoDto>> ListarAvaliacoesAsync(string userId);

    /// <summary>
    /// Lista avaliações de um paciente específico (visão do nutricionista).
    /// </summary>
    Task<List<AvaliacaoResumoDto>> ListarAvaliacoesDoPacienteAsync(string profissionalUserId, string pacienteUserId);

    /// <summary>
    /// Compara duas avaliações e calcula a evolução.
    /// </summary>
    Task<ComparacaoAvaliacoesDto> CompararAvaliacoesAsync(string userId, int avaliacaoAnteriorId, int avaliacaoAtualId);

    /// <summary>
    /// Exclui uma avaliação (soft ou hard delete conforme regras).
    /// </summary>
    Task<RetornoPadrao> ExcluirAvaliacaoAsync(string userId, int avaliacaoId);

    // ===================== FOTOS DE PROGRESSO =====================

    /// <summary>
    /// Adiciona fotos de progresso a uma avaliação existente.
    /// </summary>
    Task<RetornoPadrao> AdicionarFotosAsync(string userId, int avaliacaoId, List<FotoProgressoDto> fotos);

    /// <summary>
    /// Remove uma foto de progresso.
    /// </summary>
    Task<RetornoPadrao> RemoverFotoAsync(string userId, int fotoId);
}
