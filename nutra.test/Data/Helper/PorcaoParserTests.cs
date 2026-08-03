using Nutra.Helper;

namespace Nutra.Test.Data.Helper;

/// <summary>
/// Testes de <see cref="PorcaoParser"/>.
/// <para>
/// O parser quebra o texto livre de porção das tabelas de alimentos
/// ("1 fatia (30 g)", "200ml", "1 colher de sopa (15gr)") em dose, unidade e
/// quantidade. É a única entrada não estruturada do domínio — logo, onde mais
/// vale investir em casos de teste.
/// </para>
/// </summary>
public class PorcaoParserTests
{
    private const int Precisao = 4;

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Parse_TextoVazio_RetornaTudoNulo(string? texto)
    {
        var resultado = PorcaoParser.Parse(texto);

        Assert.Null(resultado.TextoOriginal);
        Assert.Null(resultado.Dose);
        Assert.Null(resultado.Unidade);
        Assert.Null(resultado.Quantidade);
    }

    [Fact]
    public void Parse_MedidaEntreParenteses_SeparaDoseDaMedida()
    {
        var resultado = PorcaoParser.Parse("1 fatia (30 g)");

        Assert.Equal("1 fatia (30 g)", resultado.TextoOriginal);
        Assert.Equal("1 fatia", resultado.Dose);
        Assert.Equal("g", resultado.Unidade);
        Assert.Equal(30.0, resultado.Quantidade!.Value, Precisao);
    }

    [Fact]
    public void Parse_MedidaSemParenteses_ExtraiDiretoEDeixaDoseNula()
    {
        var resultado = PorcaoParser.Parse("200ml");

        Assert.Equal("200ml", resultado.TextoOriginal);
        Assert.Null(resultado.Dose);
        Assert.Equal("ml", resultado.Unidade);
        Assert.Equal(200.0, resultado.Quantidade!.Value, Precisao);
    }

    [Fact]
    public void Parse_TextoSemNenhumaMedida_MantemTudoComoDose()
    {
        var resultado = PorcaoParser.Parse("1 unidade");

        Assert.Equal("1 unidade", resultado.Dose);
        Assert.Null(resultado.Unidade);
        Assert.Null(resultado.Quantidade);
    }

    /// <summary>
    /// As tabelas escrevem a mesma unidade de várias formas. O parser normaliza
    /// para uma forma canônica antes de devolver.
    /// </summary>
    [Theory]
    [InlineData("15gr", "g")]
    [InlineData("15 grama", "g")]
    [InlineData("15 gramas", "g")]
    [InlineData("15 g", "g")]
    [InlineData("15 ug", "mcg")]
    [InlineData("15 mcg", "mcg")]
    [InlineData("15 mg", "mg")]
    [InlineData("15 kg", "kg")]
    [InlineData("15 ml", "ml")]
    [InlineData("15 l", "l")]
    public void Parse_VariacoesDeUnidade_NormalizaParaFormaCanonica(string texto, string unidadeEsperada)
    {
        var resultado = PorcaoParser.Parse(texto);

        Assert.Equal(unidadeEsperada, resultado.Unidade);
        Assert.Equal(15.0, resultado.Quantidade!.Value, Precisao);
    }

    [Theory]
    [InlineData("2,5 kg", 2.5)]
    [InlineData("0,75 l", 0.75)]
    [InlineData("100 g", 100.0)]
    public void Parse_QuantidadeDecimalPtBr_UsaVirgulaComoSeparador(string texto, double esperado)
    {
        var resultado = PorcaoParser.Parse(texto);

        Assert.Equal(esperado, resultado.Quantidade!.Value, Precisao);
    }

    /// <summary>
    /// BUG CONHECIDO — este teste documenta o comportamento atual, não o desejado.
    /// <para>
    /// <c>ParseNumero</c> (PorcaoParser.cs:73) tenta pt-BR primeiro com
    /// <c>NumberStyles.Any</c>, que inclui <c>AllowThousands</c>. Em pt-BR o
    /// separador de milhar é o ponto, então "2.5" é lido como 25 e o fallback
    /// para <c>InvariantCulture</c> nunca é alcançado. Erro de 10x na quantidade.
    /// </para>
    /// <para>
    /// A correção é remover <c>AllowThousands</c> do parse pt-BR
    /// (usar <c>NumberStyles.Float</c>). Quando corrigirem, este teste falha —
    /// é o sinal para movê-lo para o Theory acima.
    /// </para>
    /// </summary>
    [Fact]
    public void Parse_QuantidadeComPontoDecimal_InterpretaComoSeparadorDeMilhar_BugConhecido()
    {
        var resultado = PorcaoParser.Parse("2.5 kg");

        Assert.Equal(25.0, resultado.Quantidade!.Value, Precisao);
    }

    [Fact]
    public void Parse_EspacosExcedentes_NormalizaParaUmEspacoSimples()
    {
        var resultado = PorcaoParser.Parse("  1    fatia   (30 g)  ");

        Assert.Equal("1 fatia (30 g)", resultado.TextoOriginal);
        Assert.Equal("1 fatia", resultado.Dose);
    }

    [Fact]
    public void Parse_MaisDeUmParenteses_UsaOUltimoQueContemMedida()
    {
        // "(média)" não tem medida; a busca é de trás para frente e para em "(50 g)".
        var resultado = PorcaoParser.Parse("porção (média) (50 g)");

        Assert.Equal("porção (média)", resultado.Dose);
        Assert.Equal("g", resultado.Unidade);
        Assert.Equal(50.0, resultado.Quantidade!.Value, Precisao);
    }

    [Fact]
    public void Parse_MedidaEmParentesesNoInicio_DeixaORestanteComoDose()
    {
        var resultado = PorcaoParser.Parse("(30 g) de queijo");

        Assert.Equal("de queijo", resultado.Dose);
        Assert.Equal("g", resultado.Unidade);
        Assert.Equal(30.0, resultado.Quantidade!.Value, Precisao);
    }

    [Fact]
    public void Parse_ApenasMedidaEntreParenteses_DeixaDoseNula()
    {
        var resultado = PorcaoParser.Parse("(100 g)");

        Assert.Null(resultado.Dose);
        Assert.Equal("g", resultado.Unidade);
        Assert.Equal(100.0, resultado.Quantidade!.Value, Precisao);
    }

    [Fact]
    public void Parse_DoseComPontuacaoNasPontas_RemoveSujeiraDaBorda()
    {
        var resultado = PorcaoParser.Parse("1 colher de sopa - (15 g)");

        Assert.Equal("1 colher de sopa", resultado.Dose);
    }
}
