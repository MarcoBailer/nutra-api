using Microsoft.EntityFrameworkCore;
using Nutra.Data;
using Nutra.Helper;
using Nutra.Models.Alimentos;

namespace Nutra.Seeder;

/// <summary>
/// Popula as tabelas de alimentos a partir do banco SQLite embutido.
/// Executa apenas uma vez — se qualquer tabela já tiver dados, o seed é ignorado.
/// </summary>
public static class DatabaseSeeder
{
    private const int TamanhoLote = 1000;

    public static async Task SeedAsync(IServiceProvider services, ILogger logger)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AlimentosContext>();

        // Verifica se já foi populado (qualquer tabela com dados = já rodou)
        if (await db.Tbcas.AnyAsync() ||
            await db.FastFoods.AnyAsync() ||
            await db.Fabricantes.AnyAsync() ||
            await db.Genericos.AnyAsync())
        {
            logger.LogInformation("Tabelas de alimentos já populadas. Seed ignorado.");
            return;
        }

        // Localiza o arquivo alimentos.db embutido no projeto
        var dbPath = Path.Combine(AppContext.BaseDirectory, "Data", "alimentos.db");
        if (!File.Exists(dbPath))
        {
            logger.LogWarning("Arquivo alimentos.db não encontrado em {Path}. Seed ignorado.", dbPath);
            return;
        }

        logger.LogInformation("Iniciando seed das tabelas de alimentos a partir do SQLite...");

        using var sqlite = new SqliteSourceContext(dbPath);

        await SeedTbcaAsync(sqlite, db, logger);
        await SeedFastFoodAsync(sqlite, db, logger);
        await SeedFabricantesAsync(sqlite, db, logger);
        await SeedGenericosAsync(sqlite, db, logger);

