using Nutra.Models.RegraNutricional;

namespace Nutra.Interfaces;

public interface IItemRefeicaoRepository : IBaseRepository<ItemRefeicao>
{
    /// <summary>Item com refeição, plano e substituições, validando a posse pelo perfil.</summary>
    Task<ItemRefeicao?> ObterComRefeicaoEPlanoAsync(int itemId, int perfilNutricionalId);
}
