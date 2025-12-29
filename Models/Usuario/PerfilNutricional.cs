using Nutra.Enum;
using Nutra.Models.RegraNutricional;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nutra.Models.Usuario;

public class PerfilNutricional
{
    public int Id { get; set; }
    public string UserId { get; set; }

    [ForeignKey("UserId")]
    public ApplicationUser User { get; set; }
    public int? MetaNutricionalAtualId { get; set; }

    [ForeignKey("MetaNutricionalAtualId")]
    public MetaNutricional? MetaNutricional { get; set; }

    public double AlturaCm { get; set; }
    public double PesoAtualKg { get; set; }
    public double? PercentualGorduraCorporal { get; set; }
    public double FatorAtividade { get; set; }
    public string OcupacaoProfissional { get; set; } = string.Empty;
    public bool PossuiDoencasPreExistentes { get; set; }
    public string DescricaoCondicoesMedicas { get; set; } = string.Empty;
    public double? PesoDesejadoKg { get; set; }
    public int RefeicoesPorDiaDesejadas { get; set; }
    public int TempoDisponivelPreparoMinutos { get; set; }
    public double? CircunferenciaCinturaCm { get; set; }
    public DateTime DataNascimento { get; set; }
    public EGeneroBiologico Genero { get; set; }
    public ETipoObjetivo Objetivo { get; set; }
    public ENivelAtividadeFisica NivelAtividade { get; set; }
    public EPreferenciaAlimentar PreferenciaDieta { get; set; }
    public ICollection<RestricaoAlimentar> RestricoesAlimentares { get; set; } = new List<RestricaoAlimentar>();
    public ICollection<PreferenciaAlimentar>? PreferenciasAlimentares { get; set; } = new List<PreferenciaAlimentar>();
    public ICollection<RegistroBiometrico> HistoricoMedidas { get; set; }
    public ICollection<PerfilEquipamento> EquipamentoDisponivel { get; set; }
}
