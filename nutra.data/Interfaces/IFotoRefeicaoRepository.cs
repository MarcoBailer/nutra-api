using Nutra.Models.RegraNutricional;

namespace Nutra.Interfaces;

public interface IFotoRefeicaoRepository : IBaseRepository<FotoRefeicao>
{
    Task<FotoRefeicao?> ObterPorIdEUsuarioAsync(int fotoId, string userId);

    /// <summary>Intervalo semiaberto [<paramref name="inicio"/>, <paramref name="fim"/>), ordenado por registro.</summary>
    Task<IEnumerable<FotoRefeicao>> ListarPorUsuarioEPeriodoAsync(
        string userId, DateTime inicio, DateTime fim);
}
