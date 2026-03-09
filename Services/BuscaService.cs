using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Nutra.Data;
using Nutra.Enum;
using Nutra.Interfaces;
using Nutra.Models.Alimentos;
using Nutra.Models.Dtos;

namespace Nutra.Services
{
    public class BuscaService : IBusca
    {
        private readonly IDbContextFactory<AlimentosContext> _contextFactory;
        private readonly AlimentosContext _context;
        public BuscaService(IDbContextFactory<AlimentosContext> contextFactory, AlimentosContext context)
        {
            _contextFactory = contextFactory;
            _context = context;
        }
        public async Task<List<AlimentoResumoDto>> BuscaAlimentoAsync(string termo)
        {
            if (string.IsNullOrWhiteSpace(termo)) return new List<AlimentoResumoDto>();

            termo = termo.ToLower();

            var resultadoFinal = new List<AlimentoResumoDto>();

            var taskTbca = Task.Run(async () =>
            {
                using var context = await _contextFactory.CreateDbContextAsync();

                var dados = await context.Tbcas
                    .AsNoTracking()
                    .Where(t => t.Nome != null && t.Nome.ToLower().Contains(termo))
                    .Take(20)
                    .ToListAsync();

                return dados.Select(MapTbcaToDto);
            });

            var taskFab = Task.Run(async () =>
            {
                using var context = await _contextFactory.CreateDbContextAsync();
                var dados = await context.Fabricantes
                    .AsNoTracking()
                    .Where(f => f.Produto != null && f.Produto.ToLower().Contains(termo))
                    .Take(10)
                    .ToListAsync();

                return dados.Select(MapFabricanteToDto);
            });

            var taskFast = Task.Run(async () =>
            {
                using var context = await _contextFactory.CreateDbContextAsync();
                var dados = await context.FastFoods
                    .AsNoTracking()
                    .Where(ff => ff.Produto != null && ff.Produto.ToLower().Contains(termo))
                    .Take(10)
                    .ToListAsync();

                return dados.Select(MapFastFoodToDto);
            });

            var taskGenerico = Task.Run(async () =>
            {
                using var context = await _contextFactory.CreateDbContextAsync();
                var dados = await context.Genericos
                    .AsNoTracking()
                    .Where(g => g.Produto != null && g.Produto.ToLower().Contains(termo))
                    .Take(10)
                    .ToListAsync();
                return dados.Select(MapGenericoToDto);
            });

            await Task.WhenAll(taskTbca, taskFab, taskFast);

            resultadoFinal.AddRange(taskTbca.Result);
            resultadoFinal.AddRange(taskFab.Result);
            resultadoFinal.AddRange(taskFast.Result);
            resultadoFinal.AddRange(taskGenerico.Result);

            return resultadoFinal.OrderBy(a => a.Nome.Length).ToList();

        }

        public async Task<AlimentoResumoDto> BuscaAlimentoPorIdAsync(int id, ETipoTabela tabela)
        {
            AlimentoResumoDto? alimentoEncontrado = null;

            switch (tabela)
            {
                case ETipoTabela.Tbcas:
                    var tbca = await _context.Tbcas
                        .AsNoTracking()
                        .FirstOrDefaultAsync(x => x.Id == id);
                    if (tbca != null)
                    {
                        alimentoEncontrado = MapTbcaToDto(tbca);
                    }
                    break;
                case ETipoTabela.Fabricantes:
                    var fabricante = await _context.Fabricantes
                        .AsNoTracking()
                        .FirstOrDefaultAsync(x => x.Id == id);
                    if (fabricante != null)
                    {
                        alimentoEncontrado = MapFabricanteToDto(fabricante);
                    }
                    break;
                case ETipoTabela.FastFoods:
                    var fastFood = await _context.FastFoods
                        .AsNoTracking()
                        .FirstOrDefaultAsync(x => x.Id == id);
                    if (fastFood != null)
                    {
                        alimentoEncontrado = MapFastFoodToDto(fastFood);
                    }
                    break;
                case ETipoTabela.Genericos:
                    var generico = await _context.Genericos
                        .AsNoTracking()
                        .FirstOrDefaultAsync(x => x.Id == id);
                    if (generico != null)
                    {
                        alimentoEncontrado = MapGenericoToDto(generico);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(tabela), tabela, null);
            }

            return alimentoEncontrado;
        }

