using Nutra.Enum;
using Nutra.Models.Usuario;
using Nutra.Services;

namespace Nutra.Test.Core.Services;

/// <summary>
/// Testes de <see cref="CalculadoraNutricionalService"/>.
/// <para>
/// Este é o alvo ideal de teste unitário: o serviço não tem dependência nenhuma
/// (nem banco, nem rede, nem relógio, exceto em <c>CalcularIdade</c>). Cada método
/// é uma função determinística — mesma entrada, sempre a mesma saída.
/// </para>
/// <para>
/// Os valores esperados foram calculados à mão a partir das fórmulas citadas na
/// documentação do próprio serviço. Se um teste destes quebrar, ou a fórmula mudou
/// de propósito (atualize o teste) ou alguém a quebrou sem querer (corrija o código).
/// </para>
/// </summary>
public class CalculadoraNutricionalServiceTests
{
    /// <summary>
    /// Casas decimais comparadas em asserções de <c>double</c>. Comparar ponto
    /// flutuante por igualdade exata é frágil; a precisão explícita evita falso vermelho.
    /// </summary>
    private const int Precisao = 4;

    private readonly CalculadoraNutricionalService _calculadora = new();

    // =====================================================================
    //  TMB — TAXA METABÓLICA BASAL
    // =====================================================================

    [Fact]
    public void CalcularTMB_MifflinStJeor_Masculino_SomaConstante5()
    {
        // (10 x 80) + (6.25 x 180) - (5 x 30) + 5 = 1780
        var tmb = _calculadora.CalcularTMB_MifflinStJeor(80, 180, 30, EGeneroBiologico.Masculino);

        Assert.Equal(1780.0, tmb, Precisao);
    }

    [Fact]
    public void CalcularTMB_MifflinStJeor_Feminino_SubtraiConstante161()
    {
        // (10 x 65) + (6.25 x 170) - (5 x 30) - 161 = 1401.5
        var tmb = _calculadora.CalcularTMB_MifflinStJeor(65, 170, 30, EGeneroBiologico.Feminino);

        Assert.Equal(1401.5, tmb, Precisao);
    }

    [Fact]
    public void CalcularTMB_MifflinStJeor_MesmasMedidas_DiferencaEntreGenerosEh166()
    {
        // A única diferença entre os sexos na fórmula é a constante: +5 vs -161.
        var masculino = _calculadora.CalcularTMB_MifflinStJeor(70, 175, 40, EGeneroBiologico.Masculino);
        var feminino = _calculadora.CalcularTMB_MifflinStJeor(70, 175, 40, EGeneroBiologico.Feminino);

        Assert.Equal(166.0, masculino - feminino, Precisao);
    }

    [Fact]
    public void CalcularTMB_HarrisBenedict_Masculino_RetornaValorDaFormulaRevisada()
    {
        // 88.362 + (13.397 x 80) + (4.799 x 180) - (5.677 x 30) = 1853.632 -> 1853.6
        var tmb = _calculadora.CalcularTMB_HarrisBenedict(80, 180, 30, EGeneroBiologico.Masculino);

        Assert.Equal(1853.6, tmb, Precisao);
    }

    [Fact]
    public void CalcularTMB_HarrisBenedict_Feminino_RetornaValorDaFormulaRevisada()
    {
        // 447.593 + (9.247 x 65) + (3.098 x 170) - (4.330 x 30) = 1445.408 -> 1445.4
        var tmb = _calculadora.CalcularTMB_HarrisBenedict(65, 170, 30, EGeneroBiologico.Feminino);

        Assert.Equal(1445.4, tmb, Precisao);
    }

    [Fact]
    public void CalcularTMB_KatchMcArdle_MassaMagraInformada_UsaApenasMassaMagra()
    {
        // 370 + (21.6 x 60) = 1666
        var tmb = _calculadora.CalcularTMB_KatchMcArdle(60);

        Assert.Equal(1666.0, tmb, Precisao);
    }

    [Fact]
    public void CalcularTMB_KatchMcArdle_MassaMagraZero_RetornaApenasAConstante()
    {
        Assert.Equal(370.0, _calculadora.CalcularTMB_KatchMcArdle(0), Precisao);
    }

    // =====================================================================
    //  GET — GASTO ENERGÉTICO TOTAL
    // =====================================================================

