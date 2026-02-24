using System.ComponentModel.DataAnnotations;

namespace Nutra.Models.Dtos;

/// <summary>
/// DTO para atualização de dados pessoais do usuário (paciente ou profissional).
/// </summary>
public class UpdateProfileDto
{
    [MaxLength(200)]
    public string? NomeCompleto { get; set; }

    [MaxLength(14)]
    public string? Cpf { get; set; }

    public DateTime? DataNascimento { get; set; }

    [MaxLength(20)]
    public string? Telefone { get; set; }

    [MaxLength(500)]
    public string? FotoPerfilUrl { get; set; }

    // --- Endereço ---
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
