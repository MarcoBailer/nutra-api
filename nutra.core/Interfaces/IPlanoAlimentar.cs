using Nutra.Models;
using Nutra.Models.Dtos;

namespace Nutra.Interfaces;

public interface IPlanoAlimentar
{
    // --- CRUD Plano ---
    Task<RetornoPadrao<PlanoAlimentarResultadoDto>> CriarPlanoAsync(string userId, CriarPlanoAlimentarDto dto);
    Task<RetornoPadrao<PlanoAlimentarResultadoDto>> CriarPlanoPorProfissionalAsync(string profissionalUserId, CriarPlanoProfissionalDto dto);
    Task<RetornoPadrao<PlanoAlimentarResultadoDto>> ObterPlanoAsync(string userId, int planoId);
    Task<RetornoPadrao<PlanoAlimentarResultadoDto>> ObterPlanoAtivoAsync(string userId);
    Task<RetornoPadrao<List<PlanoAlimentarResumoDto>>> ListarPlanosAsync(string userId);
    Task<RetornoPadrao<PlanoAlimentarResultadoDto>> AtualizarPlanoAsync(string userId, int planoId, AtualizarPlanoAlimentarDto dto);
    Task<RetornoPadrao> ExcluirPlanoAsync(string userId, int planoId);
    Task<RetornoPadrao> AtivarPlanoAsync(string userId, int planoId);

    // --- Refeições ---
    Task<RetornoPadrao<PlanoAlimentarResultadoDto>> AdicionarRefeicaoAsync(string userId, int planoId, AdicionarRefeicaoDto dto);
    Task<RetornoPadrao> RemoverRefeicaoAsync(string userId, int refeicaoId);

    // --- Itens ---
    Task<RetornoPadrao<PlanoAlimentarResultadoDto>> AdicionarItemAsync(string userId, int refeicaoId, AdicionarItemDto dto);
    Task<RetornoPadrao> RemoverItemAsync(string userId, int itemId);

    // --- Substituições ---
    Task<RetornoPadrao> AdicionarSubstituicaoAsync(string userId, int itemId, AdicionarSubstituicaoDto dto);
    Task<RetornoPadrao> RemoverSubstituicaoAsync(string userId, int substituicaoId);

}
