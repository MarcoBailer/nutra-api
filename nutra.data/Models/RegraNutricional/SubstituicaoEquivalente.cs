using Nutra.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nutra.Models.RegraNutricional;

/// <summary>
/// Substituição equivalente para um item de refeição do plano alimentar.
/// Permite que o usuário troque um alimento por outro de valor nutricional semelhante.
/// </summary>
public class SubstituicaoEquivalente
{
    public int Id { get; set; }

    public int ItemRefeicaoId { get; set; }
    [ForeignKey("ItemRefeicaoId")]
    public ItemRefeicao ItemRefeicao { get; set; } = null!;

    public int AlimentoId { get; set; }
    public ETipoTabela TipoTabela { get; set; }

    [Required, MaxLength(300)]
    public string NomeAlimento { get; set; } = string.Empty;

    /// <summary>
    /// Quantidade equivalente em gramas para atingir macros similares.
    /// </summary>
    public double QuantidadeG { get; set; }

    // --- Macros da substituição ---
    public double EnergiaKcal { get; set; }
    public double ProteinaG { get; set; }
    public double CarboidratoG { get; set; }
    public double GorduraG { get; set; }
    public double FibraG { get; set; }
}
