using Nutra.Models;
using Nutra.Models.Dtos;

namespace Nutra.Interfaces;

public interface IPlanoAlimentar
{
    // --- CRUD Plano ---
    Task<PlanoAlimentarResultadoDto> CriarPlanoAsync(string userId, CriarPlanoAlimentarDto dto);
    Task<PlanoAlimentarResultadoDto> CriarPlanoPorProfissionalAsync(string profissionalUserId, CriarPlanoProfissionalDto dto);
    Task<PlanoAlimentarResultadoDto> ObterPlanoAsync(string userId, int planoId);
    Task<PlanoAlimentarResultadoDto?> ObterPlanoAtivoAsync(string userId);
    Task<List<PlanoAlimentarResumoDto>> ListarPlanosAsync(string userId);
    Task<PlanoAlimentarResultadoDto> AtualizarPlanoAsync(string userId, int planoId, AtualizarPlanoAlimentarDto dto);
    Task<RetornoPadrao> ExcluirPlanoAsync(string userId, int planoId);
    Task<RetornoPadrao> AtivarPlanoAsync(string userId, int planoId);

    // --- Refeições ---
    Task<PlanoAlimentarResultadoDto> AdicionarRefeicaoAsync(string userId, int planoId, AdicionarRefeicaoDto dto);
    Task<RetornoPadrao> RemoverRefeicaoAsync(string userId, int refeicaoId);

    // --- Itens ---
    Task<PlanoAlimentarResultadoDto> AdicionarItemAsync(string userId, int refeicaoId, AdicionarItemDto dto);
    Task<RetornoPadrao> RemoverItemAsync(string userId, int itemId);

    // --- Substituições ---
    Task<RetornoPadrao> AdicionarSubstituicaoAsync(string userId, int itemId, AdicionarSubstituicaoDto dto);
    Task<RetornoPadrao> RemoverSubstituicaoAsync(string userId, int substituicaoId);

    // --- Modelos de Dieta ---
    Task<ModeloDietaResultadoDto> CriarModeloDietaAsync(string profissionalUserId, CriarModeloDietaDto dto);
    Task<List<ModeloDietaResumoDto>> ListarModelosDietaAsync(string? profissionalUserId);
    Task<ModeloDietaResultadoDto> ObterModeloDietaAsync(int modeloId);
    Task<RetornoPadrao> ExcluirModeloDietaAsync(string profissionalUserId, int modeloId);

    // --- Duplicar plano a partir de template ---
    Task<PlanoAlimentarResultadoDto> CriarPlanoAPartirDeModeloAsync(string userId, int modeloId, DateTime dataInicio, DateTime? dataFim);
}
