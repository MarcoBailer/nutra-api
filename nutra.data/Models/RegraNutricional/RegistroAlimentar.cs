using Nutra.Enum;
using Nutra.Models.Usuario;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nutra.Models.RegraNutricional;

public class RegistroAlimentar
{
    public long Id { get; set; }

    public string UserId { get; set; }
    [ForeignKey("UserId")]
    public ApplicationUser User { get; set; }

    public int AlimentoIdOrigem { get; set; }
    public string NomeAlimentoSnapshot { get; set; }
    public ETipoTabela TipoTabela { get; set; }

    public double QuantidadeConsumidaG { get; set; }
    public DateTime DataConsumo { get; set; } = DateTime.UtcNow;
    public ETipoRefeicao Refeicao { get; set; }

    public double EnergiaKcalTotal { get; set; }
    public double ProteinaTotal { get; set; }
    public double CarboTotal { get; set; }
    public double GorduraTotal { get; set; }
    public double FibraTotal { get; set; }
    public double AguaTotal { get; set; }

    public string DadosNutricionaisCompletosJson { get; set; }

    // --- Etapa 5: Diário Alimentar ---

    /// <summary>
    /// Referência opcional ao plano alimentar ativo (para comparação planejado vs consumido).
    /// </summary>
    public int? PlanoAlimentarId { get; set; }
    [ForeignKey("PlanoAlimentarId")]
    public PlanoAlimentar? PlanoAlimentar { get; set; }

    /// <summary>
    /// Referência ao item específico do plano que está sendo cumprido.
    /// </summary>
    public int? ItemRefeicaoPlanoId { get; set; }
    [ForeignKey("ItemRefeicaoPlanoId")]
    public ItemRefeicao? ItemRefeicaoPlano { get; set; }

    /// <summary>
    /// Código de barras do produto (leitura via scanner/câmera).
    /// </summary>
    [MaxLength(50)]
    public string? CodigoBarras { get; set; }

    /// <summary>
    /// Fotos associadas a este registro de consumo.
    /// </summary>
    public ICollection<FotoRefeicao> FotosRefeicao { get; set; } = new List<FotoRefeicao>();
}