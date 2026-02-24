using Nutra.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nutra.Models.RegraNutricional;

public class ItemRefeicao
{
    public int Id { get; set; }

    public int RefeicaoPlanoId { get; set; }
    [ForeignKey("RefeicaoPlanoId")]
    public RefeicaoPlano RefeicaoPlano { get; set; } = null!;

    /// <summary>
    /// ID do alimento na tabela de origem.
    /// </summary>
    public int AlimentoId { get; set; }

    public ETipoTabela TipoTabela { get; set; }

    [Required, MaxLength(300)]
    public string NomeAlimentoSnapshot { get; set; } = string.Empty;

    /// <summary>
    /// Quantidade planejada em gramas.
    /// </summary>
    public double QuantidadeG { get; set; }

    // --- Macros calculados para a quantidade ---
    public double EnergiaKcal { get; set; }
    public double ProteinaG { get; set; }
    public double CarboidratoG { get; set; }
    public double GorduraG { get; set; }
    public double FibraG { get; set; }

    /// <summary>
    /// Ordem de apresentação do item dentro da refeição.
    /// </summary>
    public int Ordem { get; set; }

    [MaxLength(500)]
    public string? Observacoes { get; set; }

    // --- Navegação ---
    public ICollection<SubstituicaoEquivalente> SubstituicoesEquivalentes { get; set; } = new List<SubstituicaoEquivalente>();
}
