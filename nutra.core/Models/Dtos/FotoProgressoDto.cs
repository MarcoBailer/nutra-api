using Nutra.Enum;
using System.ComponentModel.DataAnnotations;

namespace Nutra.Models.Dtos;

/// <summary>
/// DTO para upload de foto de progresso corporal.
/// </summary>
public class FotoProgressoDto
{
    [Required(ErrorMessage = "URL da foto é obrigatória.")]
    [MaxLength(1000)]
    public string Url { get; set; } = string.Empty;

    [Required]
    public ETipoFotoProgresso Tipo { get; set; }

    [MaxLength(500)]
    public string? Descricao { get; set; }

    public DateTime? DataFoto { get; set; }
}
