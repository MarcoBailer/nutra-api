using Nutra.Enum;
using Nutra.Models.Usuario;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nutra.Models.RegraNutricional;

/// <summary>
/// Condição clínica registrada no histórico do paciente.
/// Cada registro representa uma condição pré-existente.
/// </summary>
public class HistoricoClinico
{
    public int Id { get; set; }

    public int PerfilNutricionalId { get; set; }

    [ForeignKey("PerfilNutricionalId")]
    public PerfilNutricional PerfilNutricional { get; set; } = null!;

    /// <summary>Condição clínica (enum).</summary>
    public ECondicaoClinica Condicao { get; set; }

    /// <summary>Descrição livre caso a condição seja "Outro".</summary>
    [MaxLength(500)]
    public string? DescricaoOutra { get; set; }

    /// <summary>Data do diagnóstico (aproximada).</summary>
    public DateTime? DataDiagnostico { get; set; }

    /// <summary>Se a condição está atualmente ativa/controlada.</summary>
    public bool AtivaAtualmente { get; set; } = true;

    /// <summary>Medicamentos em uso para esta condição.</summary>
    [MaxLength(1000)]
    public string? MedicamentosEmUso { get; set; }

    /// <summary>Observações adicionais do profissional.</summary>
    [MaxLength(2000)]
    public string? Observacoes { get; set; }

    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
    public DateTime? AtualizadoEm { get; set; }
}
