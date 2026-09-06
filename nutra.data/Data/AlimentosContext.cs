using Microsoft.EntityFrameworkCore;
using Nutra.Models.Alimentos;
using Nutra.Models.RegraNutricional;
using Nutra.Models.Usuario;

namespace Nutra.Data;

public class AlimentosContext : DbContext
{
    public AlimentosContext(DbContextOptions<AlimentosContext> options)
        : base(options)
    {
    }

    // --- Usuários (domínio local) ---
    public DbSet<ApplicationUser> ApplicationUsers { get; set; }

    // --- Alimentos ---
    public DbSet<Fabricantes> Fabricantes { get; set; }
    public DbSet<FastFood> FastFoods { get; set; }
    public DbSet<Tbca> Tbcas { get; set; }
    public DbSet<Genericos> Genericos { get; set; }

    // --- Perfil do paciente ---
    public DbSet<PerfilNutricional> PerfilNutricional { get; set; }
    public DbSet<MetaNutricional> MetasNutricionais { get; set; }
    public DbSet<PerfilEquipamento> PerfisEquipamentos { get; set; }

    // --- Regras nutricionais ---
    public DbSet<PreferenciaAlimentar> PreferenciaAlimentar { get; set; }
    public DbSet<RegistroBiometrico> RegistroBiometrico { get; set; }
    public DbSet<RestricaoAlimentar> RestricaoAlimentar { get; set; }
    public DbSet<RegistroAlimentar> RegistroAlimentar { get; set; }
    public DbSet<HistoricoClinico> HistoricoClinicos { get; set; }
    public DbSet<AnamneseAlimentar> AnamnesesAlimentares { get; set; }

    // --- Avaliação Nutricional ---
    public DbSet<AvaliacaoAntropometrica> AvaliacoesAntropometricas { get; set; }
    public DbSet<FotoProgresso> FotosProgresso { get; set; }

    // --- Plano Alimentar (Etapa 4) ---
    public DbSet<PlanoAlimentar> PlanosAlimentares { get; set; }
    public DbSet<RefeicaoPlano> RefeicoesPlanejadas { get; set; }
    public DbSet<ItemRefeicao> ItensRefeicao { get; set; }
    public DbSet<SubstituicaoEquivalente> SubstituicoesEquivalentes { get; set; }

    // --- Diário Alimentar (Etapa 5) ---
    public DbSet<FotoRefeicao> FotosRefeicao { get; set; }

