namespace Nutra.Enum;

/// <summary>
/// Fórmulas disponíveis para cálculo da TMB (Taxa Metabólica Basal).
/// </summary>
public enum EFormulaCalculo
{
    /// <summary>
    /// Fórmula de Mifflin-St Jeor (1990) — considerada a mais precisa para adultos saudáveis.
    /// Homens: (10 × peso) + (6.25 × altura) - (5 × idade) + 5
    /// Mulheres: (10 × peso) + (6.25 × altura) - (5 × idade) - 161
    /// </summary>
    MifflinStJeor = 0,

    /// <summary>
    /// Fórmula de Harris-Benedict revisada (Roza &amp; Shizgal, 1984).
    /// Homens: 88.362 + (13.397 × peso) + (4.799 × altura) - (5.677 × idade)
    /// Mulheres: 447.593 + (9.247 × peso) + (3.098 × altura) - (4.330 × idade)
    /// </summary>
    HarrisBenedict = 1,

    /// <summary>
    /// Fórmula de Katch-McArdle — usa massa magra, ideal quando se tem dados de bioimpedância.
    /// TMB = 370 + (21.6 × massa magra em kg)
    /// </summary>
    KatchMcArdle = 2
}
