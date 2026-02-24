using Nutra.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nutra.Models.Usuario;

/// <summary>
/// Clínica ou consultório vinculado a um profissional.
/// Um profissional Enterprise pode ter múltiplas clínicas.
/// </summary>
public class Clinica
{
    public int Id { get; set; }

    [Required, MaxLength(200)]
    public string Nome { get; set; } = string.Empty;

    [MaxLength(18)] // 00.000.000/0000-00
    public string? CNPJ { get; set; }

    [MaxLength(20)]
    public string? Telefone { get; set; }

    [MaxLength(200)]
    public string? Email { get; set; }

    [MaxLength(500)]
    public string? LogoUrl { get; set; }

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

    // --- Vínculo com profissional ---
    public int PerfilProfissionalId { get; set; }

    [ForeignKey("PerfilProfissionalId")]
    public PerfilProfissional PerfilProfissional { get; set; } = null!;

    // --- Auditoria ---
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
    public DateTime? AtualizadoEm { get; set; }
    public bool Ativo { get; set; } = true;
}
