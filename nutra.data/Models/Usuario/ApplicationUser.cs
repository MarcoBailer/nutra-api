using System.ComponentModel.DataAnnotations;

namespace Nutra.Models.Usuario;

/// <summary>
/// Projeção local do usuário autenticado pelo shark-lock.
/// Não gerencia senha nem login — apenas dados de domínio.
/// O Id corresponde ao "sub" claim do token JWT.
/// </summary>
public class ApplicationUser
{
    [MaxLength(128)]
    public string Id { get; set; } = string.Empty;
    [MaxLength(200)]
    public string NomeCompleto { get; set; } = string.Empty;

    [MaxLength(100)]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [MaxLength(14)] // 000.000.000-00
    public string CPF { get; set; } = string.Empty;

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

    [MaxLength(9)] // 00000-000
    public string? CEP { get; set; }

    // --- Auditoria ---
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
    public DateTime? AtualizadoEm { get; set; }
    public bool Ativo { get; set; } = true;

    // --- Navegação ---
    // TODO: Verificar - mudar para obrigatório, se o usuário existe um perfil tem que existir;
    // Se fizer dessa forma o perfil tem que existir antes da projeção local, no momento de criação da projeção local eu tenho que criar o perfil.
    public PerfilNutricional? PerfilAtivo { get; set; } 

    /// <summary>
    /// Perfil profissional (preenchido se o usuário é nutricionista).
    /// </summary>
    public PerfilProfissional? PerfilProfissional { get; set; }

    /// <summary>
    /// Vínculos do paciente com nutricionistas (quando for paciente).
    /// </summary>
    public ICollection<VinculoPacienteProfissional> VinculosComoPaciente { get; set; } = new List<VinculoPacienteProfissional>();
}
