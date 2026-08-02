using Nutra.Enum;
using Nutra.Models.Usuario;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nutra.Models.RegraNutricional;

/// <summary>
/// Anamnese alimentar do paciente — registro detalhado dos hábitos alimentares.
/// Pode haver múltiplas anamneses ao longo do tempo para acompanhamento.
/// </summary>
public class AnamneseAlimentar
{
    public int Id { get; set; }

    public int PerfilNutricionalId { get; set; }

    [ForeignKey("PerfilNutricionalId")]
    public PerfilNutricional PerfilNutricional { get; set; } = null!;

    /// <summary>Data de preenchimento da anamnese.</summary>
    public DateTime DataPreenchimento { get; set; } = DateTime.UtcNow;

    // --- Rotina alimentar ---

    /// <summary>Quantas refeições faz por dia habitualmente.</summary>
    public int RefeicoesPorDia { get; set; }

    /// <summary>Horário aproximado do café da manhã (nullable se pula).</summary>
    public TimeSpan? HorarioCafeManha { get; set; }

    /// <summary>Horário aproximado do almoço.</summary>
    public TimeSpan? HorarioAlmoco { get; set; }

    /// <summary>Horário aproximado do lanche da tarde.</summary>
    public TimeSpan? HorarioLancheTarde { get; set; }

    /// <summary>Horário aproximado do jantar.</summary>
    public TimeSpan? HorarioJantar { get; set; }

    /// <summary>Horário aproximado da ceia (nullable se não faz).</summary>
    public TimeSpan? HorarioCeia { get; set; }

    /// <summary>Costuma pular alguma refeição? Qual?</summary>
    [MaxLength(500)]
    public string? RefeicoesPuladas { get; set; }

    // --- Consumo de água e bebidas ---

    /// <summary>Litros de água por dia (estimativa).</summary>
    public double ConsumoAguaLitrosDia { get; set; }

    /// <summary>Frequência de consumo de refrigerantes/sucos industrializados.</summary>
    public EFrequenciaConsumo ConsumoRefrigerantes { get; set; }

    /// <summary>Frequência de consumo de bebidas alcoólicas.</summary>
    public EFrequenciaConsumo ConsumoAlcool { get; set; }

    /// <summary>Frequência de consumo de café/chá.</summary>
    public EFrequenciaConsumo ConsumoCafeCha { get; set; }

    // --- Hábitos gerais ---

    /// <summary>Frequência de consumo de fast food / ultra processados.</summary>
    public EFrequenciaConsumo ConsumoFastFood { get; set; }

    /// <summary>Frequência de consumo de frutas.</summary>
    public EFrequenciaConsumo ConsumoFrutas { get; set; }

    /// <summary>Frequência de consumo de verduras e legumes.</summary>
    public EFrequenciaConsumo ConsumoVerduras { get; set; }

    /// <summary>Frequência de consumo de doces/sobremesas.</summary>
    public EFrequenciaConsumo ConsumoDoces { get; set; }

    /// <summary>Frequência de consumo de frituras.</summary>
    public EFrequenciaConsumo ConsumoFrituras { get; set; }

    // --- Comportamento alimentar ---

    /// <summary>Come assistindo TV/celular.</summary>
    public bool ComeComDistracao { get; set; }

    /// <summary>Sensação de compulsão alimentar.</summary>
    public bool CompulsaoAlimentar { get; set; }

    /// <summary>Histórico de dietas restritivas.</summary>
    public bool HistoricoDietasRestritivas { get; set; }

    [MaxLength(1000)]
    public string? DescricaoDietasAnteriores { get; set; }

    /// <summary>Usa suplementos? Quais?</summary>
    [MaxLength(1000)]
    public string? SuplementosEmUso { get; set; }

    // --- Intestino / digestão ---

    /// <summary>Funcionamento intestinal regular?</summary>
    public bool IntestinoRegular { get; set; }

    /// <summary>Frequência evacuatória por semana.</summary>
    public int? FrequenciaEvacuacaoSemana { get; set; }

    /// <summary>Queixas digestivas (gases, distensão, refluxo).</summary>
    [MaxLength(1000)]
    public string? QueixasDigestivas { get; set; }

    // --- Observações gerais ---

    /// <summary>Alimentos que o paciente não gosta (texto livre complementar).</summary>
    [MaxLength(1000)]
    public string? AlimentosQueNaoGosta { get; set; }

    /// <summary>Alimentos que o paciente mais gosta (texto livre complementar).</summary>
    [MaxLength(1000)]
    public string? AlimentosPreferidos { get; set; }

    /// <summary>Observações gerais do profissional ou paciente.</summary>
    [MaxLength(2000)]
    public string? ObservacoesGerais { get; set; }

    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
    public DateTime? AtualizadoEm { get; set; }
}
