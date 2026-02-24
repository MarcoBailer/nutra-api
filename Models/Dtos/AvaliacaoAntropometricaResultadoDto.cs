using Nutra.Enum;

namespace Nutra.Models.Dtos;

/// <summary>
/// DTO de retorno completo com todos os dados da avaliação e cálculos realizados.
/// </summary>
public class AvaliacaoAntropometricaResultadoDto
{
    public int Id { get; set; }
    public DateTime DataAvaliacao { get; set; }
    public string? ProfissionalResponsavel { get; set; }
    public string? Observacoes { get; set; }

    // ===================== MEDIDAS BÁSICAS =====================
    public double PesoKg { get; set; }
    public double AlturaCm { get; set; }
    public decimal IMC { get; set; }
    public string ClassificacaoIMC { get; set; } = string.Empty;

    // ===================== CIRCUNFERÊNCIAS =====================
    public CircunferenciasDto Circunferencias { get; set; } = new();

    // ===================== DOBRAS CUTÂNEAS =====================
    public DobrasCutaneasResultadoDto? DobrasCutaneas { get; set; }

    // ===================== BIOIMPEDÂNCIA =====================
    public BioimpedanciaResultadoDto? Bioimpedancia { get; set; }

    // ===================== CÁLCULOS AUTOMÁTICOS =====================
    public CalculosMetabolicosDto Calculos { get; set; } = new();

    // ===================== COMPOSIÇÃO CORPORAL =====================
    public ComposicaoCorporalDto ComposicaoCorporal { get; set; } = new();

    // ===================== MACROS RECOMENDADOS =====================
    public MacronutrientesRecomendadosDto MacrosRecomendados { get; set; } = new();

    // ===================== FOTOS =====================
    public List<FotoProgressoDto> FotosProgresso { get; set; } = new();
}

/// <summary>
/// Agrupamento de todas as circunferências medidas.
/// </summary>
public class CircunferenciasDto
{
    public double? PescocoCm { get; set; }
    public double? ToraxCm { get; set; }
    public double? CinturaCm { get; set; }
    public double? AbdomenCm { get; set; }
    public double? QuadrilCm { get; set; }
    public double? BracoDireitoCm { get; set; }
    public double? BracoEsquerdoCm { get; set; }
    public double? AntebracoDireitoCm { get; set; }
    public double? AntebracoEsquerdoCm { get; set; }
    public double? CoxaDireitaCm { get; set; }
    public double? CoxaEsquerdaCm { get; set; }
    public double? PanturrilhaDireitaCm { get; set; }
    public double? PanturrilhaEsquerdaCm { get; set; }
    public decimal? RCQ { get; set; }
    public string? ClassificacaoRCQ { get; set; }
}

/// <summary>
/// Resultado da medição de dobras cutâneas.
/// </summary>
public class DobrasCutaneasResultadoDto
{
    public EProtocoloDobrasCutaneas Protocolo { get; set; }
    public double? TricepsMm { get; set; }
    public double? BicepsMm { get; set; }
    public double? SubescapularMm { get; set; }
    public double? SuprailiacaMm { get; set; }
    public double? AbdominalMm { get; set; }
    public double? CoxaMm { get; set; }
    public double? PanturrilhaMm { get; set; }
    public double? AxilarMediaMm { get; set; }
    public double? PeitoralMm { get; set; }
    public double SomatorioDobras { get; set; }
    public decimal DensidadeCorporal { get; set; }
    public decimal PercentualGorduraEstimado { get; set; }
}

/// <summary>
/// Dados de bioimpedância.
/// </summary>
public class BioimpedanciaResultadoDto
{
    public decimal PercentualGordura { get; set; }
    public double MassaMagraKg { get; set; }
    public double MassaGordaKg { get; set; }
    public double? AguaCorporalLitros { get; set; }
    public decimal? PercentualAgua { get; set; }
    public double? TMBKcal { get; set; }
    public int? GorduraVisceralNivel { get; set; }
    public int? IdadeMetabolica { get; set; }
    public double? MassaOsseaKg { get; set; }
}

