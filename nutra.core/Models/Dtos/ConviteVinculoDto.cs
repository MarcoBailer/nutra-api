using Nutra.Enum;
using System.ComponentModel.DataAnnotations;

namespace Nutra.Models.Dtos;

/// <summary>
/// DTO para envio de convite de vínculo nutricionista-paciente.
/// </summary>
public class ConviteVinculoDto
{
    /// <summary>E-mail do paciente que será vinculado.</summary>
    [Required(ErrorMessage = "E-mail do paciente é obrigatório.")]
    [EmailAddress]
    public string EmailPaciente { get; set; } = string.Empty;

    /// <summary>ID da clínica (opcional).</summary>
    public int? ClinicaId { get; set; }

    /// <summary>Observações iniciais sobre o paciente.</summary>
    [MaxLength(2000)]
    public string? Observacoes { get; set; }
}
