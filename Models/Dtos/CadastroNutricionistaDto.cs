using System.ComponentModel.DataAnnotations;

namespace Nutra.Models.Dtos;

/// <summary>
/// DTO para cadastro de nutricionista (dados profissionais).
/// </summary>
public class CadastroNutricionistaDto
{
    [Required(ErrorMessage = "Nome completo é obrigatório.")]
    [MaxLength(200)]
    public string NomeCompleto { get; set; } = string.Empty;

    [Required(ErrorMessage = "E-mail é obrigatório.")]
    [EmailAddress(ErrorMessage = "E-mail inválido.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "CPF é obrigatório.")]
    [MaxLength(14)]
    public string CPF { get; set; } = string.Empty;

    [Required(ErrorMessage = "CRN é obrigatório.")]
    [MaxLength(20)]
    public string CRN { get; set; } = string.Empty;

    [Required(ErrorMessage = "Região do CRN é obrigatória.")]
    [Range(1, 11, ErrorMessage = "Região do CRN deve ser de 1 a 11.")]
    public int CRNRegiao { get; set; }

    [MaxLength(200)]
    public string? Especialidade { get; set; }

    [MaxLength(2000)]
    public string? BioProfissional { get; set; }

    public int? AnosExperiencia { get; set; }

    [MaxLength(20)]
    public string? Telefone { get; set; }
}