    /// <summary>
    /// [Theory] + [InlineData] roda o MESMO teste várias vezes com entradas diferentes.
    /// Aqui cobre os cinco fatores de atividade em um só método, sem copiar e colar.
    /// </summary>
    [Theory]
    [InlineData(ENivelAtividadeFisica.Sedentario, 1200.0)]
    [InlineData(ENivelAtividadeFisica.LevementeAtivo, 1375.0)]
    [InlineData(ENivelAtividadeFisica.ModeradamenteAtivo, 1550.0)]
    [InlineData(ENivelAtividadeFisica.MuitoAtivo, 1725.0)]
    [InlineData(ENivelAtividadeFisica.ExtremamenteAtivo, 1900.0)]
    public void CalcularGET_NivelDeAtividade_MultiplicaTmbPeloFatorCorreto(
        ENivelAtividadeFisica nivel, double esperado)
    {
        var get = _calculadora.CalcularGET(tmb: 1000, nivel);

        Assert.Equal(esperado, get, Precisao);
    }

    [Theory]
    [InlineData(ETipoObjetivo.PerdaDeGordura, 1600.0)]      // déficit de 20%
    [InlineData(ETipoObjetivo.Hipertrofia, 2200.0)]         // superávit de 10%
    [InlineData(ETipoObjetivo.RecomposicaoCorporal, 1900.0)] // déficit leve de 5%
    [InlineData(ETipoObjetivo.SaudeMetabolica, 2000.0)]     // manutenção
    [InlineData(ETipoObjetivo.PerformanceEsportiva, 2300.0)] // superávit de 15%
    [InlineData(ETipoObjetivo.GanhoDeEnergia, 2100.0)]      // superávit de 5%
    [InlineData(ETipoObjetivo.Manutencao, 2000.0)]          // cai no default (fator 1.0)
    public void AjustarCaloriasPeloObjetivo_Objetivo_AplicaDeficitOuSuperavitCorreto(
        ETipoObjetivo objetivo, double esperado)
    {
        var calorias = _calculadora.AjustarCaloriasPeloObjetivo(get: 2000, objetivo);

        Assert.Equal(esperado, calorias, Precisao);
    }

    // =====================================================================
    //  IMC
    // =====================================================================

    [Fact]
    public void CalcularIMC_MedidasValidas_ArredondaEmDuasCasas()
    {
        // 80 / (1.80^2) = 24.6913... -> 24.69
        var (imc, classificacao) = _calculadora.CalcularIMC(80, 180);

        Assert.Equal(24.69m, imc);
        Assert.Equal("Eutrófico (normal)", classificacao);
    }

    /// <summary>
    /// Altura fixa em 200 cm (2 m² exatos) para que o IMC seja simplesmente peso/4.
    /// Os pesos escolhidos caem exatamente sobre os limites das faixas da OMS —
    /// é ali que erros de comparação (&lt; vs &lt;=) aparecem.
    /// </summary>
    [Theory]
    [InlineData(60, "Magreza grau III (grave)")]      // IMC 15.00
    [InlineData(64, "Magreza grau II (moderada)")]    // IMC 16.00 — limite
    [InlineData(68, "Magreza grau I (leve)")]         // IMC 17.00 — limite
    [InlineData(74, "Eutrófico (normal)")]            // IMC 18.50 — limite
    [InlineData(100, "Sobrepeso (pré-obeso)")]        // IMC 25.00 — limite
    [InlineData(120, "Obesidade grau I")]             // IMC 30.00 — limite
    [InlineData(140, "Obesidade grau II")]            // IMC 35.00 — limite
    [InlineData(160, "Obesidade grau III (mórbida)")] // IMC 40.00 — limite
    public void CalcularIMC_LimitesDasFaixas_ClassificaSegundoOms(double pesoKg, string esperada)
    {
        var (_, classificacao) = _calculadora.CalcularIMC(pesoKg, 200);

        Assert.Equal(esperada, classificacao);
    }

