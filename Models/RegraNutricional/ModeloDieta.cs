using Nutra.Enum;
using Nutra.Models.Usuario;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nutra.Models.RegraNutricional;

/// <summary>
/// Modelo/Template de dieta pronta que pode ser reutilizado para criar planos alimentares.
/// Pode ser público (padrão do sistema) ou criado por um profissional.
/// </summary>
public class ModeloDieta
{
    public int Id { get; set; }

    [Required, MaxLength(200)]
    public string Nome { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string? Descricao { get; set; }

    public ETipoObjetivo ObjetivoAlvo { get; set; }
    public EPreferenciaAlimentar PreferenciaAlimentarAlvo { get; set; }

    /// <summary>
    /// Faixa calórica alvo do template (base para escalonamento).
    /// </summary>
    public double CaloriasBase { get; set; }
    public double ProteinaBaseG { get; set; }
    public double CarboidratoBaseG { get; set; }
    public double GorduraBaseG { get; set; }

    /// <summary>
    /// Número de refeições por dia neste modelo.
    /// </summary>
    public int NumeroRefeicoesDia { get; set; }

    /// <summary>
    /// Profissional que criou o modelo (null = template do sistema).
    /// </summary>
    public string? CriadoPorProfissionalId { get; set; }
    [ForeignKey("CriadoPorProfissionalId")]
    public ApplicationUser? CriadoPorProfissional { get; set; }

    /// <summary>
    /// Se true, qualquer profissional pode usar como base.
    /// </summary>
    public bool Publico { get; set; } = false;

    public bool Ativo { get; set; } = true;

    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
    public DateTime? AtualizadoEm { get; set; }

    // --- Navegação: Refeições do template ---
    public ICollection<RefeicaoModeloDieta> Refeicoes { get; set; } = new List<RefeicaoModeloDieta>();
}
