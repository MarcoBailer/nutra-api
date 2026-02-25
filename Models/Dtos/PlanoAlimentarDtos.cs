using Nutra.Enum;

namespace Nutra.Models.Dtos;

// ============================================================
// DTOs de entrada (criação/atualização de plano alimentar)
// ============================================================

public class CriarPlanoAlimentarDto
{
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public DateTime DataInicio { get; set; }
    public DateTime? DataFim { get; set; }
    public string? Observacoes { get; set; }

    /// <summary>
    /// Se informado, o plano será criado a partir deste modelo de dieta.
    /// As metas e refeições serão pré-populadas.
    /// </summary>
    public int? ModeloDietaOrigemId { get; set; }

    /// <summary>
    /// Metas personalizadas (usadas quando não se baseia em modelo).
    /// Se omitidas, auto-calcula a partir do perfil + MetaNutricional.
    /// </summary>
    public double? CaloriasAlvoDiarias { get; set; }
    public double? ProteinaAlvoG { get; set; }
    public double? CarboidratoAlvoG { get; set; }
    public double? GorduraAlvoG { get; set; }
    public double? FibraAlvoG { get; set; }
    public double? AguaAlvoL { get; set; }

    public List<RefeicaoPlanoDto>? Refeicoes { get; set; }
}

/// <summary>
/// DTO para criar plano alimentar como profissional para um paciente.
/// </summary>
public class CriarPlanoProfissionalDto : CriarPlanoAlimentarDto
{
    public string PacienteUserId { get; set; } = string.Empty;
}

public class AtualizarPlanoAlimentarDto
{
    public string? Nome { get; set; }
    public string? Descricao { get; set; }
    public DateTime? DataFim { get; set; }
    public EStatusPlano? Status { get; set; }
    public string? Observacoes { get; set; }
}

// ============================================================
// DTOs de refeições e itens
// ============================================================

public class RefeicaoPlanoDto
{
    public ETipoRefeicao TipoRefeicao { get; set; }
    public TimeSpan? HorarioSugerido { get; set; }
    public int Ordem { get; set; }
    public string? Observacoes { get; set; }
    public List<ItemRefeicaoDto> Itens { get; set; } = new();
}

public class ItemRefeicaoDto
{
    public int AlimentoId { get; set; }
    public ETipoTabela TipoTabela { get; set; }
    public double QuantidadeG { get; set; }
    public int Ordem { get; set; }
    public string? Observacoes { get; set; }
    public List<SubstituicaoDto>? Substituicoes { get; set; }
}

public class SubstituicaoDto
{
    public int AlimentoId { get; set; }
    public ETipoTabela TipoTabela { get; set; }
    public double QuantidadeG { get; set; }
}

public class AdicionarRefeicaoDto
{
    public ETipoRefeicao TipoRefeicao { get; set; }
    public TimeSpan? HorarioSugerido { get; set; }
    public int Ordem { get; set; }
    public string? Observacoes { get; set; }
    public List<ItemRefeicaoDto> Itens { get; set; } = new();
}

public class AdicionarItemDto
{
    public int AlimentoId { get; set; }
    public ETipoTabela TipoTabela { get; set; }
    public double QuantidadeG { get; set; }
    public int Ordem { get; set; }
    public string? Observacoes { get; set; }
}

public class AdicionarSubstituicaoDto
{
    public int AlimentoId { get; set; }
    public ETipoTabela TipoTabela { get; set; }
    public double QuantidadeG { get; set; }
}

// ============================================================
// DTOs de resposta (saída)
// ============================================================

public class PlanoAlimentarResultadoDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public DateTime DataInicio { get; set; }
    public DateTime? DataFim { get; set; }
    public EStatusPlano Status { get; set; }
    public string? Observacoes { get; set; }

    // Metas diárias
    public MacrosDiariosPlanoDto MetasDiarias { get; set; } = new();

    // Totais somados de todas as refeições
    public MacrosDiariosPlanoDto TotaisCalculados { get; set; } = new();

    // Diferença (meta - calculado)
    public MacrosDiariosPlanoDto DiferencaMetas { get; set; } = new();

    public string? ProfissionalResponsavel { get; set; }
    public string? ModeloDietaOrigem { get; set; }

    public List<RefeicaoPlanoResultadoDto> Refeicoes { get; set; } = new();

    public DateTime CriadoEm { get; set; }
    public DateTime? AtualizadoEm { get; set; }
}

