using Nutra.Models.Dtos;
using Nutra.Models.Usuario;
using Nutra.Services;
using Nutra.Test.Fakes;

namespace Nutra.Test.Core.Services;

/// <summary>
/// Testes de <see cref="AccountsService"/>.
/// <para>
/// Este serviço só depende de <c>IApplicationUserService</c> — uma interface —,
/// então dá para testá-lo de verdade em memória, trocando a implementação real
/// por <see cref="FakeApplicationUserService"/>. É esse desacoplamento que torna
/// uma classe testável; os demais serviços que recebem <c>AlimentosContext</c>
/// direto no construtor não têm essa propriedade.
/// </para>
/// </summary>
public class AccountsServiceTests
{
    private const string UserId = "user-1";

    private readonly FakeApplicationUserService _usuarios = new();
    private readonly AccountsService _service;

    public AccountsServiceTests()
    {
        // O construtor da classe de teste roda antes de CADA teste do arquivo.
        // É o "setup" do xUnit — não existe atributo [SetUp] como no NUnit.
        _service = new AccountsService(_usuarios);
    }

    // =====================================================================
    //  ATUALIZAR PERFIL
    // =====================================================================

    [Fact]
    public async Task AtualizarPerfilAsync_UsuarioInexistente_Retorna404SemTentarGravar()
    {
        var retorno = await _service.AtualizarPerfilAsync("nao-existe", new UpdateProfileDto());

        Assert.False(retorno.Sucesso);
        Assert.Equal(404, retorno.StatusCode);
        Assert.Equal(0, _usuarios.ChamadasUpdate);
    }

    [Fact]
    public async Task AtualizarPerfilAsync_CamposPreenchidos_GravaOsNovosValores()
    {
        _usuarios.Semear(CriarUsuario());

        var retorno = await _service.AtualizarPerfilAsync(UserId, new UpdateProfileDto
        {
            NomeCompleto = "Maria Souza",
            Cpf = "111.222.333-44",
            Cidade = "Curitiba"
        });

        var usuario = await _usuarios.FindByIdAsync(UserId);

        Assert.True(retorno.Sucesso);
        Assert.Equal(200, retorno.StatusCode);
        Assert.Equal("Maria Souza", usuario!.NomeCompleto);
        Assert.Equal("111.222.333-44", usuario.CPF);
        Assert.Equal("Curitiba", usuario.Cidade);
    }

    /// <summary>
    /// Regra central do endpoint: o DTO é um PATCH, não um PUT.
    /// Campo nulo significa "não mexe", nunca "apaga".
    /// </summary>
    [Fact]
    public async Task AtualizarPerfilAsync_CamposNulosNoDto_PreservaValoresAtuais()
    {
        _usuarios.Semear(CriarUsuario());

        await _service.AtualizarPerfilAsync(UserId, new UpdateProfileDto { Cidade = "Curitiba" });

        var usuario = await _usuarios.FindByIdAsync(UserId);

        // Único campo enviado no DTO: mudou.
        Assert.Equal("Curitiba", usuario!.Cidade);

        // Todos os demais continuam como estavam.
        Assert.Equal("João Silva", usuario.NomeCompleto);
        Assert.Equal("000.000.000-00", usuario.CPF);
        Assert.Equal("11999999999", usuario.Telefone);
        Assert.Equal("SP", usuario.Estado);
    }

    [Fact]
    public async Task AtualizarPerfilAsync_Sucesso_CarimbaAtualizadoEmEmUtc()
    {
        _usuarios.Semear(CriarUsuario());

        await _service.AtualizarPerfilAsync(UserId, new UpdateProfileDto { NomeCompleto = "Maria" });

        var usuario = await _usuarios.FindByIdAsync(UserId);

        Assert.NotNull(usuario!.AtualizadoEm);
        Assert.Equal(DateTimeKind.Utc, usuario.AtualizadoEm!.Value.Kind);
    }

    [Fact]
    public async Task AtualizarPerfilAsync_GravacaoFalha_Retorna404()
    {
        _usuarios.Semear(CriarUsuario());
        _usuarios.UpdateDeveFalhar = true;

        var retorno = await _service.AtualizarPerfilAsync(UserId, new UpdateProfileDto { NomeCompleto = "Maria" });

        Assert.False(retorno.Sucesso);
        Assert.Equal(404, retorno.StatusCode);
    }

    // =====================================================================
    //  DESATIVAR / REATIVAR CONTA
    // =====================================================================

    [Fact]
    public async Task DesativarContaAsync_UsuarioAtivo_MarcaComoInativoERetorna200()
    {
        _usuarios.Semear(CriarUsuario());

        var retorno = await _service.DesativarContaAsync(UserId);
        var usuario = await _usuarios.FindByIdAsync(UserId);

        Assert.True(retorno.Sucesso);
        Assert.Equal(200, retorno.StatusCode);
        Assert.False(usuario!.Ativo);
    }

    [Fact]
    public async Task DesativarContaAsync_UsuarioInexistente_Retorna404()
    {
        var retorno = await _service.DesativarContaAsync("nao-existe");

        Assert.False(retorno.Sucesso);
        Assert.Equal(404, retorno.StatusCode);
        Assert.Equal(0, _usuarios.ChamadasUpdate);
    }

    [Fact]
    public async Task DesativarContaAsync_GravacaoFalha_Retorna404()
    {
        _usuarios.Semear(CriarUsuario());
        _usuarios.UpdateDeveFalhar = true;

        var retorno = await _service.DesativarContaAsync(UserId);

        Assert.False(retorno.Sucesso);
        Assert.Equal(404, retorno.StatusCode);
    }

    [Fact]
    public async Task ReativarContaAsync_UsuarioInativo_MarcaComoAtivoERetorna200()
    {
        var usuario = CriarUsuario();
        usuario.Ativo = false;
        _usuarios.Semear(usuario);

        var retorno = await _service.ReativarContaAsync(UserId);

        Assert.True(retorno.Sucesso);
        Assert.Equal(200, retorno.StatusCode);
        Assert.True((await _usuarios.FindByIdAsync(UserId))!.Ativo);
    }

    [Fact]
    public async Task ReativarContaAsync_UsuarioInexistente_Retorna404()
    {
        var retorno = await _service.ReativarContaAsync("nao-existe");

        Assert.False(retorno.Sucesso);
        Assert.Equal(404, retorno.StatusCode);
    }

    [Fact]
    public async Task ReativarContaAsync_GravacaoFalha_Retorna404()
    {
        var usuario = CriarUsuario();
        usuario.Ativo = false;
        _usuarios.Semear(usuario);
        _usuarios.UpdateDeveFalhar = true;

        var retorno = await _service.ReativarContaAsync(UserId);

        Assert.False(retorno.Sucesso);
        Assert.Equal(404, retorno.StatusCode);
    }

    private static ApplicationUser CriarUsuario() => new()
    {
        Id = UserId,
        NomeCompleto = "João Silva",
        Email = "joao@exemplo.com",
        CPF = "000.000.000-00",
        Telefone = "11999999999",
        Cidade = "São Paulo",
        Estado = "SP",
        Ativo = true
    };
}
