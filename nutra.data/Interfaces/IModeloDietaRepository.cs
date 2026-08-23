using Nutra.Models.RegraNutricional;

namespace Nutra.Interfaces;

public interface IModeloDietaRepository : IBaseRepository<ModeloDieta>
{
    /// <summary>
    /// Modelos ativos visíveis ao profissional: públicos ou de autoria dele.
    /// <paramref name="profissionalUserId"/> nulo devolve todos os ativos.
    /// </summary>
    Task<IEnumerable<ModeloDieta>> ListarDisponiveisAsync(string? profissionalUserId);

    /// <summary>Modelo ativo com refeições, itens e autor.</summary>
    Task<ModeloDieta?> ObterCompletoAtivoAsync(int modeloId);

    Task<ModeloDieta?> ObterPorIdEProfissionalAsync(int modeloId, string profissionalUserId);
}
