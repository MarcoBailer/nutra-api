namespace Nutra.Enum;

/// <summary>
/// Métodos disponíveis para cálculo de peso ideal.
/// </summary>
public enum EMetodoPesoIdeal
{
    /// <summary>
    /// Devine (1974) — Mais utilizado na prática clínica.
    /// </summary>
    Devine = 1,

    /// <summary>
    /// Hamwi (1964).
    /// </summary>
    Hamwi = 2,

    /// <summary>
    /// Robinson (1983).
    /// </summary>
    Robinson = 3,

    /// <summary>
    /// Miller (1983).
    /// </summary>
    Miller = 4,

    /// <summary>
    /// Baseado no IMC ideal (22 kg/m²).
    /// </summary>
    IMC = 5
}
