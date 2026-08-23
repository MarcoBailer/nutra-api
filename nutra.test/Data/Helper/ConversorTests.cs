using Nutra.Helper;

namespace Nutra.Test.Data.Helper;

/// <summary>
/// Testes de <see cref="Conversor"/>.
/// <para>
/// O conversor limpa os valores nutricionais que vêm em texto das tabelas de
/// alimentos (TBCA, fabricantes). Entrada suja é a regra, não a exceção: "NA",
/// "tr" (traço), "*", vírgula decimal, unidade grudada no número.
/// </para>
/// </summary>
public class ConversorTests
{
    private const int Precisao = 4;

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void LimparEConverter_TextoVazio_RetornaZero(string? valor)
    {
        Assert.Equal(0.0, Conversor.LimparEConverter(valor), Precisao);
    }

    [Theory]
    [InlineData("NA")]   // "não analisado"
    [InlineData("na")]
    [InlineData("tr")]   // "traço"
    [InlineData("TR")]
    [InlineData("*")]
    public void LimparEConverter_MarcadoresDeAusencia_RetornamZero(string valor)
    {
        Assert.Equal(0.0, Conversor.LimparEConverter(valor), Precisao);
    }

    [Theory]
    [InlineData("12", 12.0)]
    [InlineData("0", 0.0)]
    [InlineData("12,5", 12.5)]      // decimal pt-BR
    [InlineData("  7  ", 7.0)]
    [InlineData("12 g", 12.0)]      // unidade separada
    [InlineData("12g", 12.0)]       // unidade grudada
    [InlineData("kcal 250", 250.0)] // número no fim
    public void LimparEConverter_TextoComNumero_ExtraiONumero(string valor, double esperado)
    {
        Assert.Equal(esperado, Conversor.LimparEConverter(valor), Precisao);
    }

    [Fact]
    public void LimparEConverter_TextoSemNumero_RetornaZero()
    {
        Assert.Equal(0.0, Conversor.LimparEConverter("valor desconhecido"), Precisao);
    }

    [Fact]
    public void LimparEConverter_VariosNumeros_UsaApenasOPrimeiro()
    {
        Assert.Equal(10.0, Conversor.LimparEConverter("10 a 20 g"), Precisao);
    }

    /// <summary>
    /// BUG CONHECIDO — este teste documenta o comportamento atual, não o desejado.
    /// <para>
    /// A checagem de "não analisado" usa <c>v.Contains("na")</c>, que casa com
    /// QUALQUER texto contendo "na" — "banana", "manga natural", "carnaúba".
    /// Nesses casos o valor numérico real é descartado e vira zero.
    /// A correção é comparar igualdade (<c>v == "na"</c>), como já é feito com
    /// "tr" e "*" em Conversor.cs:14. Quando corrigirem, este teste falha —
    /// é o sinal para apagá-lo.
    /// </para>
    /// </summary>
    [Fact]
    public void LimparEConverter_TextoContendoNa_DescartaONumero_BugConhecido()
    {
        Assert.Equal(0.0, Conversor.LimparEConverter("banana 89"), Precisao);
    }

    /// <summary>
    /// BUG CONHECIDO — mesmo defeito de <c>PorcaoParser.ParseNumero</c>.
    /// <para>
    /// Conversor.cs:22 tenta pt-BR primeiro com <c>NumberStyles.Any</c>, que inclui
    /// <c>AllowThousands</c>. Em pt-BR o ponto é separador de milhar, então "2.5"
    /// vira 25 e o fallback para <c>InvariantCulture</c> nunca roda. Se alguma
    /// tabela de alimentos usar ponto decimal, todo valor nutricional dela fica
    /// multiplicado por 10.
    /// </para>
    /// <para>
    /// Correção: usar <c>NumberStyles.Float</c> no parse pt-BR.
    /// </para>
    /// </summary>
    [Fact]
    public void LimparEConverter_NumeroComPontoDecimal_InterpretaComoMilhar_BugConhecido()
    {
        Assert.Equal(25.0, Conversor.LimparEConverter("2.5"), Precisao);
    }
}
