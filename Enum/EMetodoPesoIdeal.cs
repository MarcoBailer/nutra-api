namespace Nutra.Enum;

/// <summary>
/// Métodos disponíveis para cálculo de peso ideal.
/// </summary>
public enum EMetodoPesoIdeal
{
    /// <summary>
    /// Devine (1974) — Mais utilizado na prática clínica.
    /// </summary>
    Devine = 0,

    /// <summary>
    /// Hamwi (1964).
    /// </summary>
    Hamwi = 1,

    /// <summary>
    /// Robinson (1983).
    /// </summary>
    Robinson = 2,

    /// <summary>
    /// Miller (1983).
    /// </summary>
    Miller = 3,

    /// <summary>
    /// Baseado no IMC ideal (22 kg/m²).
    /// </summary>
    IMC = 4
}
