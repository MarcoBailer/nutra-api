using Nutra.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nutra.Models.Usuario;

/// <summary>
/// Vínculo entre um paciente e um profissional nutricionista.
/// Permite que o nutricionista gerencie o perfil nutricional do paciente.
/// </summary>
public class VinculoPacienteProfissional
{
    public int Id { get; set; }

    // --- Paciente ---
    public string PacienteUserId { get; set; } = string.Empty;

    [ForeignKey("PacienteUserId")]
    public ApplicationUser Paciente { get; set; } = null!;

    // --- Profissional ---
    public int PerfilProfissionalId { get; set; }

    [ForeignKey("PerfilProfissionalId")]
    public PerfilProfissional Profissional { get; set; } = null!;

    /// <summary>Clínica de atendimento (opcional).</summary>
    public int? ClinicaId { get; set; }

    [ForeignKey("ClinicaId")]
    public Clinica? Clinica { get; set; }

    public EStatusVinculo Status { get; set; } = EStatusVinculo.Pendente;

    /// <summary>Data em que o convite foi enviado.</summary>
    public DateTime DataConvite { get; set; } = DateTime.UtcNow;

    /// <summary>Data em que o paciente aceitou o vínculo.</summary>
    public DateTime? DataAceite { get; set; }

    /// <summary>Data de encerramento do vínculo.</summary>
    public DateTime? DataEncerramento { get; set; }

    /// <summary>Observações internas do profissional sobre o paciente.</summary>
    [MaxLength(2000)]
    public string? Observacoes { get; set; }
}
