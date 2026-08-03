using Nutra.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nutra.Models.Usuario;

/// <summary>
/// Assinatura ativa do profissional (controla limites e recursos).
/// </summary>
public class Assinatura
{
    public int Id { get; set; }

    public int PerfilProfissionalId { get; set; }

    [ForeignKey("PerfilProfissionalId")]
    public PerfilProfissional PerfilProfissional { get; set; } = null!;

    public EPlanoAssinatura Plano { get; set; } = EPlanoAssinatura.Gratuito;
    public EStatusAssinatura Status { get; set; } = EStatusAssinatura.Trial;

    public DateTime DataInicio { get; set; } = DateTime.UtcNow;
    public DateTime? DataExpiracao { get; set; }
    public DateTime? DataCancelamento { get; set; }

    /// <summary>Identificador externo do gateway de pagamento (Stripe, etc.).</summary>
    [MaxLength(200)]
    public string? GatewaySubscriptionId { get; set; }

    /// <summary>Valor mensal em BRL.</summary>
    [Column(TypeName = "decimal(10,2)")]
    public decimal ValorMensal { get; set; }

    /// <summary>Renovação automática habilitada.</summary>
    public bool RenovacaoAutomatica { get; set; } = true;

    // --- Auditoria ---
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
    public DateTime? AtualizadoEm { get; set; }
}