        logger.LogInformation("Seed das tabelas de alimentos concluído com sucesso!");
    }

    private static async Task SeedTbcaAsync(SqliteSourceContext sqlite, AlimentosContext db, ILogger logger)
    {
        var itensAntigos = await sqlite.Tbcas.AsNoTracking().ToListAsync();
        logger.LogInformation("TBCA: {Count} registros encontrados no SQLite.", itensAntigos.Count);

        var lote = new List<Tbca>();
        int total = 0;

        foreach (var old in itensAntigos)
        {
            lote.Add(new Tbca
            {
                Nome = old.Nome ?? "Desconhecido",
                NomeCientifico = old.NomeCientifico ?? "Desconhecido",
                Grupo = old.Grupo ?? "Desconhecido",
                Marca = old.Marca ?? "Desconhecido",
                AlfaTocoferolVitaminaEMg = Conversor.LimparEConverter(old.AlfaTocoferolVitaminaEMg),
                AcucarDeAdicaoG = Conversor.LimparEConverter(old.AcucarDeAdicaoG),
                CarboidratoDisponivelG = Conversor.LimparEConverter(old.CarboidratoDisponivelG),
                CarboidratoTotalG = Conversor.LimparEConverter(old.CarboidratoTotalG),
                CinzasG = Conversor.LimparEConverter(old.CinzasG),
                CobreMg = Conversor.LimparEConverter(old.CobreMg),
                ColesterolMg = Conversor.LimparEConverter(old.ColesterolMg),
                CalcioMg = Conversor.LimparEConverter(old.CalcioMg),
                EnergiaKJ = old.EnergiaKJ,
                EnergiaKcal = old.EnergiaKcal,
                EquivalenteDeFolatoMcg = Conversor.LimparEConverter(old.EquivalenteDeFolatoMcg),
                FerroMg = Conversor.LimparEConverter(old.FerroMg),
                FibraAlimentarG = Conversor.LimparEConverter(old.FibraAlimentarG),
                FosforoMg = Conversor.LimparEConverter(old.FosforoMg),
                LipidiosG = Conversor.LimparEConverter(old.LipidiosG),
                MagnesioMg = Conversor.LimparEConverter(old.MagnesioMg),
                ManganesMg = Conversor.LimparEConverter(old.ManganesMg),
                NiacinaMg = Conversor.LimparEConverter(old.NiacinaMg),
                PotassioMg = Conversor.LimparEConverter(old.PotassioMg),
                ProteinaG = Conversor.LimparEConverter(old.ProteinaG),
                RiboflavinaMg = Conversor.LimparEConverter(old.RiboflavinaMg),
                SalDeAdicaoG = Conversor.LimparEConverter(old.SalDeAdicaooG),
                SelenioMcg = Conversor.LimparEConverter(old.SelenioMcg),
                SodioMg = Conversor.LimparEConverter(old.SodioMg),
                TiaminaMg = Conversor.LimparEConverter(old.TiaminaMg),
                UmidadeG = Conversor.LimparEConverter(old.UmidadeG),
                VitaminaARaeMcg = Conversor.LimparEConverter(old.VitaminaARaeMcg),
                VitaminaAReMcg = Conversor.LimparEConverter(old.VitaminaAReMcg),
                VitaminaB12Mcg = Conversor.LimparEConverter(old.VitaminaB12Mcg),
                VitaminaB6Mg = Conversor.LimparEConverter(old.VitaminaB6Mg),
                VitaminaCMg = Conversor.LimparEConverter(old.VitaminaCMg),
                VitaminaDMcg = Conversor.LimparEConverter(old.VitaminaDMcg),
                ZincoMg = Conversor.LimparEConverter(old.ZincoMg),
                AcidosGraxosMonoinsaturadosG = Conversor.LimparEConverter(old.AcidosGraxosMonoinsaturadosG),
                AcidosGraxosPoliinsaturadosG = Conversor.LimparEConverter(old.AcidosGraxosPoliinsaturadosG),
                AcidosGraxosSaturadosG = Conversor.LimparEConverter(old.AcidosGraxosSaturadosG),
                AcidosGraxosTransG = Conversor.LimparEConverter(old.AcidosGraxosTransG),
                AlcoolG = Conversor.LimparEConverter(old.AlcoolG),
            });

            total++;
            if (lote.Count >= TamanhoLote)
            {
                await db.Tbcas.AddRangeAsync(lote);
                await db.SaveChangesAsync();
                db.ChangeTracker.Clear();
                lote.Clear();
                logger.LogInformation("  TBCA: {Total} registros processados...", total);
            }
        }

        if (lote.Count > 0)
        {
            await db.Tbcas.AddRangeAsync(lote);
            await db.SaveChangesAsync();
            db.ChangeTracker.Clear();
        }

        logger.LogInformation("TBCA concluída! Total: {Total}", total);
    }

    private static async Task SeedFastFoodAsync(SqliteSourceContext sqlite, AlimentosContext db, ILogger logger)
    {
        var itensAntigos = await sqlite.FastFoods.AsNoTracking().ToListAsync();
        logger.LogInformation("FastFood: {Count} registros encontrados no SQLite.", itensAntigos.Count);

        var lote = new List<FastFood>();
        int total = 0;

        foreach (var old in itensAntigos)
        {
            lote.Add(new FastFood
            {
                Produto = old.Produto ?? "Desconhecido",
                Fabricante = old.Fabricante ?? "Desconhecido",
                Porcao = Conversor.LimparEConverter(old.Porcao),
                EnergiaKcal = Conversor.LimparEConverter(old.EnergiaKcal),
                EnergiaKj = Conversor.LimparEConverter(old.EnergiaKj),
                Proteinas = Conversor.LimparEConverter(old.Proteinas),
                Carboidratos = Conversor.LimparEConverter(old.Carboidratos),
                Acucar = Conversor.LimparEConverter(old.Acucar),
                Gorduras = Conversor.LimparEConverter(old.Gorduras),
                GorduraSaturada = Conversor.LimparEConverter(old.GorduraSaturada),
                GorduraPoliinsaturada = Conversor.LimparEConverter(old.GorduraPoliinsaturada),
                GorduraMonoinsaturada = Conversor.LimparEConverter(old.GorduraMonoinsaturada),
                GorduraTrans = Conversor.LimparEConverter(old.GorduraTrans),
                Colesterol = Conversor.LimparEConverter(old.Colesterol),
                Fibras = Conversor.LimparEConverter(old.Fibras),
                Sodio = Conversor.LimparEConverter(old.Sodio),
                Potassio = Conversor.LimparEConverter(old.Potassio),
            });

            total++;
            if (lote.Count >= TamanhoLote)
            {
                await db.FastFoods.AddRangeAsync(lote);
                await db.SaveChangesAsync();
                db.ChangeTracker.Clear();
                lote.Clear();
                logger.LogInformation("  FastFood: {Total} registros processados...", total);
            }
        }

        if (lote.Count > 0)
        {
            await db.FastFoods.AddRangeAsync(lote);
            await db.SaveChangesAsync();
            db.ChangeTracker.Clear();
        }

        logger.LogInformation("FastFood concluída! Total: {Total}", total);
    }

    private static async Task SeedFabricantesAsync(SqliteSourceContext sqlite, AlimentosContext db, ILogger logger)
    {
        var itensAntigos = await sqlite.Fabricantes.AsNoTracking().ToListAsync();
        logger.LogInformation("Fabricantes: {Count} registros encontrados no SQLite.", itensAntigos.Count);

        var lote = new List<Fabricantes>();
        int total = 0;

        foreach (var old in itensAntigos)
        {
            lote.Add(new Fabricantes
            {
                Fabricante = old.Fabricante ?? "Desconhecido",
                Produto = old.Produto ?? "Desconhecido",
                Porcao = Conversor.LimparEConverter(old.Porcao),
                EnergiaKcal = Conversor.LimparEConverter(old.EnergiaKcal),
                EnergiaKj = Conversor.LimparEConverter(old.EnergiaKj),
                Proteinas = Conversor.LimparEConverter(old.Proteinas),
                Carboidratos = Conversor.LimparEConverter(old.Carboidratos),
                Acucar = Conversor.LimparEConverter(old.Acucar),
                Gorduras = Conversor.LimparEConverter(old.Gorduras),
                GorduraSaturada = Conversor.LimparEConverter(old.GorduraSaturada),
                GorduraPoliinsaturada = Conversor.LimparEConverter(old.GorduraPoliinsaturada),
                GorduraMonoinsaturada = Conversor.LimparEConverter(old.GorduraMonoinsaturada),
                GorduraTrans = Conversor.LimparEConverter(old.GorduraTrans),
                Colesterol = Conversor.LimparEConverter(old.Colesterol),
                Fibras = Conversor.LimparEConverter(old.Fibras),
                Sodio = Conversor.LimparEConverter(old.Sodio),
                Potassio = Conversor.LimparEConverter(old.Potassio),
            });

            total++;
            if (lote.Count >= TamanhoLote)
            {
                await db.Fabricantes.AddRangeAsync(lote);
                await db.SaveChangesAsync();
                db.ChangeTracker.Clear();
                lote.Clear();
                logger.LogInformation("  Fabricantes: {Total} registros processados...", total);
            }
        }

        if (lote.Count > 0)
        {
            await db.Fabricantes.AddRangeAsync(lote);
            await db.SaveChangesAsync();
            db.ChangeTracker.Clear();
        }

        logger.LogInformation("Fabricantes concluída! Total: {Total}", total);
    }

    private static async Task SeedGenericosAsync(SqliteSourceContext sqlite, AlimentosContext db, ILogger logger)
    {
        var itensAntigos = await sqlite.Genericos.AsNoTracking().ToListAsync();
        logger.LogInformation("Genericos: {Count} registros encontrados no SQLite.", itensAntigos.Count);

        var lote = new List<Genericos>();
        int total = 0;

        foreach (var old in itensAntigos)
        {
            lote.Add(new Genericos
            {
                CategoriaPrincipal = old.CategoriaPrincipal ?? "Desconhecido",
                SubCategoria = old.SubCategoria ?? "Desconhecido",
                Produto = old.Produto ?? "Desconhecido",
                Porcao = Conversor.LimparEConverter(old.Porcao),
                EnergiaKcal = Conversor.LimparEConverter(old.EnergiaKcal),
                EnergiaKj = Conversor.LimparEConverter(old.EnergiaKj),
                Proteinas = Conversor.LimparEConverter(old.Proteinas),
                Carboidratos = Conversor.LimparEConverter(old.Carboidratos),
                Acucar = Conversor.LimparEConverter(old.Acucar),
                Gorduras = Conversor.LimparEConverter(old.Gorduras),
                GorduraSaturada = Conversor.LimparEConverter(old.GorduraSaturada),
                GorduraPoliinsaturada = Conversor.LimparEConverter(old.GorduraPoliinsaturada),
                GorduraMonoinsaturada = Conversor.LimparEConverter(old.GorduraMonoinsaturada),
                GorduraTrans = Conversor.LimparEConverter(old.GorduraTrans),
                Colesterol = Conversor.LimparEConverter(old.Colesterol),
                Fibras = Conversor.LimparEConverter(old.Fibras),
                Sodio = Conversor.LimparEConverter(old.Sodio),
                Potassio = Conversor.LimparEConverter(old.Potassio),
            });

            total++;
            if (lote.Count >= TamanhoLote)
            {
                await db.Genericos.AddRangeAsync(lote);
                await db.SaveChangesAsync();
                db.ChangeTracker.Clear();
                lote.Clear();
                logger.LogInformation("  Genericos: {Total} registros processados...", total);
            }
        }

        if (lote.Count > 0)
        {
            await db.Genericos.AddRangeAsync(lote);
            await db.SaveChangesAsync();
            db.ChangeTracker.Clear();
        }

        logger.LogInformation("Genericos concluída! Total: {Total}", total);
    }
}
