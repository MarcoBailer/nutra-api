using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nutra.Models.Usuario;

/// <summary>
/// Perfil profissional do nutricionista.
/// Vinculado 1:1 com ApplicationUser quando Role == Nutricionista.
/// </summary>
public class PerfilProfissional
{
    public int Id { get; set; }

    // --- Vínculo com Identity ---
    public string UserId { get; set; } = string.Empty;

    [ForeignKey("UserId")]
    public ApplicationUser User { get; set; } = null!;

    // --- Dados Profissionais ---

    /// <summary>CRN - Conselho Regional de Nutricionistas (ex: "CRN-3 12345").</summary>
    [Required, MaxLength(20)]
    public string CRN { get; set; } = string.Empty;

    /// <summary>Região/Estado do CRN (1 a 11).</summary>
    public int CRNRegiao { get; set; }

    /// <summary>Especialidade principal (ex: "Nutrição Esportiva", "Nutrição Clínica").</summary>
    [MaxLength(200)]
    public string? Especialidade { get; set; }

    /// <summary>Breve biografia profissional.</summary>
    [MaxLength(2000)]
    public string? BioProfissional { get; set; }

    /// <summary>Anos de experiência.</summary>
    public int? AnosExperiencia { get; set; }

    /// <summary>URL do diploma ou certificado verificado.</summary>
    [MaxLength(500)]
    public string? UrlDiploma { get; set; }

    /// <summary>CRN verificado pelo sistema.</summary>
    public bool CRNVerificado { get; set; } = false;

    /// <summary>Data de verificação do CRN.</summary>
    public DateTime? DataVerificacaoCRN { get; set; }

    // --- Limites do plano ---

    /// <summary>Máximo de pacientes simultâneos (definido pelo plano).</summary>
    public int MaxPacientes { get; set; } = 5;

    /// <summary>Se pode gerenciar múltiplas clínicas (Enterprise).</summary>
    public bool MultiClinicaHabilitado { get; set; } = false;

    // --- Auditoria ---
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
    public DateTime? AtualizadoEm { get; set; }
    public bool Ativo { get; set; } = true;

    // --- Navegação ---
    public Assinatura? AssinaturaAtiva { get; set; }
    public ICollection<Clinica> Clinicas { get; set; } = new List<Clinica>();
    public ICollection<VinculoPacienteProfissional> Pacientes { get; set; } = new List<VinculoPacienteProfissional>();
}