    // --- Profissionais ---
    public DbSet<PerfilProfissional> PerfisProfissionais { get; set; }
    public DbSet<Clinica> Clinicas { get; set; }
    public DbSet<Assinatura> Assinaturas { get; set; }
    public DbSet<VinculoPacienteProfissional> VinculosPacienteProfissional { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // PerfilNutricional 1:1 com ApplicationUser
        builder.Entity<PerfilNutricional>()
            .HasIndex(p => p.UserId)
            .IsUnique();

        // MetaNutricional -> PerfilNutricional (evitar cascade duplo)
        builder.Entity<MetaNutricional>()
            .HasOne(m => m.PerfilNutricional)
            .WithMany()
            .HasForeignKey(m => m.PerfilNutricionalId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<PerfilNutricional>()
            .HasOne(p => p.MetaNutricional)
            .WithMany()
            .HasForeignKey(p => p.MetaNutricionalAtualId)
            .OnDelete(DeleteBehavior.SetNull);

        // PerfilProfissional 1:1 com ApplicationUser
        builder.Entity<PerfilProfissional>()
            .HasIndex(p => p.UserId)
            .IsUnique();

        builder.Entity<PerfilProfissional>()
            .HasIndex(p => p.CRN)
            .IsUnique();

        // Assinatura 1:1 com PerfilProfissional
        builder.Entity<Assinatura>()
            .HasIndex(a => a.PerfilProfissionalId)
            .IsUnique();

        // VinculoPacienteProfissional
        builder.Entity<VinculoPacienteProfissional>()
            .HasOne(v => v.Paciente)
            .WithMany(u => u.VinculosComoPaciente)
            .HasForeignKey(v => v.PacienteUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<VinculoPacienteProfissional>()
            .HasOne(v => v.Profissional)
            .WithMany(p => p.Pacientes)
            .HasForeignKey(v => v.PerfilProfissionalId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<VinculoPacienteProfissional>()
            .HasIndex(v => new { v.PacienteUserId, v.PerfilProfissionalId })
            .IsUnique()
            .HasFilter("\"Status\" IN (1, 2)"); // Pendente ou Ativo

        // Clinica -> PerfilProfissional
        builder.Entity<Clinica>()
            .HasOne(c => c.PerfilProfissional)
            .WithMany(p => p.Clinicas)
            .HasForeignKey(c => c.PerfilProfissionalId)
            .OnDelete(DeleteBehavior.Cascade);

        // HistoricoClinico -> PerfilNutricional
        builder.Entity<HistoricoClinico>()
            .HasOne(h => h.PerfilNutricional)
            .WithMany(p => p.HistoricosClinico)
            .HasForeignKey(h => h.PerfilNutricionalId)
            .OnDelete(DeleteBehavior.Cascade);

        // AnamneseAlimentar -> PerfilNutricional
        builder.Entity<AnamneseAlimentar>()
            .HasOne(a => a.PerfilNutricional)
            .WithMany(p => p.Anamneses)
            .HasForeignKey(a => a.PerfilNutricionalId)
            .OnDelete(DeleteBehavior.Cascade);

        // AvaliacaoAntropometrica -> PerfilNutricional
        builder.Entity<AvaliacaoAntropometrica>()
            .HasOne(a => a.PerfilNutricional)
            .WithMany(p => p.AvaliacoesAntropometricas)
            .HasForeignKey(a => a.PerfilNutricionalId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<AvaliacaoAntropometrica>()
            .HasOne(a => a.ProfissionalResponsavel)
            .WithMany()
            .HasForeignKey(a => a.ProfissionalResponsavelId)
            .OnDelete(DeleteBehavior.SetNull);

        // FotoProgresso -> AvaliacaoAntropometrica
        builder.Entity<FotoProgresso>()
            .HasOne(f => f.AvaliacaoAntropometrica)
            .WithMany(a => a.FotosProgresso)
            .HasForeignKey(f => f.AvaliacaoAntropometricaId)
            .OnDelete(DeleteBehavior.Cascade);

        // ============================================================
        // Etapa 4: Plano Alimentar
        // ============================================================

        // PlanoAlimentar -> PerfilNutricional
        builder.Entity<PlanoAlimentar>()
            .HasOne(p => p.PerfilNutricional)
            .WithMany(pn => pn.PlanosAlimentares)
            .HasForeignKey(p => p.PerfilNutricionalId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<PlanoAlimentar>()
            .HasOne(p => p.ProfissionalResponsavel)
            .WithMany()
            .HasForeignKey(p => p.ProfissionalResponsavelId)
            .OnDelete(DeleteBehavior.SetNull);

        // RefeicaoPlano -> PlanoAlimentar
        builder.Entity<RefeicaoPlano>()
            .HasOne(r => r.PlanoAlimentar)
            .WithMany(p => p.RefeicoesPlanejadas)
            .HasForeignKey(r => r.PlanoAlimentarId)
            .OnDelete(DeleteBehavior.Cascade);

        // ItemRefeicao -> RefeicaoPlano
        builder.Entity<ItemRefeicao>()
            .HasOne(i => i.RefeicaoPlano)
            .WithMany(r => r.Itens)
            .HasForeignKey(i => i.RefeicaoPlanoId)
            .OnDelete(DeleteBehavior.Cascade);

        // SubstituicaoEquivalente -> ItemRefeicao
        builder.Entity<SubstituicaoEquivalente>()
            .HasOne(s => s.ItemRefeicao)
            .WithMany(i => i.SubstituicoesEquivalentes)
            .HasForeignKey(s => s.ItemRefeicaoId)
            .OnDelete(DeleteBehavior.Cascade);

        // ============================================================
        // Etapa 5: Diário Alimentar
        // ============================================================

        // RegistroAlimentar -> PlanoAlimentar (opcional)
        builder.Entity<RegistroAlimentar>()
            .HasOne(r => r.PlanoAlimentar)
            .WithMany()
            .HasForeignKey(r => r.PlanoAlimentarId)
            .OnDelete(DeleteBehavior.SetNull);

        // RegistroAlimentar -> ItemRefeicaoPlano (opcional)
        builder.Entity<RegistroAlimentar>()
            .HasOne(r => r.ItemRefeicaoPlano)
            .WithMany()
            .HasForeignKey(r => r.ItemRefeicaoPlanoId)
            .OnDelete(DeleteBehavior.SetNull);

        // FotoRefeicao -> ApplicationUser
        builder.Entity<FotoRefeicao>()
            .HasOne(f => f.User)
            .WithMany()
            .HasForeignKey(f => f.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // FotoRefeicao -> RegistroAlimentar (opcional)
        builder.Entity<FotoRefeicao>()
            .HasOne(f => f.RegistroAlimentar)
            .WithMany(r => r.FotosRefeicao)
            .HasForeignKey(f => f.RegistroAlimentarId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
