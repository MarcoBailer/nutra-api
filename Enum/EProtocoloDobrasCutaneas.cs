namespace Nutra.Enum;

/// <summary>
/// Protocolos disponíveis para estimativa do percentual de gordura por dobras cutâneas.
/// </summary>
public enum EProtocoloDobrasCutaneas
{
    /// <summary>
    /// Jackson &amp; Pollock 3 dobras — Homens: peitoral, abdominal, coxa.
    /// Mulheres: tríceps, suprailíaca, coxa.
    /// </summary>
    JacksonPollock3 = 0,

    /// <summary>
    /// Jackson &amp; Pollock 7 dobras — peitoral, axilar média, tríceps, subescapular, 
    /// abdominal, suprailíaca, coxa.
    /// </summary>
    JacksonPollock7 = 1,

    /// <summary>
    /// Protocolo de Guedes 3 dobras — Homens: tríceps, suprailíaca, abdominal.
    /// Mulheres: tríceps, suprailíaca, coxa.
    /// </summary>
    Guedes3 = 2,

    /// <summary>
    /// Protocolo de Petroski — 4 dobras dependendo do sexo.
    /// </summary>
    Petroski = 3
}
