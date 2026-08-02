using Nutra.Enum;
using System.ComponentModel.DataAnnotations;

namespace Nutra.Models.Dtos;

/// <summary>
/// DTO de entrada para registrar uma avaliação antropométrica completa.
/// Apenas peso e altura são obrigatórios; todas as demais medidas são opcionais.
/// Os cálculos (IMC, TMB, GET, %gordura, peso ideal, macros) são gerados automaticamente pelo serviço.
/// </summary>
public class AvaliacaoAntropometricaDto
{
    // ===================== MEDIDAS OBRIGATÓRIAS =====================

    [Required(ErrorMessage = "Peso é obrigatório.")]
    [Range(20, 500, ErrorMessage = "Peso deve estar entre 20 e 500 kg.")]
    public double PesoKg { get; set; }

    [Required(ErrorMessage = "Altura é obrigatória.")]
    [Range(50, 280, ErrorMessage = "Altura deve estar entre 50 e 280 cm.")]
    public double AlturaCm { get; set; }

    // ===================== CIRCUNFERÊNCIAS (cm) =====================

    [Range(0, 100)]
    public double? CircunferenciaPescocoCm { get; set; }

    [Range(0, 250)]
    public double? CircunferenciaToraxCm { get; set; }

    [Range(0, 250)]
    public double? CircunferenciaCinturaCm { get; set; }

    [Range(0, 250)]
    public double? CircunferenciaAbdomenCm { get; set; }

    [Range(0, 250)]
    public double? CircunferenciaQuadrilCm { get; set; }

    [Range(0, 100)]
    public double? CircunferenciaBracoDireitoCm { get; set; }

    [Range(0, 100)]
    public double? CircunferenciaBracoEsquerdoCm { get; set; }

    [Range(0, 100)]
    public double? CircunferenciaAntebracoDireitoCm { get; set; }

    [Range(0, 100)]
    public double? CircunferenciaAntebracoEsquerdoCm { get; set; }

    [Range(0, 150)]
    public double? CircunferenciaCoxaDireitaCm { get; set; }

    [Range(0, 150)]
    public double? CircunferenciaCoxaEsquerdaCm { get; set; }

    [Range(0, 100)]
    public double? CircunferenciaPanturrilhaDireitaCm { get; set; }

    [Range(0, 100)]
    public double? CircunferenciaPanturrilhaEsquerdaCm { get; set; }

    // ===================== DOBRAS CUTÂNEAS (mm) =====================

    public EProtocoloDobrasCutaneas? ProtocoloDobrasCutaneas { get; set; }

    [Range(0, 100)]
    public double? DobraTricepsMm { get; set; }

    [Range(0, 100)]
    public double? DobraBicepsMm { get; set; }

    [Range(0, 100)]
    public double? DobraSubescapularMm { get; set; }

    [Range(0, 100)]
    public double? DobraSuprailiacaMm { get; set; }

    [Range(0, 100)]
    public double? DobraAbdominalMm { get; set; }

    [Range(0, 100)]
    public double? DobraCoxaMm { get; set; }

    [Range(0, 100)]
    public double? DobraPanturrilhaMm { get; set; }

    [Range(0, 100)]
    public double? DobraAxilarMediaMm { get; set; }

    [Range(0, 100)]
    public double? DobraPeitoralMm { get; set; }

    // ===================== BIOIMPEDÂNCIA =====================

    public bool PossuiBioimpedancia { get; set; }

    [Range(0, 80)]
    public decimal? BioPercentualGordura { get; set; }

    [Range(0, 200)]
    public double? BioMassaMagraKg { get; set; }

    [Range(0, 200)]
    public double? BioMassaGordaKg { get; set; }

    [Range(0, 100)]
    public double? BioAguaCorporalLitros { get; set; }

    [Range(0, 100)]
    public decimal? BioPercentualAgua { get; set; }

    [Range(0, 5000)]
    public double? BioTMBKcal { get; set; }

    [Range(0, 60)]
    public int? BioGorduraVisceralNivel { get; set; }

    [Range(0, 120)]
    public int? BioIdadeMetabolica { get; set; }

    [Range(0, 10)]
    public double? BioMassaOsseaKg { get; set; }

    // ===================== FOTOS =====================

    public List<FotoProgressoDto>? FotosProgresso { get; set; }

    // ===================== OBSERVAÇÕES =====================

    [MaxLength(2000)]
    public string? Observacoes { get; set; }
}
