using Nutra.Models.RegraNutricional;

namespace Nutra.Interfaces;

public interface IRegistroAlimentarRepository : IBaseRepository<RegistroAlimentar>
{
    Task<RegistroAlimentar?> ObterPorIdEUsuarioAsync(long registroId, string userId);

    /// <summary>Intervalo semiaberto [<paramref name="inicio"/>, <paramref name="fim"/>), ordenado por consumo.</summary>
    Task<IEnumerable<RegistroAlimentar>> ListarPorUsuarioEPeriodoAsync(
        string userId, DateTime inicio, DateTime fim);

    /// <summary>Igual ao anterior, com o item de plano vinculado — o diário mostra o nome dele.</summary>
    Task<IEnumerable<RegistroAlimentar>> ListarComItemPlanoPorUsuarioEPeriodoAsync(
        string userId, DateTime inicio, DateTime fim);
}
