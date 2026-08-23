using Nutra.Interfaces;
using Nutra.Models.Usuario;

namespace Nutra.Test.Fakes;

/// <summary>
/// Dublê em memória de <see cref="IApplicationUserService"/>.
/// <para>
/// Serve para testar quem depende dessa interface sem subir banco nenhum.
/// Escrito à mão de propósito: a interface tem 4 métodos, não vale adicionar
/// uma biblioteca de mock ao projeto por causa disso.
/// </para>
/// </summary>
public sealed class FakeApplicationUserService : IApplicationUserRepository
{
    private readonly Dictionary<string, ApplicationUser> _usuarios = [];

    /// <summary>
    /// Quando <c>true</c>, <see cref="UpdateAsync"/> devolve <c>false</c>.
    /// Simula o caso em que o registro sumiu entre a leitura e a gravação.
    /// </summary>
    public bool UpdateDeveFalhar { get; set; }

    /// <summary>Quantas vezes <see cref="UpdateAsync"/> foi chamado.</summary>
    public int ChamadasUpdate { get; private set; }

    /// <summary>Coloca um usuário no estado inicial do dublê.</summary>
    public void Semear(ApplicationUser usuario) => _usuarios[usuario.Id] = usuario;

    public Task<ApplicationUser?> FindByIdAsync(string userId) =>
        Task.FromResult(_usuarios.GetValueOrDefault(userId));

    public Task<ApplicationUser?> FindByEmailAsync(string email) =>
        Task.FromResult(_usuarios.Values.FirstOrDefault(usuario => usuario.Email == email));

    public Task<ApplicationUser> CreateAsync(ApplicationUser user)
    {
        _usuarios[user.Id] = user;
        return Task.FromResult(user);
    }

    public Task<bool> UpdateAsync(ApplicationUser user)
    {
        ChamadasUpdate++;

        if (UpdateDeveFalhar)
            return Task.FromResult(false);

        _usuarios[user.Id] = user;
        return Task.FromResult(true);
    }
}
