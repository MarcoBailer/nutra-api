using Nutra.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nutra.Models.RegraNutricional;

/// <summary>
/// Foto de progresso associada a uma avaliação antropométrica.
/// Permite acompanhamento visual da evolução corporal.
/// </summary>
public class FotoProgresso
{
    public int Id { get; set; }

    public int AvaliacaoAntropometricaId { get; set; }

    [ForeignKey("AvaliacaoAntropometricaId")]
    public AvaliacaoAntropometrica AvaliacaoAntropometrica { get; set; } = null!;

    /// <summary>URL da imagem armazenada (storage externo).</summary>
    [Required, MaxLength(1000)]
    public string Url { get; set; } = string.Empty;

    /// <summary>Ângulo/tipo da foto.</summary>
    public ETipoFotoProgresso Tipo { get; set; }

    /// <summary>Data em que a foto foi tirada.</summary>
    public DateTime DataFoto { get; set; } = DateTime.UtcNow;

    [MaxLength(500)]
    public string? Descricao { get; set; }

    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
}
