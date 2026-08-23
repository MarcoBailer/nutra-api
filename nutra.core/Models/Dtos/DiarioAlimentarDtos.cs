using Nutra.Enum;

namespace Nutra.Models.Dtos;

// ============================================================
// DTOs de entrada (Diário Alimentar)
// ============================================================

public class RegistroConsumoDto
{
    public int AlimentoId { get; set; }
    public ETipoTabela TipoTabela { get; set; }
    public double QuantidadeConsumidaG { get; set; }
    public ETipoRefeicao TipoRefeicao { get; set; }
    public DateTime DataConsumo { get; set; }

    /// <summary>
    /// Item do plano alimentar que este registro cumpre (opcional).
    /// </summary>
    public int? ItemRefeicaoPlanoId { get; set; }

    /// <summary>
    /// Código de barras do produto (leitura via scanner/câmera).
    /// </summary>
    public string? CodigoBarras { get; set; }
}

public class RegistroConsumoLoteDto
{
    public List<RegistroConsumoDto> Itens { get; set; } = new();
}

public class FotoRefeicaoDto
{
    public ETipoRefeicao TipoRefeicao { get; set; }
    public string FotoUrl { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public long? RegistroAlimentarId { get; set; }
}

// ============================================================
// DTOs de resposta (Diário Alimentar)
// ============================================================

public class DiarioDiaDto
{
    public DateTime Data { get; set; }

    /// <summary>
    /// Metas do dia (vindo do plano ativo ou MetaNutricional).
    /// </summary>
    public MacrosDiariosPlanoDto MetasDoDia { get; set; } = new();

    /// <summary>
    /// Total consumido no dia.
    /// </summary>
    public MacrosDiariosPlanoDto TotalConsumido { get; set; } = new();

    /// <summary>
    /// Saldo restante (meta - consumido).
    /// </summary>
    public MacrosDiariosPlanoDto SaldoRestante { get; set; } = new();

    /// <summary>
    /// Percentual de aderência ao plano (0-100+).
    /// </summary>
    public double PercentualAderenciaCaloricas { get; set; }

    /// <summary>
    /// Refeições do dia com detalhes de planejado vs consumido.
    /// </summary>
    public List<RefeicaoDiarioDto> Refeicoes { get; set; } = new();

    /// <summary>
    /// Fotos registradas no dia.
    /// </summary>
    public List<FotoRefeicaoResultadoDto> Fotos { get; set; } = new();
}

public class RefeicaoDiarioDto
{
    public ETipoRefeicao TipoRefeicao { get; set; }
    public TimeSpan? HorarioPlanejado { get; set; }

    // Planejado (do plano alimentar ativo)
    public MacroRefeicaoDto? Planejado { get; set; }

    // Consumido (registros alimentares reais)
    public MacroRefeicaoDto Consumido { get; set; } = new();

    /// <summary>
    /// Percentual de aderência calórica desta refeição.
    /// </summary>
    public double? PercentualAderencia { get; set; }

    public List<RegistroConsumoResultadoDto> Registros { get; set; } = new();
}

public class MacroRefeicaoDto
{
    public double EnergiaKcal { get; set; }
    public double ProteinaG { get; set; }
    public double CarboidratoG { get; set; }
    public double GorduraG { get; set; }
    public double FibraG { get; set; }
}

public class RegistroConsumoResultadoDto
{
    public long Id { get; set; }
    public int AlimentoId { get; set; }
    public ETipoTabela TipoTabela { get; set; }
    public string NomeAlimento { get; set; } = string.Empty;
    public double QuantidadeConsumidaG { get; set; }
    public DateTime DataConsumo { get; set; }
    public ETipoRefeicao Refeicao { get; set; }

    public double EnergiaKcal { get; set; }
    public double ProteinaG { get; set; }
    public double CarboidratoG { get; set; }
    public double GorduraG { get; set; }
    public double FibraG { get; set; }

    /// <summary>
    /// Se veio de leitura de código de barras.
    /// </summary>
    public string? CodigoBarras { get; set; }

    /// <summary>
    /// Nome do item planejado que este registro cobre (se vinculado ao plano).
    /// </summary>
    public string? ItemPlanoVinculado { get; set; }
}

public class FotoRefeicaoResultadoDto
{
    public int Id { get; set; }
    public ETipoRefeicao TipoRefeicao { get; set; }
    public string FotoUrl { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public DateTime DataRegistro { get; set; }
}

// ============================================================
// DTOs de Relatório de Aderência
// ============================================================

public class RelatorioAdesaoDto
{
    public DateTime DataInicio { get; set; }
    public DateTime DataFim { get; set; }
    public int TotalDias { get; set; }
    public int DiasComRegistro { get; set; }

    /// <summary>
    /// Percentual médio de aderência calórica ao plano.
    /// </summary>
    public double AderenciaCaloricoMediaPercent { get; set; }
    public double AderenciaProteinaMediaPercent { get; set; }
    public double AderenciaCarboidratoMediaPercent { get; set; }
    public double AderenciaGorduraMediaPercent { get; set; }

    /// <summary>
    /// Resumo macro médio por dia.
    /// </summary>
    public MacrosDiariosPlanoDto MediaDiariaConsumida { get; set; } = new();
    public MacrosDiariosPlanoDto MetaDiaria { get; set; } = new();

    /// <summary>
    /// Aderência por refeição.
    /// </summary>
    public List<AderenciaPorRefeicaoDto> AderenciaPorRefeicao { get; set; } = new();

    /// <summary>
    /// Detalhamento diário (tendência).
    /// </summary>
    public List<AderenciaDiariaDto> HistoricoDiario { get; set; } = new();
}

public class AderenciaPorRefeicaoDto
{
    public ETipoRefeicao TipoRefeicao { get; set; }
    public double AderenciaMediaPercent { get; set; }
    public int TotalRegistros { get; set; }
}

public class AderenciaDiariaDto
{
    public DateTime Data { get; set; }
    public double CaloriasConsumidas { get; set; }
    public double CaloriasMeta { get; set; }
    public double AderenciaPercent { get; set; }
    public int TotalRegistros { get; set; }
}

// ============================================================
// Consulta por período
// ============================================================

public class ConsultaPeriodoDto
{
    public DateTime DataInicio { get; set; }
    public DateTime DataFim { get; set; }
}
