using Nutra.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nutra.Models.RegraNutricional;

/// <summary>
/// Refeição dentro de um modelo/template de dieta.
/// </summary>
public class RefeicaoModeloDieta
{
    public int Id { get; set; }

    public int ModeloDietaId { get; set; }
    [ForeignKey("ModeloDietaId")]
    public ModeloDieta ModeloDieta { get; set; } = null!;

    public ETipoRefeicao TipoRefeicao { get; set; }
    public TimeSpan? HorarioSugerido { get; set; }
    public int Ordem { get; set; }

    [MaxLength(500)]
    public string? Observacoes { get; set; }

    /// <summary>
    /// Percentual calórico desta refeição em relação ao total diário (ex: 25 = 25%).
    /// </summary>
    public double PercentualCaloricoSugerido { get; set; }

    // --- Navegação ---
    public ICollection<ItemModeloDieta> Itens { get; set; } = new List<ItemModeloDieta>();
}
