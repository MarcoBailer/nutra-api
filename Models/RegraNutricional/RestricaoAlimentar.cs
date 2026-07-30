using Nutra.Enum;
using Nutra.Models.Usuario;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nutra.Models.RegraNutricional;

public class RestricaoAlimentar
{
    public int Id { get; set; }
    public int PerfilNutricionalId { get; set; }
    [ForeignKey("PerfilNutricionalId")]
    public virtual PerfilNutricional Perfil { get; set; }
    public EAlergico CompostoOrganico { get; set; } // e.g., Lactose, Glúten
}
