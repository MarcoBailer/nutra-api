using Nutra.Models.RegraNutricional;

namespace Nutra.Interfaces;

public interface IFotoProgressoRepository : IBaseRepository<FotoProgresso>
{
    /// <summary>Garante que a foto pertence a uma avaliação do perfil informado.</summary>
    Task<FotoProgresso?> ObterPorIdEPerfilAsync(int fotoId, int perfilNutricionalId);
}
