using Nutra.Models.RegraNutricional;

namespace Nutra.Interfaces;

public interface IAvaliacaoAntropometricaRepository : IBaseRepository<AvaliacaoAntropometrica>
{
    Task<AvaliacaoAntropometrica?> ObterPorIdEPerfilAsync(int id, int perfilNutricionalId);

    /// <summary>Com profissional responsável e fotos — é o que o resultado completo precisa.</summary>
    Task<AvaliacaoAntropometrica?> ObterCompletaPorIdEPerfilAsync(int id, int perfilNutricionalId);

    Task<AvaliacaoAntropometrica?> ObterComFotosPorIdEPerfilAsync(int id, int perfilNutricionalId);

    /// <summary>Mais recentes primeiro, com fotos (o resumo informa a contagem).</summary>
    Task<IEnumerable<AvaliacaoAntropometrica>> ListarPorPerfilAsync(int perfilNutricionalId);
}
