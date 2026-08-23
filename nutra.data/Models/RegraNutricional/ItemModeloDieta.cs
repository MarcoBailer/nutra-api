using Nutra.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nutra.Models.RegraNutricional;

/// <summary>
/// Item alimentar dentro de uma refeição de modelo/template de dieta.
/// </summary>
public class ItemModeloDieta
{
    public int Id { get; set; }

    public int RefeicaoModeloDietaId { get; set; }
    [ForeignKey("RefeicaoModeloDietaId")]
    public RefeicaoModeloDieta RefeicaoModeloDieta { get; set; } = null!;

    public int AlimentoId { get; set; }
    public ETipoTabela TipoTabela { get; set; }

    [Required, MaxLength(300)]
    public string NomeAlimentoSnapshot { get; set; } = string.Empty;

    public double QuantidadeG { get; set; }

    public double EnergiaKcal { get; set; }
    public double ProteinaG { get; set; }
    public double CarboidratoG { get; set; }
    public double GorduraG { get; set; }
    public double FibraG { get; set; }

    public int Ordem { get; set; }
}
