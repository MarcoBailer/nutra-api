using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Nutra.Models.Alimentos;
using Nutra.Models.RegraNutricional;
using Nutra.Models.Usuario;

namespace Nutra.Data;

public class AlimentosContext : IdentityDbContext<ApplicationUser>
{
    public AlimentosContext(DbContextOptions<AlimentosContext> options)
        : base(options)
    {
    }

    public DbSet<Fabricantes> Fabricantes { get; set; }

    public DbSet<FastFood> FastFoods { get; set; }

    public DbSet<Tbca> Tbcas { get; set; }

    public DbSet<Genericos> Genericos { get; set; }

    public DbSet<PerfilNutricional> PerfilNutricional { get; set; }

    public DbSet<MetaNutricional> MetasNutricionais { get; set; }

    public DbSet<PerfilEquipamento> PerfisEquipamentos { get; set; }

    public DbSet<PreferenciaAlimentar> PreferenciaAlimentar { get; set; }
    public DbSet<RegistroBiometrico> RegistroBiometrico { get; set; }
    public DbSet<RestricaoAlimentar> RestricaoAlimentar { get; set; }
    public DbSet<RegistroAlimentar> RegistroAlimentar { get; set; }
}
