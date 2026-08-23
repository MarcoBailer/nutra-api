using System.ComponentModel.DataAnnotations.Schema;

namespace Nutra.Models.Usuario;

public class MetaNutricional
{
    //O que usuário deve consumir por dia
    public int Id { get; set; }
    public DateTime DataCalculo { get; set; } = DateTime.UtcNow;
    public double CaloriasDiarias { get; set; }
    public double ProteinasDiarias { get; set; }
    public double CarboidratosDiarios { get; set; }
    public double GordurasDiarias { get; set; }
    public double AguaDiaria { get; set; }
    public double FibraDiaria { get; set; }

    // TODO: Vitaminas diarias ou semanais 

    public int PerfilNutricionalId { get; set; }
    public PerfilNutricional PerfilNutricional { get; set; }
}
