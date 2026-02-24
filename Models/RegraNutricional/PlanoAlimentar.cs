using Nutra.Enum;
using Nutra.Models.Usuario;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nutra.Models.RegraNutricional;

public class PlanoAlimentar
{
    public int Id { get; set; }

    public int PerfilNutricionalId { get; set; }
    [ForeignKey("PerfilNutricionalId")]
    public PerfilNutricional PerfilNutricional { get; set; } = null!;

    public string? ProfissionalResponsavelId { get; set; }
    [ForeignKey("ProfissionalResponsavelId")]
    public ApplicationUser? ProfissionalResponsavel { get; set; }

    [Required, MaxLength(200)]
    public string Nome { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string? Descricao { get; set; }

    public DateTime DataInicio { get; set; }
    public DateTime? DataFim { get; set; }

    public EStatusPlano Status { get; set; } = EStatusPlano.Rascunho;

    // --- Metas diárias do plano ---
    public double CaloriasAlvoDiarias { get; set; }
    public double ProteinaAlvoG { get; set; }
    public double CarboidratoAlvoG { get; set; }
    public double GorduraAlvoG { get; set; }
    public double FibraAlvoG { get; set; }
    public double AguaAlvoL { get; set; }

    // --- Referência a modelo de dieta (se baseado em template) ---
    public int? ModeloDietaOrigemId { get; set; }
    [ForeignKey("ModeloDietaOrigemId")]
    public ModeloDieta? ModeloDietaOrigem { get; set; }

    [MaxLength(2000)]
    public string? Observacoes { get; set; }

    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
    public DateTime? AtualizadoEm { get; set; }

    // --- Navegação ---
    public ICollection<RefeicaoPlano> RefeicoesPlanejadas { get; set; } = new List<RefeicaoPlano>();
}
