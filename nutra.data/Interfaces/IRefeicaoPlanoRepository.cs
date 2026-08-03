using Nutra.Models.RegraNutricional;

namespace Nutra.Interfaces;

public interface IRefeicaoPlanoRepository : IBaseRepository<RefeicaoPlano>
{
    /// <summary>Refeição com plano, itens e substituições — o necessário para excluir em cascata.</summary>
    Task<RefeicaoPlano?> ObterComItensESubstituicoesAsync(int refeicaoId, int perfilNutricionalId);

    Task<RefeicaoPlano?> ObterComItensAsync(int refeicaoId, int perfilNutricionalId);

    Task<RefeicaoPlano?> ObterComItensPorIdAsync(int refeicaoId);
}
