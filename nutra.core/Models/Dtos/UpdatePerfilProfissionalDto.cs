using System.ComponentModel.DataAnnotations;

namespace Nutra.Models.Dtos;

/// <summary>
/// DTO para atualização do perfil profissional do nutricionista.
/// </summary>
public class UpdatePerfilProfissionalDto
{
    [MaxLength(200)]
    public string? Especialidade { get; set; }

    [MaxLength(2000)]
    public string? BioProfissional { get; set; }

    public int? AnosExperiencia { get; set; }

    [MaxLength(20)]
    public string? Telefone { get; set; }

    [MaxLength(200)]
    public string? NomeCompleto { get; set; }
}
