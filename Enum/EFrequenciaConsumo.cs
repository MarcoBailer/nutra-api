namespace Nutra.Enum;

/// <summary>
/// Frequência de consumo de alimentos (utilizado na anamnese alimentar).
/// </summary>
public enum EFrequenciaConsumo
{
    Nunca = 0,
    Raramente = 1,         // 1-2x por mês
    Eventualmente = 2,     // 1-2x por semana
    Frequentemente = 3,    // 3-5x por semana
    Diariamente = 4,       // Todos os dias
    VariasVezesAoDia = 5   // Múltiplas vezes ao dia
}