public class MacrosDiariosPlanoDto
{
    public double CaloriasKcal { get; set; }
    public double ProteinaG { get; set; }
    public double CarboidratoG { get; set; }
    public double GorduraG { get; set; }
    public double FibraG { get; set; }
    public double AguaL { get; set; }
}

public class RefeicaoPlanoResultadoDto
{
    public int Id { get; set; }
    public ETipoRefeicao TipoRefeicao { get; set; }
    public TimeSpan? HorarioSugerido { get; set; }
    public int Ordem { get; set; }
    public string? Observacoes { get; set; }

    // Totais da refeição
    public double TotalEnergiaKcal { get; set; }
    public double TotalProteinaG { get; set; }
    public double TotalCarboidratoG { get; set; }
    public double TotalGorduraG { get; set; }
    public double TotalFibraG { get; set; }

    /// <summary>
    /// Percentual que esta refeição representa do total calórico diário.
    /// </summary>
    public double PercentualCaloricoRefeicao { get; set; }

    public List<ItemRefeicaoResultadoDto> Itens { get; set; } = new();
}

public class ItemRefeicaoResultadoDto
{
    public int Id { get; set; }
    public int AlimentoId { get; set; }
    public ETipoTabela TipoTabela { get; set; }
    public string NomeAlimento { get; set; } = string.Empty;
    public double QuantidadeG { get; set; }
    public int Ordem { get; set; }
    public string? Observacoes { get; set; }

    public double EnergiaKcal { get; set; }
    public double ProteinaG { get; set; }
    public double CarboidratoG { get; set; }
    public double GorduraG { get; set; }
    public double FibraG { get; set; }

    public List<SubstituicaoResultadoDto> Substituicoes { get; set; } = new();
}

public class SubstituicaoResultadoDto
{
    public int Id { get; set; }
    public int AlimentoId { get; set; }
    public ETipoTabela TipoTabela { get; set; }
    public string NomeAlimento { get; set; } = string.Empty;
    public double QuantidadeG { get; set; }
    public double EnergiaKcal { get; set; }
    public double ProteinaG { get; set; }
    public double CarboidratoG { get; set; }
    public double GorduraG { get; set; }
    public double FibraG { get; set; }
}

public class PlanoAlimentarResumoDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public EStatusPlano Status { get; set; }
    public DateTime DataInicio { get; set; }
    public DateTime? DataFim { get; set; }
    public double CaloriasAlvoDiarias { get; set; }
    public int TotalRefeicoes { get; set; }
    public int TotalItens { get; set; }
    public string? ProfissionalResponsavel { get; set; }
    public DateTime CriadoEm { get; set; }
}

// ============================================================
// DTOs de Modelo/Template de Dieta
// ============================================================

public class CriarModeloDietaDto
{
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public ETipoObjetivo ObjetivoAlvo { get; set; }
    public EPreferenciaAlimentar PreferenciaAlimentarAlvo { get; set; }
    public int NumeroRefeicoesDia { get; set; }
    public bool Publico { get; set; }
    public List<RefeicaoModeloDietaDto> Refeicoes { get; set; } = new();
}

public class RefeicaoModeloDietaDto
{
    public ETipoRefeicao TipoRefeicao { get; set; }
    public TimeSpan? HorarioSugerido { get; set; }
    public int Ordem { get; set; }
    public double PercentualCaloricoSugerido { get; set; }
    public string? Observacoes { get; set; }
    public List<ItemRefeicaoDto> Itens { get; set; } = new();
}

public class ModeloDietaResultadoDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public ETipoObjetivo ObjetivoAlvo { get; set; }
    public EPreferenciaAlimentar PreferenciaAlimentarAlvo { get; set; }
    public double CaloriasBase { get; set; }
    public double ProteinaBaseG { get; set; }
    public double CarboidratoBaseG { get; set; }
    public double GorduraBaseG { get; set; }
    public int NumeroRefeicoesDia { get; set; }
    public bool Publico { get; set; }
    public string? CriadoPorProfissional { get; set; }
    public List<RefeicaoPlanoResultadoDto> Refeicoes { get; set; } = new();
}

public class ModeloDietaResumoDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public ETipoObjetivo ObjetivoAlvo { get; set; }
    public EPreferenciaAlimentar PreferenciaAlimentarAlvo { get; set; }
    public double CaloriasBase { get; set; }
    public int NumeroRefeicoesDia { get; set; }
    public bool Publico { get; set; }
}
