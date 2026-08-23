using Nutra.Enum;
using Nutra.Models.RegraNutricional;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nutra.Models.Usuario;

public class PerfilNutricional
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;

    [ForeignKey("UserId")]
    public ApplicationUser User { get; set; } = null!;

    public int? MetaNutricionalAtualId { get; set; }

    // TODO: Verificar - Transformar em obrigatório, deve ser criado junto do perfil. Usuário nasce -> perfil nasce -> meta nasce
    // Se for obrigatório a Meta precisa ser criado antes do perfil.
    [ForeignKey("MetaNutricionalAtualId")]
    public MetaNutricional? MetaNutricional { get; set; } 

    // --- Medidas corporais ---
    public double AlturaCm { get; set; }
    public double PesoAtualKg { get; set; }
    public double? PercentualGorduraCorporal { get; set; }
    public double? CircunferenciaCinturaCm { get; set; }
    public double? CircunferenciaQuadrilCm { get; set; }
    public double? CircunferenciaBracoCm { get; set; }

    // --- Dados pessoais (espelhados/derivados para cálculo) ---
    public DateTime DataNascimento { get; set; }
    public EGeneroBiologico Genero { get; set; }

    // --- Atividade e estilo de vida ---
    public double FatorAtividade { get; set; }
    public ENivelAtividadeFisica NivelAtividade { get; set; }

    [MaxLength(300)]
    public string OcupacaoProfissional { get; set; } = string.Empty;

    /// <summary>Habilidade culinária do paciente (impacta na complexidade das receitas sugeridas).</summary>
    public ENivelHabilidadeCulinaria HabilidadeCulinaria { get; set; } = ENivelHabilidadeCulinaria.Intermediario;

    /// <summary>Orçamento mensal estimado para alimentação.</summary>
    public EOrcamentoMensalEstimado OrcamentoMensal { get; set; } = EOrcamentoMensalEstimado.Medio;

    // --- Saúde ---
    public bool PossuiDoencasPreExistentes { get; set; }

    [MaxLength(2000)]
    public string DescricaoCondicoesMedicas { get; set; } = string.Empty;

    /// <summary>É fumante?</summary>
    public bool Fumante { get; set; }

    /// <summary>Qualidade do sono (1-5 escala).</summary>
    public int? QualidadeSono { get; set; }

    /// <summary>Horas de sono por noite (média).</summary>
    public double? HorasSonoPorNoite { get; set; }

    // --- Objetivos ---
    public ETipoObjetivo Objetivo { get; set; }
    public double? PesoDesejadoKg { get; set; }

    // --- Preferências alimentares ---
    public EPreferenciaAlimentar PreferenciaDieta { get; set; }
    public int RefeicoesPorDiaDesejadas { get; set; }
    public int TempoDisponivelPreparoMinutos { get; set; }

    // --- Auditoria ---
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
    public DateTime? AtualizadoEm { get; set; }

    // --- Navegação ---
    public ICollection<RestricaoAlimentar> RestricoesAlimentares { get; set; } = new List<RestricaoAlimentar>();
    public ICollection<PreferenciaAlimentar>? PreferenciasAlimentares { get; set; } = new List<PreferenciaAlimentar>();
    public ICollection<RegistroBiometrico> HistoricoMedidas { get; set; } = new List<RegistroBiometrico>();
    public ICollection<PerfilEquipamento> EquipamentoDisponivel { get; set; } = new List<PerfilEquipamento>();
    public ICollection<HistoricoClinico> HistoricosClinico { get; set; } = new List<HistoricoClinico>();
    public ICollection<AnamneseAlimentar> Anamneses { get; set; } = new List<AnamneseAlimentar>();
    public ICollection<AvaliacaoAntropometrica> AvaliacoesAntropometricas { get; set; } = new List<AvaliacaoAntropometrica>();
    public ICollection<PlanoAlimentar> PlanosAlimentares { get; set; } = new List<PlanoAlimentar>();
}
