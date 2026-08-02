using System.ComponentModel.DataAnnotations.Schema;

namespace Nutra.Models.Usuario;

public class RegistroBiometrico
{
    public int Id { get; set; }
    public DateTime Data { get; set; }

    public double PesoKg { get; set; }
    public double? PercentualGordura { get; set; }
    public double? CircunferenciaCinturaCm { get; set; }
    // Pode adicionar fotos aqui no futuro (string UrlFoto)

    public int PerfilNutricionalId { get; set; }

    [ForeignKey("PerfilNutricionalId")]
    public virtual PerfilNutricional Perfil { get; set; }
}
