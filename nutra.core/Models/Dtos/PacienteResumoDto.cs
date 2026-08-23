using Nutra.Enum;

namespace Nutra.Models.Dtos;

/// <summary>
/// DTO de retorno com dados resumidos de um paciente vinculado.
/// </summary>
public class PacienteResumoDto
{
    public string UserId { get; set; } = string.Empty;
    public string NomeCompleto { get; set; } = string.Empty;
    public string? Email { get; set; }
    public EStatusVinculo StatusVinculo { get; set; }
    public DateTime DataVinculo { get; set; }
    public string? Clinica { get; set; }
    public bool PerfilNutricionalCriado { get; set; }
}