/// <summary>
/// Cálculos metabólicos (TMB, GET).
/// </summary>
public class CalculosMetabolicosDto
{
    /// <summary>TMB por Mifflin-St Jeor (kcal/dia).</summary>
    public double TMBMifflinStJeor { get; set; }

    /// <summary>TMB por Harris-Benedict (kcal/dia).</summary>
    public double TMBHarrisBenedict { get; set; }

    /// <summary>TMB por Katch-McArdle (kcal/dia) — só calculado se houver massa magra.</summary>
    public double? TMBKatchMcArdle { get; set; }

    /// <summary>Gasto Energético Total (kcal/dia).</summary>
    public double GET { get; set; }

    /// <summary>Taxa metabólica ajustada ao objetivo (kcal/dia).</summary>
    public double TaxaMetabolicaAjustada { get; set; }
}

/// <summary>
/// Composição corporal estimada.
/// </summary>
public class ComposicaoCorporalDto
{
    /// <summary>Percentual de gordura estimado (melhor valor disponível).</summary>
    public decimal? PercentualGordura { get; set; }

    /// <summary>Fonte do percentual de gordura.</summary>
    public string? FontePercentualGordura { get; set; }

    /// <summary>Massa magra estimada em kg.</summary>
    public double? MassaMagraKg { get; set; }

    /// <summary>Massa gorda estimada em kg.</summary>
    public double? MassaGordaKg { get; set; }

    /// <summary>Peso ideal por Devine (kg).</summary>
    public double PesoIdealDevineKg { get; set; }

    /// <summary>Peso ideal por IMC 22 (kg).</summary>
    public double PesoIdealIMCKg { get; set; }

    /// <summary>Diferença entre peso atual e peso ideal (kg). Positivo = acima do ideal.</summary>
    public double DiferencaPesoIdealKg { get; set; }
}

/// <summary>
/// Macronutrientes recomendados com base no GET ajustado.
/// </summary>
public class MacronutrientesRecomendadosDto
{
    public double CaloriasAlvo { get; set; }
    public double ProteinaG { get; set; }
    public double CarboidratoG { get; set; }
    public double GorduraG { get; set; }
    public double FibraG { get; set; }
    public double AguaLitros { get; set; }

    /// <summary>Distribuição percentual estimada dos macros (informativo).</summary>
    public double PercentualProteina { get; set; }
    public double PercentualCarboidrato { get; set; }
    public double PercentualGordura { get; set; }
}

/// <summary>
/// DTO resumido para listagem de avaliações.
/// </summary>
public class AvaliacaoResumoDto
{
    public int Id { get; set; }
    public DateTime DataAvaliacao { get; set; }
    public double PesoKg { get; set; }
    public decimal IMC { get; set; }
    public string ClassificacaoIMC { get; set; } = string.Empty;
    public decimal? PercentualGordura { get; set; }
    public double? GET { get; set; }
    public bool PossuiBioimpedancia { get; set; }
    public bool PossuiDobrasCutaneas { get; set; }
    public int TotalFotos { get; set; }
}

/// <summary>
/// DTO para comparação entre duas avaliações (evolução).
/// </summary>
public class ComparacaoAvaliacoesDto
{
    public AvaliacaoAntropometricaResultadoDto AvaliacaoAnterior { get; set; } = null!;
    public AvaliacaoAntropometricaResultadoDto AvaliacaoAtual { get; set; } = null!;
    public EvolucaoDto Evolucao { get; set; } = new();
}

/// <summary>
/// Diferenças entre duas avaliações.
/// </summary>
public class EvolucaoDto
{
    public double DeltaPesoKg { get; set; }
    public decimal DeltaIMC { get; set; }
    public decimal? DeltaPercentualGordura { get; set; }
    public double? DeltaMassaMagraKg { get; set; }
    public double? DeltaMassaGordaKg { get; set; }
    public double? DeltaGET { get; set; }
    public double? DeltaCinturaCm { get; set; }
    public double? DeltaQuadrilCm { get; set; }
    public int DiasEntreAvaliacoes { get; set; }
}