        private AlimentoResumoDto MapTbcaToDto(Tbca t)
        {
            return new AlimentoResumoDto
            {
                Id = t.Id,
                Nome = t.Nome,
                NomeCientifico = t.NomeCientifico ?? "Desconhecido",
                MarcaFabricante = t.Marca,
                Grupo = t.Grupo ?? "Desconhecido",
                Fonte = "TBCA",
                Macros = new()
                {
                    EnergiaKcal = t.EnergiaKcal ?? 0,
                    EnergiaKJ = t.EnergiaKJ ?? 0,
                    Proteina = t.ProteinaG ?? 0,
                    CarboDisponivel = t.CarboidratoDisponivelG ?? 0,
                    Fibras = t.FibraAlimentarG ?? 0,
                    Acucar = t.AcucarDeAdicaoG ?? 0,
                    LipidiosG = t.LipidiosG ?? 0,
                    Umidade = t.UmidadeG ?? 0,
                    CarboTotal = t.CarboidratoTotalG ?? 0,
                    AlcoolG = t.AlcoolG ?? 0,
                },
                Minerais = new()
                {
                    ManganesMg = t.ManganesMg ?? 0,
                    MagnesioMg = t.MagnesioMg ?? 0,
                    FosforoMg = t.FosforoMg ?? 0,
                    FerroMg = t.FerroMg ?? 0,
                    NiacinaMg = t.NiacinaMg ?? 0,
                    CalcioMg = t.CalcioMg ?? 0,
                    PotassioMg = t.PotassioMg ?? 0,
                    SelenioMcg = t.SelenioMcg ?? 0,
                    SodioMg = t.SodioMg ?? 0,
                    ZincoMg = t.ZincoMg ?? 0,
                    CobreMg = t.CobreMg ?? 0,
                    CinzasG = t.CinzasG ?? 0,
                },
                Vitaminas = new()
                {
                    VitaminaDMcg = t.VitaminaDMcg ?? 0,
                    VitaminaARaeMcg = t.VitaminaARaeMcg ?? 0,
                    VitaminaAReMcg = t.VitaminaAReMcg ?? 0,
                    VitaminaCMg = t.VitaminaCMg ?? 0,
                    VitaminaB12Mcg = t.VitaminaB12Mcg ?? 0,
                    VitaminaB6Mg = t.VitaminaB6Mg ?? 0,
                    RiboflavinaMg = t.RiboflavinaMg ?? 0,
                    TiaminaMg = t.TiaminaMg ?? 0,
                    AlfaTocoferolVitaminaEMg = t.AlfaTocoferolVitaminaEMg ?? 0,
                },
                Gorduras = new()
                {
                    Totais = t.LipidiosG ?? 0,
                    Saturadas = t.AcidosGraxosSaturadosG ?? 0,
                    Trans = t.AcidosGraxosTransG ?? 0,
                    ColesterolMg = t.ColesterolMg ?? 0,
                    Poliinsaturadas = t.AcidosGraxosPoliinsaturadosG ?? 0,
                    Monoinsaturadas = t.AcidosGraxosMonoinsaturadosG ?? 0,
                }
            };
        }

        private AlimentoResumoDto MapFabricanteToDto(Fabricantes f)
        {
            return new AlimentoResumoDto
            {
                Id = f.Id,
                Nome = f.Produto ?? "Desconhecido",
                MarcaFabricante = f.Fabricante ?? "Genérico",
                PorcaoReferencia = f.Porcao ?? 0,
                Fonte = "Fabricantes",
                Macros = new()
                {
                    EnergiaKcal = f.EnergiaKcal ?? 0,
                    EnergiaKJ = f.EnergiaKj ?? 0,
                    Proteina = f.Proteinas ?? 0,
                    CarboDisponivel = f.Carboidratos ?? 0,
                    LipidiosG = f.Gorduras ?? 0,
                    Acucar = f.Acucar ?? 0,
                    Fibras = f.Fibras ?? 0,
                },
                Minerais = new()
                {
                    SodioMg = f.Sodio ?? 0,
                    PotassioMg = f.Potassio ?? 0,
                },
                Gorduras = new()
                {
                    Totais = f.Gorduras ?? 0,
                    Saturadas = f.GorduraSaturada ?? 0,
                    ColesterolMg = f.Colesterol ?? 0,
                    Monoinsaturadas = f.GorduraMonoinsaturada ?? 0,
                    Poliinsaturadas = f.GorduraPoliinsaturada ?? 0,
                }
            };
        }

        private AlimentoResumoDto MapFastFoodToDto(FastFood ff)
        {
            return new AlimentoResumoDto
            {
                Id = ff.Id,
                Nome = ff.Produto ?? "Desconhecido",
                PorcaoReferencia = ff.Porcao ?? 0,
                MarcaFabricante = ff.Fabricante ?? "Restaurante",
                Fonte = "FastFood",
                Macros = new()
                {
                    EnergiaKcal = ff.EnergiaKcal ?? 0,
                    EnergiaKJ = ff.EnergiaKj ?? 0,
                    Proteina = ff.Proteinas ?? 0,
                    CarboDisponivel = ff.Carboidratos ?? 0,
                    LipidiosG = ff.Gorduras ?? 0,
                    Acucar = ff.Acucar ?? 0,
                    Fibras = ff.Fibras ?? 0,
                },
                Minerais = new()
                {
                    SodioMg = ff.Sodio ?? 0,
                    PotassioMg = ff.Potassio ?? 0,
                },
                Gorduras = new()
                {
                    Totais = ff.Gorduras ?? 0,
                    Saturadas = ff.GorduraSaturada ?? 0,
                    ColesterolMg = ff.Colesterol ?? 0,
                    Monoinsaturadas = ff.GorduraMonoinsaturada ?? 0,
                    Poliinsaturadas = ff.GorduraPoliinsaturada ?? 0,
                }
            };
        }

        private AlimentoResumoDto MapGenericoToDto(Genericos g)
        {
            return new AlimentoResumoDto
            {
                Id = g.Id,
                Nome = g.Produto ?? "Desconhecido",
                PorcaoReferencia = g.Porcao ?? 0,
                MarcaFabricante = "Genérico",
                Fonte = "Genericos",
                Macros = new()
                {
                    EnergiaKcal = g.EnergiaKcal ?? 0,
                    EnergiaKJ = g.EnergiaKj ?? 0,
                    Proteina = g.Proteinas ?? 0,
                    CarboDisponivel = g.Carboidratos ?? 0,
                    LipidiosG = g.Gorduras ?? 0,
                    Acucar = g.Acucar ?? 0,
                    Fibras = g.Fibras ?? 0,
                },
                Minerais = new()
                {
                    SodioMg = g.Sodio ?? 0,
                    PotassioMg = g.Potassio ?? 0,
                },
                Gorduras = new()
                {
                    Totais = g.Gorduras ?? 0,
                    Saturadas = g.GorduraSaturada ?? 0,
                    ColesterolMg = g.Colesterol ?? 0,
                    Monoinsaturadas = g.GorduraMonoinsaturada ?? 0,
                    Poliinsaturadas = g.GorduraPoliinsaturada ?? 0,
                }
            };
        }
    }
}
