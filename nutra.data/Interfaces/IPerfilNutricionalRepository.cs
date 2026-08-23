using Nutra.Models.Usuario;

namespace Nutra.Interfaces;

public interface IPerfilNutricionalRepository : IBaseRepository<PerfilNutricional>
{
    Task<PerfilNutricional?> ObterPorUsuarioIdAsync(string userId);

    /// <summary>
    /// Perfil com restrições, equipamentos, histórico clínico e preferências.
    /// Uma consulta só serve leitura e atualização: a diferença entre elas era
    /// uma coleção, não valia um segundo método para manter em sincronia.
    /// </summary>
    Task<PerfilNutricional?> ObterComColecoesAsync(string userId);

    Task<bool> ExistePorEmailAsync(string email);
}
