namespace Nutra.Enum;

/// <summary>
/// Planos de assinatura disponíveis para profissionais.
/// </summary>
public enum EPlanoAssinatura
{
    /// <summary>Gratuito - até 5 pacientes</summary>
    Gratuito = 0,

    /// <summary>Básico - até 30 pacientes</summary>
    Basico = 1,

    /// <summary>Profissional - até 100 pacientes</summary>
    Profissional = 2,

    /// <summary>Enterprise - pacientes ilimitados, multi-clínica</summary>
    Enterprise = 3
}
