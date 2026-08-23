using Nutra.Enum;
using Nutra.Models.Usuario;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nutra.Models.RegraNutricional;

/// <summary>
/// Avaliação antropométrica completa de um paciente.
/// Cada registro é um "snapshot" — um ponto no tempo para acompanhamento progressivo.
/// </summary>
public class AvaliacaoAntropometrica
{
    public int Id { get; set; }

    public int PerfilNutricionalId { get; set; }

    [ForeignKey("PerfilNutricionalId")]
    public PerfilNutricional PerfilNutricional { get; set; } = null!;

    /// <summary>
    /// Profissional que realizou a avaliação (null = autoavaliação).
    /// </summary>
    public string? ProfissionalResponsavelId { get; set; }

    [ForeignKey("ProfissionalResponsavelId")]
    public ApplicationUser? ProfissionalResponsavel { get; set; }

    public DateTime DataAvaliacao { get; set; } = DateTime.UtcNow;

    [MaxLength(2000)]
    public string? Observacoes { get; set; }

    // ===================== MEDIDAS BÁSICAS =====================

    /// <summary>Peso em kg no dia da avaliação.</summary>
    public double PesoKg { get; set; }

    /// <summary>Altura em cm.</summary>
    public double AlturaCm { get; set; }

    /// <summary>IMC calculado automaticamente (Peso / Altura²).</summary>
    [Column(TypeName = "decimal(5,2)")]
    public decimal IMC { get; set; }

    /// <summary>Classificação textual do IMC (Abaixo do peso, Normal, Sobrepeso, etc.).</summary>
    [MaxLength(100)]
    public string ClassificacaoIMC { get; set; } = string.Empty;

    // ===================== CIRCUNFERÊNCIAS (cm) =====================

    public double? CircunferenciaPescocoCm { get; set; }
    public double? CircunferenciaToraxCm { get; set; }
    public double? CircunferenciaCinturaCm { get; set; }
    public double? CircunferenciaAbdomenCm { get; set; }
    public double? CircunferenciaQuadrilCm { get; set; }
    public double? CircunferenciaBracoDireitoCm { get; set; }
    public double? CircunferenciaBracoEsquerdoCm { get; set; }
    public double? CircunferenciaAntebracoDireitoCm { get; set; }
    public double? CircunferenciaAntebracoEsquerdoCm { get; set; }
    public double? CircunferenciaCoxaDireitaCm { get; set; }
    public double? CircunferenciaCoxaEsquerdaCm { get; set; }
    public double? CircunferenciaPanturrilhaDireitaCm { get; set; }
    public double? CircunferenciaPanturrilhaEsquerdaCm { get; set; }

    /// <summary>Relação Cintura/Quadril — fator de risco cardiovascular.</summary>
    [Column(TypeName = "decimal(4,2)")]
    public decimal? RCQ { get; set; }

    // ===================== DOBRAS CUTÂNEAS (mm) =====================

    public EProtocoloDobrasCutaneas? ProtocoloDobrasCutaneas { get; set; }

    public double? DobraTricepsMm { get; set; }
    public double? DobraBicepsMm { get; set; }
    public double? DobraSubescapularMm { get; set; }
    public double? DobraSuprailiacaMm { get; set; }
    public double? DobraAbdominalMm { get; set; }
    public double? DobraCoxaMm { get; set; }
    public double? DobraPanturrilhaMm { get; set; }
    public double? DobraAxilarMediaMm { get; set; }
    public double? DobraPeitoralMm { get; set; }

    /// <summary>Somatório das dobras medidas (mm).</summary>
    public double? SomatorioDobras { get; set; }

    /// <summary>Densidade corporal calculada a partir das dobras.</summary>
    [Column(TypeName = "decimal(6,4)")]
    public decimal? DensidadeCorporal { get; set; }

    /// <summary>Percentual de gordura estimado por dobras cutâneas.</summary>
    [Column(TypeName = "decimal(5,2)")]
    public decimal? PercentualGorduraDobrasCutaneas { get; set; }

    // ===================== BIOIMPEDÂNCIA =====================

    /// <summary>Se houve exame de bioimpedância associado.</summary>
    public bool PossuiBioimpedancia { get; set; }

    /// <summary>Percentual de gordura corporal (bioimpedância).</summary>
    [Column(TypeName = "decimal(5,2)")]
    public decimal? BioPercentualGordura { get; set; }

    /// <summary>Massa magra em kg (bioimpedância).</summary>
    public double? BioMassaMagraKg { get; set; }

    /// <summary>Massa gorda em kg (bioimpedância).</summary>
    public double? BioMassaGordaKg { get; set; }

    /// <summary>Água corporal total em litros (bioimpedância).</summary>
    public double? BioAguaCorporalLitros { get; set; }

    /// <summary>Percentual de água corporal (bioimpedância).</summary>
    [Column(TypeName = "decimal(5,2)")]
    public decimal? BioPercentualAgua { get; set; }

    /// <summary>TMB informada pelo aparelho de bioimpedância (kcal).</summary>
    public double? BioTMBKcal { get; set; }

    /// <summary>Nível de gordura visceral (escala do equipamento).</summary>
    public int? BioGorduraVisceralNivel { get; set; }

    /// <summary>Idade metabólica estimada (bioimpedância).</summary>
    public int? BioIdadeMetabolica { get; set; }

    /// <summary>Massa óssea estimada em kg (bioimpedância).</summary>
    public double? BioMassaOsseaKg { get; set; }

    // ===================== CÁLCULOS AUTOMÁTICOS =====================

    /// <summary>TMB por Mifflin-St Jeor (kcal/dia).</summary>
    public double? TMBMifflinStJeor { get; set; }

    /// <summary>TMB por Harris-Benedict (kcal/dia).</summary>
    public double? TMBHarrisBenedict { get; set; }

    /// <summary>TMB por Katch-McArdle (kcal/dia) — requer massa magra.</summary>
    public double? TMBKatchMcArdle { get; set; }

    /// <summary>Gasto Energético Total (kcal/dia).</summary>
    public double? GET { get; set; }

    /// <summary>Percentual de gordura estimado (melhor valor disponível).</summary>
    [Column(TypeName = "decimal(5,2)")]
    public decimal? PercentualGorduraEstimado { get; set; }

    /// <summary>Massa magra estimada em kg.</summary>
    public double? MassaMagraEstimadaKg { get; set; }

    /// <summary>Massa gorda estimada em kg.</summary>
    public double? MassaGordaEstimadaKg { get; set; }

    /// <summary>Peso ideal estimado (Devine) em kg.</summary>
    public double? PesoIdealDevineKg { get; set; }

    /// <summary>Peso ideal baseado no IMC ideal (22) em kg.</summary>
    public double? PesoIdealIMCKg { get; set; }

    /// <summary>Taxa metabólica ajustada ao objetivo (kcal/dia).</summary>
    public double? TaxaMetabolicaAjustada { get; set; }

    /// <summary>Gramas de proteína recomendadas por dia.</summary>
    public double? ProteinaRecomendadaG { get; set; }

    /// <summary>Gramas de carboidrato recomendadas por dia.</summary>
    public double? CarboidratoRecomendadoG { get; set; }

    /// <summary>Gramas de gordura recomendadas por dia.</summary>
    public double? GorduraRecomendadaG { get; set; }

    // ===================== NAVEGAÇÃO =====================

    public ICollection<FotoProgresso> FotosProgresso { get; set; } = new List<FotoProgresso>();

    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
    public DateTime? AtualizadoEm { get; set; }
}
