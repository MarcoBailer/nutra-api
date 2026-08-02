using Nutra.Enum;
using Nutra.Models.Usuario;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nutra.Models.RegraNutricional;

/// <summary>
/// Foto tirada de uma refeição no diário alimentar.
/// Permite registro visual do que foi consumido.
/// </summary>
public class FotoRefeicao
{
    public int Id { get; set; }

    public string UserId { get; set; } = string.Empty;
    [ForeignKey("UserId")]
    public ApplicationUser User { get; set; } = null!;

    public DateTime DataRegistro { get; set; } = DateTime.UtcNow;

    public ETipoRefeicao TipoRefeicao { get; set; }

    [Required, MaxLength(1000)]
    public string FotoUrl { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Descricao { get; set; }

    /// <summary>
    /// Referência opcional ao registro alimentar associado.
    /// </summary>
    public long? RegistroAlimentarId { get; set; }
    [ForeignKey("RegistroAlimentarId")]
    public RegistroAlimentar? RegistroAlimentar { get; set; }

    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
}
