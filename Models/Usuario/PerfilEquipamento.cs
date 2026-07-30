using Nutra.Enum;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nutra.Models.Usuario;

public class PerfilEquipamento
{
    public int Id { get; set; }
    public int PerfilNutricionalId { get; set; }
    [ForeignKey("PerfilNutricionalId")]
    public PerfilNutricional PerfilNutricional { get; set; }
    public EEquipamentoDisponivel Equipamento { get; set; }
}
