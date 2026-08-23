using Nutra.Models.RegraNutricional;

namespace Nutra.Interfaces;

public interface ISubstituicaoEquivalenteRepository : IBaseRepository<SubstituicaoEquivalente>
{
    /// <summary>Substituição com item → refeição → plano, validando a posse pelo perfil.</summary>
    Task<SubstituicaoEquivalente?> ObterComItemRefeicaoEPlanoAsync(int substituicaoId, int perfilNutricionalId);
}
