using System.ComponentModel.DataAnnotations;

namespace Nutra.Models.Dtos;

/// <summary>
/// DTO para cadastro e atualização de clínica.
/// </summary>
public class ClinicaDto
{
    [Required(ErrorMessage = "Nome da clínica é obrigatório.")]
    [MaxLength(200)]
    public string Nome { get; set; } = string.Empty;

    [MaxLength(18)]
    public string? CNPJ { get; set; }

    [MaxLength(20)]
    public string? Telefone { get; set; }

    [MaxLength(200)]
    public string? Email { get; set; }

    [MaxLength(200)]
    public string? Logradouro { get; set; }

    [MaxLength(10)]
    public string? Numero { get; set; }

    [MaxLength(100)]
    public string? Complemento { get; set; }

    [MaxLength(100)]
    public string? Bairro { get; set; }

    [MaxLength(100)]
    public string? Cidade { get; set; }

    [MaxLength(2)]
    public string? Estado { get; set; }

    [MaxLength(9)]
    public string? CEP { get; set; }
}