    /// <summary>
    /// BUG CONHECIDO — este teste documenta o comportamento atual, não o desejado.
    /// <para>
    /// <c>CalcularIMC</c> (CalculadoraNutricionalService.cs:135) não tem guarda para
    /// altura zero, ao contrário de <c>CalcularRCQ</c>, que protege o denominador em
    /// CalculadoraNutricionalService.cs:165. Altura 0 gera <c>double.PositiveInfinity</c>,
    /// e o cast para <c>decimal</c> estoura.
    /// </para>
    /// <para>
    /// O caminho é alcançável: <c>PerfilNutricionalDto.AlturaCm</c> não tem nenhum
    /// atributo de validação e chega crua em AvaliacaoNutricionalService.cs:319.
    /// Um POST com <c>alturaCm: 0</c> vira HTTP 500 no ExceptionMiddleware, quando
    /// deveria ser 400. Correção: validar a altura no DTO ou espelhar a guarda do RCQ.
    /// </para>
    /// </summary>
    [Fact]
    public void CalcularIMC_AlturaZero_EstouraEmVezDeRetornarDadoInsuficiente_BugConhecido()
    {
        Assert.Throws<OverflowException>(() => _calculadora.CalcularIMC(80, 0));
    }

    // =====================================================================
    //  RCQ — RELAÇÃO CINTURA/QUADRIL
    // =====================================================================

    [Fact]
    public void CalcularRCQ_QuadrilZero_RetornaZeroEDadoInsuficiente()
    {
        // Divisão por zero é evitada por guarda explícita no serviço.
        var (rcq, classificacao) = _calculadora.CalcularRCQ(85, 0, EGeneroBiologico.Masculino);

        Assert.Equal(0m, rcq);
        Assert.Equal("Dado insuficiente", classificacao);
    }

    [Theory]
    [InlineData(85, "Risco baixo")]     // 0.85
    [InlineData(90, "Risco baixo")]     // 0.90 — limite superior do baixo
    [InlineData(95, "Risco moderado")]  // 0.95
    [InlineData(99, "Risco moderado")]  // 0.99 — limite superior do moderado
    [InlineData(105, "Risco alto")]     // 1.05
    public void CalcularRCQ_Masculino_UsaFaixasMasculinas(double cinturaCm, string esperada)
    {
        var (_, classificacao) = _calculadora.CalcularRCQ(cinturaCm, 100, EGeneroBiologico.Masculino);

        Assert.Equal(esperada, classificacao);
    }

    [Theory]
    [InlineData(75, "Risco baixo")]     // 0.75
    [InlineData(80, "Risco baixo")]     // 0.80 — limite superior do baixo
    [InlineData(82, "Risco moderado")]  // 0.82
    [InlineData(84, "Risco moderado")]  // 0.84 — limite superior do moderado
    [InlineData(90, "Risco alto")]      // 0.90
    public void CalcularRCQ_Feminino_UsaFaixasFemininasMaisRestritivas(double cinturaCm, string esperada)
    {
        var (_, classificacao) = _calculadora.CalcularRCQ(cinturaCm, 100, EGeneroBiologico.Feminino);

        Assert.Equal(esperada, classificacao);
    }

    [Fact]
    public void CalcularRCQ_MesmaRelacao_ClassificaMulherComRiscoMaiorQueHomem()
    {
        // RCQ 0.85 é "baixo" para homem e "alto" para mulher.
        var (_, homem) = _calculadora.CalcularRCQ(85, 100, EGeneroBiologico.Masculino);
        var (_, mulher) = _calculadora.CalcularRCQ(85, 100, EGeneroBiologico.Feminino);

        Assert.Equal("Risco baixo", homem);
        Assert.Equal("Risco alto", mulher);
    }

    // =====================================================================
    //  DOBRAS CUTÂNEAS
    // =====================================================================

    [Fact]
    public void CalcularGorduraPorDobras_JP3_Masculino_AplicaEquacaoMasculinaESiri()
    {
        // Soma = 45, idade 30 -> Dc = 1.0677 -> %G = (4.95/Dc - 4.50) x 100 = 13.61
        var (densidade, percentual) = _calculadora.CalcularGorduraPorDobras_JP3(
            [10, 15, 20], idade: 30, EGeneroBiologico.Masculino);

        Assert.Equal(1.0677m, densidade);
        Assert.Equal(13.61m, percentual);
    }

    [Fact]
    public void CalcularGorduraPorDobras_JP3_Feminino_AplicaEquacaoFeminina()
    {
        // Soma = 65, idade 25 -> Dc = 1.0412 -> %G = 25.42
        var (densidade, percentual) = _calculadora.CalcularGorduraPorDobras_JP3(
            [18, 22, 25], idade: 25, EGeneroBiologico.Feminino);

        Assert.Equal(1.0412m, densidade);
        Assert.Equal(25.42m, percentual);
    }

