using Nutra.Enum;
using System.ComponentModel.DataAnnotations;

namespace Nutra.Models.Dtos;

/// <summary>
/// DTO para preenchimento da anamnese alimentar.
/// </summary>
public class AnamneseAlimentarDto
{
    // --- Rotina alimentar ---
    public int RefeicoesPorDia { get; set; }

    public TimeSpan? HorarioCafeManha { get; set; }
    public TimeSpan? HorarioAlmoco { get; set; }
    public TimeSpan? HorarioLancheTarde { get; set; }
    public TimeSpan? HorarioJantar { get; set; }
    public TimeSpan? HorarioCeia { get; set; }

    [MaxLength(500)]
    public string? RefeicoesPuladas { get; set; }

    // --- Consumo de água e bebidas ---
    public double ConsumoAguaLitrosDia { get; set; }
    public EFrequenciaConsumo ConsumoRefrigerantes { get; set; }
    public EFrequenciaConsumo ConsumoAlcool { get; set; }
    public EFrequenciaConsumo ConsumoCafeCha { get; set; }

    // --- Hábitos gerais ---
    public EFrequenciaConsumo ConsumoFastFood { get; set; }
    public EFrequenciaConsumo ConsumoFrutas { get; set; }
    public EFrequenciaConsumo ConsumoVerduras { get; set; }
    public EFrequenciaConsumo ConsumoDoces { get; set; }
    public EFrequenciaConsumo ConsumoFrituras { get; set; }

    // --- Comportamento alimentar ---
    public bool ComeComDistracao { get; set; }
    public bool CompulsaoAlimentar { get; set; }
    public bool HistoricoDietasRestritivas { get; set; }

    [MaxLength(1000)]
    public string? DescricaoDietasAnteriores { get; set; }

    [MaxLength(1000)]
    public string? SuplementosEmUso { get; set; }

    // --- Intestino / digestão ---
    public bool IntestinoRegular { get; set; }
    public int? FrequenciaEvacuacaoSemana { get; set; }

    [MaxLength(1000)]
    public string? QueixasDigestivas { get; set; }

    // --- Observações ---
    [MaxLength(1000)]
    public string? AlimentosQueNaoGosta { get; set; }

    [MaxLength(1000)]
    public string? AlimentosPreferidos { get; set; }

    [MaxLength(2000)]
    public string? ObservacoesGerais { get; set; }
}
