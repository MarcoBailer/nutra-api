using Microsoft.EntityFrameworkCore;
using Nutra.Data;
using Nutra.Helper;
using Nutra.Models.Alimentos;

namespace Nutra.Seeder;

/// <summary>
/// Popula as tabelas de alimentos a partir do banco SQLite embutido.
/// Popula tabelas vazias e corrige metadados de porção em tabelas ja populadas.
/// </summary>
public static class DatabaseSeeder
{
    private const int TamanhoLote = 1000;

    public static async Task SeedAsync(IServiceProvider services, ILogger logger)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AlimentosContext>();

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
        if (await db.Tbcas.AnyAsync())
        {
            logger.LogInformation("TBCA ja populada. Seed ignorado para esta tabela.");
            return;
        }

        var itensAntigos = await sqlite.Tbcas.AsNoTracking().ToListAsync();
        logger.LogInformation("TBCA: {Count} registros encontrados no SQLite.", itensAntigos.Count);

        var lote = new List<Tbca>();
        int total = 0;

        foreach (var old in itensAntigos)
        {
            lote.Add(new Tbca
            {
                Id = old.Id,
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

        if (await db.FastFoods.AnyAsync())
        {
            await AtualizarPorcoesFastFoodAsync(itensAntigos, db, logger);
            return;
        }

        var lote = new List<FastFood>();
        int total = 0;

        foreach (var old in itensAntigos)
        {
            var porcao = PorcaoParser.Parse(old.Porcao);

            lote.Add(new FastFood
            {
                Id = old.Id,
                Produto = old.Produto ?? "Desconhecido",
                Fabricante = old.Fabricante ?? "Desconhecido",
                PorcaoTexto = porcao.TextoOriginal,
                Dose = porcao.Dose,
                Unidade = porcao.Unidade,
                Porcao = porcao.Quantidade,
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

        if (await db.Fabricantes.AnyAsync())
        {
            await AtualizarPorcoesFabricantesAsync(itensAntigos, db, logger);
            return;
        }

        var lote = new List<Fabricantes>();
        int total = 0;

        foreach (var old in itensAntigos)
        {
            var porcao = PorcaoParser.Parse(old.Porcao);

            lote.Add(new Fabricantes
            {
                Id = old.Id,
                Fabricante = old.Fabricante ?? "Desconhecido",
                Produto = old.Produto ?? "Desconhecido",
                PorcaoTexto = porcao.TextoOriginal,
                Dose = porcao.Dose,
                Unidade = porcao.Unidade,
                Porcao = porcao.Quantidade,
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

        if (await db.Genericos.AnyAsync())
        {
            await AtualizarPorcoesGenericosAsync(itensAntigos, db, logger);
            return;
        }

        var lote = new List<Genericos>();
        int total = 0;

        foreach (var old in itensAntigos)
        {
            var porcao = PorcaoParser.Parse(old.Porcao);

            lote.Add(new Genericos
            {
                Id = old.Id,
                CategoriaPrincipal = old.CategoriaPrincipal ?? "Desconhecido",
                SubCategoria = old.SubCategoria ?? "Desconhecido",
                Produto = old.Produto ?? "Desconhecido",
                PorcaoTexto = porcao.TextoOriginal,
                Dose = porcao.Dose,
                Unidade = porcao.Unidade,
                Porcao = porcao.Quantidade,
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

    private static async Task AtualizarPorcoesFastFoodAsync(
        List<Seeder.Models.FastFoodSqlite> itensAntigos,
        AlimentosContext db,
        ILogger logger)
    {
        var existentes = await db.FastFoods.ToDictionaryAsync(item => item.Id);
        var atualizados = 0;

        foreach (var old in itensAntigos)
        {
            if (!existentes.TryGetValue(old.Id, out var entity))
            {
                continue;
            }

            AplicarPorcao(entity, old.Porcao);
            atualizados++;
        }

        await db.SaveChangesAsync();
        db.ChangeTracker.Clear();
        logger.LogInformation("FastFood ja populada. Metadados de porcao atualizados: {Total}", atualizados);
    }

    private static async Task AtualizarPorcoesFabricantesAsync(
        List<Seeder.Models.FabricantesSqlite> itensAntigos,
        AlimentosContext db,
        ILogger logger)
    {
        var existentes = await db.Fabricantes.ToDictionaryAsync(item => item.Id);
        var atualizados = 0;

        foreach (var old in itensAntigos)
        {
            if (!existentes.TryGetValue(old.Id, out var entity))
            {
                continue;
            }

            AplicarPorcao(entity, old.Porcao);
            atualizados++;
        }

        await db.SaveChangesAsync();
        db.ChangeTracker.Clear();
        logger.LogInformation("Fabricantes ja populada. Metadados de porcao atualizados: {Total}", atualizados);
    }

    private static async Task AtualizarPorcoesGenericosAsync(
        List<Seeder.Models.GenericosSqlite> itensAntigos,
        AlimentosContext db,
        ILogger logger)
    {
        var existentes = await db.Genericos.ToDictionaryAsync(item => item.Id);
        var atualizados = 0;

        foreach (var old in itensAntigos)
        {
            if (!existentes.TryGetValue(old.Id, out var entity))
            {
                continue;
            }

            AplicarPorcao(entity, old.Porcao);
            atualizados++;
        }

        await db.SaveChangesAsync();
        db.ChangeTracker.Clear();
        logger.LogInformation("Genericos ja populada. Metadados de porcao atualizados: {Total}", atualizados);
    }

    private static void AplicarPorcao(FastFood entity, string? porcaoTexto)
    {
        var porcao = PorcaoParser.Parse(porcaoTexto);
        entity.PorcaoTexto = porcao.TextoOriginal;
        entity.Dose = porcao.Dose;
        entity.Unidade = porcao.Unidade;
        entity.Porcao = porcao.Quantidade;
    }

    private static void AplicarPorcao(Fabricantes entity, string? porcaoTexto)
    {
        var porcao = PorcaoParser.Parse(porcaoTexto);
        entity.PorcaoTexto = porcao.TextoOriginal;
        entity.Dose = porcao.Dose;
        entity.Unidade = porcao.Unidade;
        entity.Porcao = porcao.Quantidade;
    }

    private static void AplicarPorcao(Genericos entity, string? porcaoTexto)
    {
        var porcao = PorcaoParser.Parse(porcaoTexto);
        entity.PorcaoTexto = porcao.TextoOriginal;
        entity.Dose = porcao.Dose;
        entity.Unidade = porcao.Unidade;
        entity.Porcao = porcao.Quantidade;
    }
}