    [Fact]
    public void CalcularGorduraPorDobras_JP3_MaisDeTresDobras_UsaApenasAsTresPrimeiras()
    {
        var apenasTres = _calculadora.CalcularGorduraPorDobras_JP3(
            [10, 15, 20], idade: 30, EGeneroBiologico.Masculino);

        var comExtras = _calculadora.CalcularGorduraPorDobras_JP3(
            [10, 15, 20, 99, 99], idade: 30, EGeneroBiologico.Masculino);

        Assert.Equal(apenasTres, comExtras);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    public void CalcularGorduraPorDobras_JP3_MenosDeTresDobras_LancaArgumentException(int quantidade)
    {
        // Guarda de programação: chegar aqui com menos de 3 dobras é bug do chamador,
        // por isso o serviço lança exceção em vez de devolver RetornoPadrao.
        var dobras = new double[quantidade];

        var erro = Assert.Throws<ArgumentException>(
            () => _calculadora.CalcularGorduraPorDobras_JP3(dobras, 30, EGeneroBiologico.Masculino));

        Assert.Equal("dobras", erro.ParamName);
    }

    [Fact]
    public void CalcularGorduraPorDobras_JP3_ArrayNulo_LancaArgumentException()
    {
        Assert.Throws<ArgumentException>(
            () => _calculadora.CalcularGorduraPorDobras_JP3(null!, 30, EGeneroBiologico.Masculino));
    }

    [Fact]
    public void CalcularGorduraPorDobras_JP7_Masculino_SomaTodasAsSeteDobras()
    {
        // Soma = 112, idade 30 -> Dc = 1.0615 -> %G = 16.31
        var (densidade, percentual) = _calculadora.CalcularGorduraPorDobras_JP7(
            [10, 12, 14, 16, 18, 20, 22], idade: 30, EGeneroBiologico.Masculino);

        Assert.Equal(1.0615m, densidade);
        Assert.Equal(16.31m, percentual);
    }

    [Fact]
    public void CalcularGorduraPorDobras_JP7_Feminino_AplicaEquacaoFeminina()
    {
        // Soma = 112, idade 25 -> Dc = 1.0482 -> %G = 22.23
        var (densidade, percentual) = _calculadora.CalcularGorduraPorDobras_JP7(
            [10, 12, 14, 16, 18, 20, 22], idade: 25, EGeneroBiologico.Feminino);

        Assert.Equal(1.0482m, densidade);
        Assert.Equal(22.23m, percentual);
    }

    [Fact]
    public void CalcularGorduraPorDobras_JP7_MenosDeSeteDobras_LancaArgumentException()
    {
        var erro = Assert.Throws<ArgumentException>(
            () => _calculadora.CalcularGorduraPorDobras_JP7(
                [10, 12, 14, 16, 18, 20], 30, EGeneroBiologico.Masculino));

        Assert.Equal("dobras", erro.ParamName);
    }

    [Fact]
    public void CalcularGorduraPorDobras_JP7_DobrasMuitoBaixas_NuncaRetornaPercentualNegativo()
    {
        // Siri pode gerar valor negativo em densidades altíssimas; o serviço aplica piso zero.
        var (_, percentual) = _calculadora.CalcularGorduraPorDobras_JP7(
            [1, 1, 1, 1, 1, 1, 1], idade: 18, EGeneroBiologico.Masculino);

        Assert.True(percentual >= 0m, $"Percentual de gordura não pode ser negativo, veio {percentual}.");
    }

    // =====================================================================
    //  PESO IDEAL
    // =====================================================================

    [Fact]
    public void CalcularPesoIdeal_Devine_Masculino_UsaBase50Kg()
    {
        // 180cm = 70.866 pol -> 50 + 2.3 x (70.866 - 60) = 74.99 -> 75.0
        Assert.Equal(75.0, _calculadora.CalcularPesoIdeal_Devine(180, EGeneroBiologico.Masculino), Precisao);
    }

    [Fact]
    public void CalcularPesoIdeal_Devine_Feminino_UsaBase45Ponto5Kg()
    {
        // 165cm = 64.961 pol -> 45.5 + 2.3 x (64.961 - 60) = 56.91 -> 56.9
        Assert.Equal(56.9, _calculadora.CalcularPesoIdeal_Devine(165, EGeneroBiologico.Feminino), Precisao);
    }

    [Fact]
    public void CalcularPesoIdeal_Devine_AlturaMuitoBaixa_AplicaPisoDeSeguranca30Kg()
    {
        // Devine cru daria 2.55 kg para 100cm — absurdo clínico, o piso protege.
        Assert.Equal(30.0, _calculadora.CalcularPesoIdeal_Devine(100, EGeneroBiologico.Masculino), Precisao);
    }

    [Fact]
    public void CalcularPesoIdeal_IMC_UsaImcIdealDe22()
    {
        // 22 x 1.80^2 = 71.28 -> 71.3
        Assert.Equal(71.3, _calculadora.CalcularPesoIdeal_IMC(180), Precisao);
    }

    // =====================================================================
    //  MACRONUTRIENTES
    // =====================================================================

    [Fact]
    public void CalcularMacronutrientes_Hipertrofia_DistribuiProteinaGorduraECarboRestante()
    {
        // Peso 80 / meta 2400 kcal / hipertrofia (2.0 g proteína por kg):
        //   proteína = 80 x 2.0 = 160 g  -> 640 kcal
        //   gordura  = 80 x 0.9 = 72 g   -> 648 kcal
        //   carbo    = (2400 - 1288) / 4 = 278 g
        //   fibra    = (2400/1000) x 14  = 34 g
        //   água     = 80 x 0.035        = 2.8 L
        var (prot, carb, gord, fibra, agua) =
            _calculadora.CalcularMacronutrientes(80, 2400, ETipoObjetivo.Hipertrofia);

        Assert.Equal(160.0, prot, Precisao);
        Assert.Equal(278.0, carb, Precisao);
        Assert.Equal(72.0, gord, Precisao);
        Assert.Equal(34.0, fibra, Precisao);
        Assert.Equal(2.8, agua, Precisao);
    }

    [Theory]
    [InlineData(ETipoObjetivo.Hipertrofia, 200.0)]          // 2.0 g/kg
    [InlineData(ETipoObjetivo.PerdaDeGordura, 220.0)]       // 2.2 g/kg — protege massa magra no déficit
    [InlineData(ETipoObjetivo.RecomposicaoCorporal, 240.0)] // 2.4 g/kg
    [InlineData(ETipoObjetivo.PerformanceEsportiva, 200.0)] // 2.0 g/kg
    [InlineData(ETipoObjetivo.SaudeMetabolica, 180.0)]      // default 1.8 g/kg
    [InlineData(ETipoObjetivo.GanhoDeEnergia, 180.0)]       // default 1.8 g/kg
    [InlineData(ETipoObjetivo.Manutencao, 180.0)]           // default 1.8 g/kg
    public void CalcularMacronutrientes_Objetivo_DefineGramasDeProteinaPorKg(
        ETipoObjetivo objetivo, double proteinaEsperada)
    {
        var (prot, _, _, _, _) = _calculadora.CalcularMacronutrientes(100, 3000, objetivo);

        Assert.Equal(proteinaEsperada, prot, Precisao);
    }

    [Fact]
    public void CalcularMacronutrientes_MetaCaloricaMenorQueProteinaEGordura_ZeraCarboidratoEmVezDeNegativar()
    {
        // Proteína + gordura já custam 1610 kcal; a meta é 500. Carbo tem piso zero.
        var (_, carb, _, _, _) =
            _calculadora.CalcularMacronutrientes(100, 500, ETipoObjetivo.Hipertrofia);

        Assert.Equal(0.0, carb, Precisao);
    }

    [Fact]
    public void CalcularMacronutrientes_GorduraIndependeDoObjetivo_SempreZeroPonto9PorKg()
    {
        var (_, _, gordCorte, _, _) = _calculadora.CalcularMacronutrientes(70, 2000, ETipoObjetivo.PerdaDeGordura);
        var (_, _, gordBulk, _, _) = _calculadora.CalcularMacronutrientes(70, 3000, ETipoObjetivo.Hipertrofia);

        Assert.Equal(63.0, gordCorte, Precisao);
        Assert.Equal(63.0, gordBulk, Precisao);
    }

    // =====================================================================
    //  IDADE
    // =====================================================================

    [Fact]
    public void CalcularIdade_AniversarioJaOcorreuNesteAno_RetornaIdadeCheia()
    {
        var nascimento = DateTime.Today.AddYears(-30);

        Assert.Equal(30, CalculadoraNutricionalService.CalcularIdade(nascimento));
    }

    [Fact]
    public void CalcularIdade_AniversarioAindaNaoOcorreu_DescontaUmAno()
    {
        // Nasceu "amanhã" há 30 anos: ainda não fez 30.
        var nascimento = DateTime.Today.AddYears(-30).AddDays(1);

        Assert.Equal(29, CalculadoraNutricionalService.CalcularIdade(nascimento));
    }

    [Fact]
    public void CalcularIdade_AniversarioEhHoje_JaContaOAnoNovo()
    {
        var nascimento = DateTime.Today.AddYears(-25);

        Assert.Equal(25, CalculadoraNutricionalService.CalcularIdade(nascimento));
    }

    // =====================================================================
    //  GERAR META INICIAL (composição de todos os cálculos acima)
    // =====================================================================

    [Fact]
    public void GerarMetaInicial_PerfilCompleto_ComponhaTmbGetObjetivoEMacros()
    {
        var perfil = CriarPerfil();

        var meta = _calculadora.GerarMetaInicial(perfil);

        // Reproduz a cadeia esperada: TMB -> GET -> ajuste por objetivo -> macros.
        var idade = CalculadoraNutricionalService.CalcularIdade(perfil.DataNascimento);
        var tmb = _calculadora.CalcularTMB_MifflinStJeor(perfil.PesoAtualKg, perfil.AlturaCm, idade, perfil.Genero);
        var get = _calculadora.CalcularGET(tmb, perfil.NivelAtividade);
        var caloriasEsperadas = Math.Round(_calculadora.AjustarCaloriasPeloObjetivo(get, perfil.Objetivo));
        var (prot, carb, gord, fibra, agua) =
            _calculadora.CalcularMacronutrientes(perfil.PesoAtualKg, caloriasEsperadas, perfil.Objetivo);

        Assert.Equal(caloriasEsperadas, meta.CaloriasDiarias, Precisao);
        Assert.Equal(prot, meta.ProteinasDiarias, Precisao);
        Assert.Equal(carb, meta.CarboidratosDiarios, Precisao);
        Assert.Equal(gord, meta.GordurasDiarias, Precisao);
        Assert.Equal(fibra, meta.FibraDiaria, Precisao);
        Assert.Equal(agua, meta.AguaDiaria, Precisao);
    }

    [Fact]
    public void GerarMetaInicial_QualquerPerfil_VinculaMetaAoPerfilEDataEmUtc()
    {
        var perfil = CriarPerfil();
        perfil.Id = 42;

        var meta = _calculadora.GerarMetaInicial(perfil);

        Assert.Equal(42, meta.PerfilNutricionalId);
        Assert.Equal(DateTimeKind.Utc, meta.DataCalculo.Kind);
    }

    [Fact]
    public void GerarMetaInicial_MesmoPerfilComObjetivoDeCorte_GeraMenosCaloriasQueHipertrofia()
    {
        var corte = CriarPerfil();
        corte.Objetivo = ETipoObjetivo.PerdaDeGordura;

        var bulk = CriarPerfil();
        bulk.Objetivo = ETipoObjetivo.Hipertrofia;

        Assert.True(
            _calculadora.GerarMetaInicial(corte).CaloriasDiarias <
            _calculadora.GerarMetaInicial(bulk).CaloriasDiarias);
    }

    /// <summary>
    /// Fábrica de perfil válido. Centralizar aqui evita repetir 8 propriedades em cada teste
    /// e deixa explícito qual campo cada teste realmente varia.
    /// </summary>
    private static PerfilNutricional CriarPerfil() => new()
    {
        Id = 1,
        PesoAtualKg = 80,
        AlturaCm = 180,
        DataNascimento = DateTime.Today.AddYears(-30),
        Genero = EGeneroBiologico.Masculino,
        NivelAtividade = ENivelAtividadeFisica.ModeradamenteAtivo,
        Objetivo = ETipoObjetivo.Hipertrofia
    };
}
