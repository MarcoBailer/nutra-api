using Nutra.Models.Usuario;

namespace Nutra.Interfaces;

/// <summary>
/// Projeção local do usuário autenticado no IDP.
/// <para>
/// Não herda <see cref="IBaseRepository{T}"/> de propósito: os quatro métodos
/// abaixo são tudo que se faz com essa entidade, e a herança obrigaria todo
/// dublê de teste a implementar membros que ninguém chama.
/// </para>
/// <para>
/// Exceção deliberada à regra "só a UnitOfWork confirma":
/// <see cref="CreateAsync"/> e <see cref="UpdateAsync"/> gravam por conta
/// própria porque são chamados no evento de validação do JWT, fora de qualquer
/// serviço e de qualquer transação.
/// </para>
/// </summary>
public interface IApplicationUserRepository
{
    Task<ApplicationUser?> FindByIdAsync(string userId);
    Task<ApplicationUser?> FindByEmailAsync(string email);
    Task<ApplicationUser> CreateAsync(ApplicationUser user);

    /// <summary><c>false</c> quando o registro não existe mais.</summary>
    Task<bool> UpdateAsync(ApplicationUser user);
}
