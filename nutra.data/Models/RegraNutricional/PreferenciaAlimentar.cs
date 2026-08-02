using Nutra.Enum;
using Nutra.Models.Usuario;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nutra.Models.RegraNutricional;

public class PreferenciaAlimentar
{
    public int Id { get; set; }
    public int PerfilNutricionalId { get; set; }

    [ForeignKey("PerfilNutricionalId")]
    public virtual PerfilNutricional Perfil { get; set; }
    public int AlimentoId { get; set; } // Ex: "Brócolis", "Peixes", "Doces"
    public ETipoTabela Tabela { get; set; } // Ex: Tbcas, Fabricantes, FastFoods, Genericos
    public ETipoPreferencia Tipo { get; set; }
}
