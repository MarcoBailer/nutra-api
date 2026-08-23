using Nutra.Models;

namespace Nutra.Test.Core.Models;

/// <summary>
/// Testes de <see cref="RetornoPadrao"/> e <see cref="RetornoPadrao{T}"/>.
/// <para>
/// São fábricas triviais, mas o <c>StatusCode</c> de cada uma é contrato com o
/// controller (que faz <c>StatusCode(r.StatusCode, r)</c>) e, portanto, com o
/// frontend. Trocar um 404 por 400 sem querer quebra o cliente em silêncio —
/// é exatamente o tipo de erro que estes testes travam.
/// </para>
/// </summary>
public class RetornoPadraoTests
{
    [Fact]
    public void Ok_RetornaSucessoCom200()
    {
        var retorno = RetornoPadrao.Ok("tudo certo");

        Assert.True(retorno.Sucesso);
        Assert.Equal(200, retorno.StatusCode);
        Assert.Equal("tudo certo", retorno.Mensagem);
    }

    [Fact]
    public void Criado_RetornaSucessoCom201()
    {
        var retorno = RetornoPadrao.Criado("criado");

        Assert.True(retorno.Sucesso);
        Assert.Equal(201, retorno.StatusCode);
    }

    /// <summary>
    /// Falha de negócio é retorno, não exceção. Cada fábrica de falha tem um
    /// status HTTP fixo e <c>Sucesso == false</c>.
    /// </summary>
    [Theory]
    [InlineData(400)]
    [InlineData(403)]
    [InlineData(404)]
    [InlineData(409)]
    public void FabricasDeFalha_MarcamSucessoFalsoEPreservamAMensagem(int statusEsperado)
    {
        RetornoPadrao retorno = statusEsperado switch
        {
            400 => RetornoPadrao.Invalido("erro"),
            403 => RetornoPadrao.Proibido("erro"),
            404 => RetornoPadrao.NaoEncontrado("erro"),
            _ => RetornoPadrao.Conflito("erro")
        };

        Assert.False(retorno.Sucesso);
        Assert.Equal(statusEsperado, retorno.StatusCode);
        Assert.Equal("erro", retorno.Mensagem);
    }

    [Fact]
    public void RetornoPadraoGenerico_Ok_CarregaOsDados()
    {
        var retorno = RetornoPadrao<int>.Ok(42, "achou");

        Assert.True(retorno.Sucesso);
        Assert.Equal(200, retorno.StatusCode);
        Assert.Equal(42, retorno.Dados);
        Assert.Equal("achou", retorno.Mensagem);
    }

    [Fact]
    public void RetornoPadraoGenerico_Criado_Retorna201ComDados()
    {
        var retorno = RetornoPadrao<string>.Criado("abc");

        Assert.Equal(201, retorno.StatusCode);
        Assert.Equal("abc", retorno.Dados);
    }

    [Fact]
    public void RetornoPadraoGenerico_Falha_NaoPreencheDados()
    {
        var retorno = RetornoPadrao<string>.NaoEncontrado("sumiu");

        Assert.False(retorno.Sucesso);
        Assert.Null(retorno.Dados);
    }

    /// <summary>
    /// <c>Falha</c> repassa uma falha de uma etapa interna para o retorno público
    /// sem inventar mensagem nem status novos.
    /// </summary>
    [Fact]
    public void RetornoPadraoGenerico_Falha_PropagaMensagemEStatusDaOrigem()
    {
        var origem = RetornoPadrao.Conflito("plano já existe");

        var propagado = RetornoPadrao<string>.Falha(origem);

        Assert.False(propagado.Sucesso);
        Assert.Equal(409, propagado.StatusCode);
        Assert.Equal("plano já existe", propagado.Mensagem);
        Assert.Null(propagado.Dados);
    }
}
