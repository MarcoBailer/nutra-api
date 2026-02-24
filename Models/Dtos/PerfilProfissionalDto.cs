using Nutra.Enum;

namespace Nutra.Models.Dtos;

/// <summary>
/// DTO de retorno com informações do perfil profissional.
/// </summary>
public class PerfilProfissionalDto
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string NomeCompleto { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string CRN { get; set; } = string.Empty;
    public int CRNRegiao { get; set; }
    public bool CRNVerificado { get; set; }
    public string? Especialidade { get; set; }
    public string? BioProfissional { get; set; }
    public int? AnosExperiencia { get; set; }
    public EPlanoAssinatura PlanoAtual { get; set; }
    public EStatusAssinatura StatusAssinatura { get; set; }
    public int TotalPacientesAtivos { get; set; }
    public int MaxPacientes { get; set; }
    public bool MultiClinicaHabilitado { get; set; }
    public int TotalClinicas { get; set; }
    public DateTime CriadoEm { get; set; }
}
