using Nutra.Models.RegraNutricional;

namespace Nutra.Interfaces;

public interface IPlanoAlimentarRepository : IBaseRepository<PlanoAlimentar>
{
    /// <summary>
    /// Plano com perfil, profissional, modelo de origem e a árvore
    /// refeições → itens → substituições, tudo ordenado.
    /// </summary>
    Task<PlanoAlimentar?> ObterCompletoPorIdAsync(int planoId);

    Task<PlanoAlimentar?> ObterCompletoPorIdEUsuarioAsync(int planoId, string userId);

    Task<PlanoAlimentar?> ObterCompletoPorIdEPerfilAsync(int planoId, int perfilNutricionalId);

    Task<PlanoAlimentar?> ObterCompletoAtivoPorUsuarioAsync(string userId);

    Task<PlanoAlimentar?> ObterPorIdEPerfilAsync(int planoId, int perfilNutricionalId);

    Task<PlanoAlimentar?> ObterAtivoPorPerfilAsync(int perfilNutricionalId);

    Task<PlanoAlimentar?> ObterAtivoComRefeicoesEItensAsync(int perfilNutricionalId);

    /// <summary>Mais recentes primeiro, com refeições/itens e profissional (o resumo conta e nomeia).</summary>
    Task<IEnumerable<PlanoAlimentar>> ListarPorPerfilAsync(int perfilNutricionalId);

    Task<IEnumerable<PlanoAlimentar>> ListarAtivosPorPerfilAsync(int perfilNutricionalId);
}
