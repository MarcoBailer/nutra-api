using Nutra.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nutra.Models.RegraNutricional;

public class RefeicaoPlano
{
    public int Id { get; set; }

    public int PlanoAlimentarId { get; set; }
    [ForeignKey("PlanoAlimentarId")]
    public PlanoAlimentar PlanoAlimentar { get; set; } = null!;

    public ETipoRefeicao TipoRefeicao { get; set; }

    /// <summary>
    /// Horário sugerido para a refeição (ex: 07:00, 12:30).
    /// </summary>
    public TimeSpan? HorarioSugerido { get; set; }

    /// <summary>
    /// Ordem de apresentação dentro do plano (1=primeira refeição do dia).
    /// </summary>
    public int Ordem { get; set; }

    [MaxLength(500)]
    public string? Observacoes { get; set; }

    // --- Totais calculados da refeição ---
    public double TotalEnergiaKcal { get; set; }
    public double TotalProteinaG { get; set; }
    public double TotalCarboidratoG { get; set; }
    public double TotalGorduraG { get; set; }
    public double TotalFibraG { get; set; }

    // --- Navegação ---
    public ICollection<ItemRefeicao> Itens { get; set; } = new List<ItemRefeicao>();
}
