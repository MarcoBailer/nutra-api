using Nutra.Enum;
using System.ComponentModel.DataAnnotations;

namespace Nutra.Models.Dtos;

/// <summary>
/// DTO para registro de condição no histórico clínico.
/// </summary>
public class HistoricoClinicoDto
{
    [Required]
    public ECondicaoClinica Condicao { get; set; }

    [MaxLength(500)]
    public string? DescricaoOutra { get; set; }

    public DateTime? DataDiagnostico { get; set; }

    public bool AtivaAtualmente { get; set; } = true;

    [MaxLength(1000)]
    public string? MedicamentosEmUso { get; set; }

    [MaxLength(2000)]
    public string? Observacoes { get; set; }
}
