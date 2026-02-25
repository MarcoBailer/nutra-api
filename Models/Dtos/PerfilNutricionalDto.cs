using Nutra.Enum;
using System.ComponentModel.DataAnnotations;

namespace Nutra.Models.Dtos;

public class PerfilNutricionalDto
{
    public string UserId { get; set; } = string.Empty;

    public double AlturaCm { get; set; }
    public double PesoAtualKg { get; set; }
    public double? PercentualGorduraCorporal { get; set; }
    public double? CircunferenciaCinturaCm { get; set; }
    public double? CircunferenciaQuadrilCm { get; set; }
    public double? CircunferenciaBracoCm { get; set; }

    public double FatorAtividade { get; set; }
    public ENivelAtividadeFisica NivelAtividade { get; set; }

    [MaxLength(300)]
    public string OcupacaoProfissional { get; set; } = string.Empty;

    public ENivelHabilidadeCulinaria HabilidadeCulinaria { get; set; } = ENivelHabilidadeCulinaria.Intermediario;
    public EOrcamentoMensalEstimado OrcamentoMensal { get; set; } = EOrcamentoMensalEstimado.Medio;

    public bool PossuiDoencasPreExistentes { get; set; }

    [MaxLength(2000)]
    public string DescricaoCondicoesMedicas { get; set; } = string.Empty;

    public bool Fumante { get; set; }
    public int? QualidadeSono { get; set; }
    public double? HorasSonoPorNoite { get; set; }

    public double? PesoDesejadoKg { get; set; }
    public int RefeicoesPorDiaDesejadas { get; set; }
    public int TempoDisponivelPreparoMinutos { get; set; }
    public DateTime DataNascimento { get; set; }
    public EGeneroBiologico Genero { get; set; }
    public ETipoObjetivo Objetivo { get; set; }
    public EPreferenciaAlimentar PreferenciaDieta { get; set; }

    public List<EAlergico> RestricoesIds { get; set; } = new List<EAlergico>();
    public List<PreferenciaCadastroDto> Preferencias { get; set; } = new List<PreferenciaCadastroDto>();
    public List<EEquipamentoDisponivel> EquipamentosIds { get; set; } = new List<EEquipamentoDisponivel>();
    public List<HistoricoClinicoDto> HistoricoClinicos { get; set; } = new List<HistoricoClinicoDto>();
}
