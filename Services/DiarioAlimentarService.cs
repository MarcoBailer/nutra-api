using Microsoft.EntityFrameworkCore;
using Nutra.Data;
using Nutra.Enum;
using Nutra.Helper;
using Nutra.Interfaces;
using Nutra.Models;
using Nutra.Models.Dtos;
using Nutra.Models.RegraNutricional;
using Nutra.Models.Usuario;
using System.Text.Json;

namespace Nutra.Services;

public class DiarioAlimentarService : IDiarioAlimentar
{
    private readonly AlimentosContext _context;
    private readonly IBusca _busca;

    public DiarioAlimentarService(AlimentosContext context, IBusca busca)
    {
        _context = context;
        _busca = busca;
    }

    // ============================================================
    // Registro de consumo
    // ============================================================

    public async Task<RegistroConsumoResultadoDto> RegistrarConsumoAsync(string userId, RegistroConsumoDto dto)
    {
        var alimento = await _busca.BuscaAlimentoPorIdAsync(dto.AlimentoId, dto.TipoTabela);
        if (alimento == null)
            throw new InvalidOperationException("Alimento não encontrado.");

        var registro = CriarRegistroAlimentar(userId, alimento, dto);
        _context.RegistroAlimentar.Add(registro);
        await _context.SaveChangesAsync();

        return MapearRegistro(registro);
    }

    public async Task<List<RegistroConsumoResultadoDto>> RegistrarConsumoLoteAsync(
        string userId, RegistroConsumoLoteDto dto)
    {
        var resultados = new List<RegistroConsumoResultadoDto>();

        foreach (var item in dto.Itens)
        {
            var alimento = await _busca.BuscaAlimentoPorIdAsync(item.AlimentoId, item.TipoTabela);
            if (alimento == null) continue;

            var registro = CriarRegistroAlimentar(userId, alimento, item);
            _context.RegistroAlimentar.Add(registro);
            resultados.Add(MapearRegistro(registro));
        }

        await _context.SaveChangesAsync();
        return resultados;
    }

    public async Task<RetornoPadrao> ExcluirRegistroAsync(string userId, long registroId)
    {
        var registro = await _context.RegistroAlimentar
            .FirstOrDefaultAsync(r => r.Id == registroId && r.UserId == userId);
        if (registro == null)
            return new RetornoPadrao { Sucesso = false, Mensagem = "Registro não encontrado." };

        _context.RegistroAlimentar.Remove(registro);
        await _context.SaveChangesAsync();
        return new RetornoPadrao { Sucesso = true, Mensagem = "Registro excluído com sucesso." };
    }

    // ============================================================
    // Fotos de refeição
    // ============================================================

    public async Task<FotoRefeicaoResultadoDto> AdicionarFotoRefeicaoAsync(string userId, FotoRefeicaoDto dto)
    {
        var foto = new FotoRefeicao
        {
            UserId = userId,
            TipoRefeicao = dto.TipoRefeicao,
            FotoUrl = dto.FotoUrl,
            Descricao = dto.Descricao,
            RegistroAlimentarId = dto.RegistroAlimentarId,
            DataRegistro = DateTime.UtcNow
        };

        _context.FotosRefeicao.Add(foto);
        await _context.SaveChangesAsync();

        return new FotoRefeicaoResultadoDto
        {
            Id = foto.Id,
            TipoRefeicao = foto.TipoRefeicao,
            FotoUrl = foto.FotoUrl,
            Descricao = foto.Descricao,
            DataRegistro = foto.DataRegistro
        };
    }

    public async Task<RetornoPadrao> RemoverFotoRefeicaoAsync(string userId, int fotoId)
    {
        var foto = await _context.FotosRefeicao
            .FirstOrDefaultAsync(f => f.Id == fotoId && f.UserId == userId);
        if (foto == null)
            return new RetornoPadrao { Sucesso = false, Mensagem = "Foto não encontrada." };

        _context.FotosRefeicao.Remove(foto);
        await _context.SaveChangesAsync();
        return new RetornoPadrao { Sucesso = true, Mensagem = "Foto removida com sucesso." };
    }

    public async Task<List<FotoRefeicaoResultadoDto>> ListarFotosDoDiaAsync(string userId, DateTime data)
    {
        var dataInicio = data.Date;
        var dataFim = dataInicio.AddDays(1);

        return await _context.FotosRefeicao
            .Where(f => f.UserId == userId && f.DataRegistro >= dataInicio && f.DataRegistro < dataFim)
            .OrderBy(f => f.DataRegistro)
            .Select(f => new FotoRefeicaoResultadoDto
            {
                Id = f.Id,
                TipoRefeicao = f.TipoRefeicao,
                FotoUrl = f.FotoUrl,
                Descricao = f.Descricao,
                DataRegistro = f.DataRegistro
            })
            .ToListAsync();
    }

    // ============================================================
    // Diário do dia (planejado vs consumido)
    // ============================================================

    public async Task<DiarioDiaDto> ObterDiarioDoDiaAsync(string userId, DateTime? data = null)
    {
        var dia = (data ?? DateTime.UtcNow).Date;
        return await MontarDiarioDia(userId, dia);
    }

    public async Task<List<DiarioDiaDto>> ObterDiarioPorPeriodoAsync(
        string userId, DateTime dataInicio, DateTime dataFim)
    {
        var diarios = new List<DiarioDiaDto>();
        for (var dia = dataInicio.Date; dia <= dataFim.Date; dia = dia.AddDays(1))
        {
            diarios.Add(await MontarDiarioDia(userId, dia));
        }
        return diarios;
    }

    // ============================================================
    // Relatório de aderência
    // ============================================================

    public async Task<RelatorioAdesaoDto> GerarRelatorioAdesaoAsync(
        string userId, DateTime dataInicio, DateTime dataFim)
    {
        return await CalcularRelatorio(userId, dataInicio, dataFim);
    }

    public async Task<RelatorioAdesaoDto> GerarRelatorioAdesaoPacienteAsync(
        string profissionalUserId, string pacienteUserId, DateTime dataInicio, DateTime dataFim)
    {
        // Validar vínculo
        var vinculoExiste = await _context.VinculosPacienteProfissional
            .Include(v => v.Profissional)
            .AnyAsync(v =>
                v.Profissional.UserId == profissionalUserId &&
                v.PacienteUserId == pacienteUserId &&
                (v.Status == EStatusVinculo.Ativo || v.Status == EStatusVinculo.Pendente));
        if (!vinculoExiste)
            throw new InvalidOperationException("Profissional não possui vínculo com este paciente.");

        return await CalcularRelatorio(pacienteUserId, dataInicio, dataFim);
    }

    // ============================================================
    // Helpers privados
    // ============================================================

    private async Task<DiarioDiaDto> MontarDiarioDia(string userId, DateTime dia)
    {
        //DateTimeHelper.EnsureUtcDateTime
        var diaInicio = DateTimeHelper.EnsureUtcDateTime(dia.Date);
        var diaFim = DateTimeHelper.EnsureUtcDateTime(diaInicio.AddDays(1));

        // Buscar registros alimentares do dia
        var registros = await _context.RegistroAlimentar
            .Where(r => r.UserId == userId && r.DataConsumo >= diaInicio && r.DataConsumo < diaFim)
            .Include(r => r.ItemRefeicaoPlano)
            .OrderBy(r => r.DataConsumo)
            .ToListAsync();

        // Buscar fotos do dia
        var fotos = await _context.FotosRefeicao
            .Where(f => f.UserId == userId && f.DataRegistro >= diaInicio && f.DataRegistro < diaFim)
            .OrderBy(f => f.DataRegistro)
            .ToListAsync();

        // Buscar plano ativo e suas refeições
        var perfil = await _context.PerfilNutricional
            .FirstOrDefaultAsync(p => p.UserId == userId);

        PlanoAlimentar? planoAtivo = null;
        if (perfil != null)
        {
            planoAtivo = await _context.PlanosAlimentares
                .Include(p => p.RefeicoesPlanejadas).ThenInclude(r => r.Itens)
                .FirstOrDefaultAsync(p => p.PerfilNutricionalId == perfil.Id && p.Status == EStatusPlano.Ativo);
        }

        // Buscar meta nutricional como fallback
        MetaNutricional? meta = null;
        if (perfil != null)
        {
            meta = await _context.MetasNutricionais
                .FirstOrDefaultAsync(m => m.PerfilNutricionalId == perfil.Id);
        }

        // Montar metas do dia
        var metasDoDia = new MacrosDiariosPlanoDto
        {
            CaloriasKcal = planoAtivo?.CaloriasAlvoDiarias ?? meta?.CaloriasDiarias ?? 2000,
            ProteinaG = planoAtivo?.ProteinaAlvoG ?? meta?.ProteinasDiarias ?? 120,
            CarboidratoG = planoAtivo?.CarboidratoAlvoG ?? meta?.CarboidratosDiarios ?? 250,
            GorduraG = planoAtivo?.GorduraAlvoG ?? meta?.GordurasDiarias ?? 65,
            FibraG = planoAtivo?.FibraAlvoG ?? meta?.FibraDiaria ?? 25,
            AguaL = planoAtivo?.AguaAlvoL ?? meta?.AguaDiaria ?? 2.0
        };

        // Calcular totais consumidos
        var totalConsumido = new MacrosDiariosPlanoDto
        {
            CaloriasKcal = Math.Round(registros.Sum(r => r.EnergiaKcalTotal), 1),
            ProteinaG = Math.Round(registros.Sum(r => r.ProteinaTotal), 1),
            CarboidratoG = Math.Round(registros.Sum(r => r.CarboTotal), 1),
            GorduraG = Math.Round(registros.Sum(r => r.GorduraTotal), 1),
            FibraG = Math.Round(registros.Sum(r => r.FibraTotal), 1),
            AguaL = Math.Round(registros.Sum(r => r.AguaTotal), 2)
        };

        // Montar refeições comparativas
        var tiposRefeicao = System.Enum.GetValues<ETipoRefeicao>();
        var refeicoes = new List<RefeicaoDiarioDto>();

        foreach (var tipo in tiposRefeicao)
        {
            var registrosTipo = registros.Where(r => r.Refeicao == tipo).ToList();
            var refeicaoPlanejada = planoAtivo?.RefeicoesPlanejadas
                .FirstOrDefault(r => r.TipoRefeicao == tipo);

            var refeicaoDiario = new RefeicaoDiarioDto
            {
                TipoRefeicao = tipo,
                HorarioPlanejado = refeicaoPlanejada?.HorarioSugerido,
                Consumido = new MacroRefeicaoDto
                {
                    EnergiaKcal = Math.Round(registrosTipo.Sum(r => r.EnergiaKcalTotal), 1),
                    ProteinaG = Math.Round(registrosTipo.Sum(r => r.ProteinaTotal), 1),
                    CarboidratoG = Math.Round(registrosTipo.Sum(r => r.CarboTotal), 1),
                    GorduraG = Math.Round(registrosTipo.Sum(r => r.GorduraTotal), 1),
                    FibraG = Math.Round(registrosTipo.Sum(r => r.FibraTotal), 1)
                },
                Registros = registrosTipo.Select(r => MapearRegistro(r)).ToList()
            };

            if (refeicaoPlanejada != null)
            {
                refeicaoDiario.Planejado = new MacroRefeicaoDto
                {
                    EnergiaKcal = refeicaoPlanejada.TotalEnergiaKcal,
                    ProteinaG = refeicaoPlanejada.TotalProteinaG,
                    CarboidratoG = refeicaoPlanejada.TotalCarboidratoG,
                    GorduraG = refeicaoPlanejada.TotalGorduraG,
                    FibraG = refeicaoPlanejada.TotalFibraG
                };

                refeicaoDiario.PercentualAderencia = refeicaoPlanejada.TotalEnergiaKcal > 0
                    ? Math.Round(refeicaoDiario.Consumido.EnergiaKcal / refeicaoPlanejada.TotalEnergiaKcal * 100, 1)
                    : null;
            }

            // Incluir apenas se tem registros ou planejado
            if (registrosTipo.Any() || refeicaoPlanejada != null)
                refeicoes.Add(refeicaoDiario);
        }

        double aderenciaCaloricas = metasDoDia.CaloriasKcal > 0
            ? Math.Round(totalConsumido.CaloriasKcal / metasDoDia.CaloriasKcal * 100, 1)
            : 0;

        return new DiarioDiaDto
        {
            Data = dia,
            MetasDoDia = metasDoDia,
            TotalConsumido = totalConsumido,
            SaldoRestante = new MacrosDiariosPlanoDto
            {
                CaloriasKcal = Math.Round(metasDoDia.CaloriasKcal - totalConsumido.CaloriasKcal, 1),
                ProteinaG = Math.Round(metasDoDia.ProteinaG - totalConsumido.ProteinaG, 1),
                CarboidratoG = Math.Round(metasDoDia.CarboidratoG - totalConsumido.CarboidratoG, 1),
                GorduraG = Math.Round(metasDoDia.GorduraG - totalConsumido.GorduraG, 1),
                FibraG = Math.Round(metasDoDia.FibraG - totalConsumido.FibraG, 1),
                AguaL = Math.Round(metasDoDia.AguaL - totalConsumido.AguaL, 2)
            },
            PercentualAderenciaCaloricas = aderenciaCaloricas,
            Refeicoes = refeicoes,
            Fotos = fotos.Select(f => new FotoRefeicaoResultadoDto
            {
                Id = f.Id,
                TipoRefeicao = f.TipoRefeicao,
                FotoUrl = f.FotoUrl,
                Descricao = f.Descricao,
                DataRegistro = f.DataRegistro
            }).ToList()
        };
    }

    private async Task<RelatorioAdesaoDto> CalcularRelatorio(
        string userId, DateTime dataInicio, DateTime dataFim)
    {
        var inicio = dataInicio.Date;
        var fim = dataFim.Date.AddDays(1);
        int totalDias = (int)(dataFim.Date - dataInicio.Date).TotalDays + 1;

        // Buscar todos os registros no período
        var registros = await _context.RegistroAlimentar
            .Where(r => r.UserId == userId && r.DataConsumo >= inicio && r.DataConsumo < fim)
            .ToListAsync();

        // Buscar perfil e meta
        var perfil = await _context.PerfilNutricional
            .FirstOrDefaultAsync(p => p.UserId == userId);

        PlanoAlimentar? planoAtivo = null;
        MetaNutricional? meta = null;
        if (perfil != null)
        {
            planoAtivo = await _context.PlanosAlimentares
                .FirstOrDefaultAsync(p => p.PerfilNutricionalId == perfil.Id && p.Status == EStatusPlano.Ativo);
            meta = await _context.MetasNutricionais
                .FirstOrDefaultAsync(m => m.PerfilNutricionalId == perfil.Id);
        }

        double metaCal = planoAtivo?.CaloriasAlvoDiarias ?? meta?.CaloriasDiarias ?? 2000;
        double metaProt = planoAtivo?.ProteinaAlvoG ?? meta?.ProteinasDiarias ?? 120;
        double metaCarb = planoAtivo?.CarboidratoAlvoG ?? meta?.CarboidratosDiarios ?? 250;
        double metaGord = planoAtivo?.GorduraAlvoG ?? meta?.GordurasDiarias ?? 65;
        double metaFibra = planoAtivo?.FibraAlvoG ?? meta?.FibraDiaria ?? 25;
        double metaAgua = planoAtivo?.AguaAlvoL ?? meta?.AguaDiaria ?? 2.0;

        // Agrupar por dia
        var registrosPorDia = registros
            .GroupBy(r => r.DataConsumo.Date)
            .ToDictionary(g => g.Key, g => g.ToList());

        int diasComRegistro = registrosPorDia.Count;

        // Calcular médias
        double totalCalConsumido = registros.Sum(r => r.EnergiaKcalTotal);
        double totalProtConsumido = registros.Sum(r => r.ProteinaTotal);
        double totalCarbConsumido = registros.Sum(r => r.CarboTotal);
        double totalGordConsumido = registros.Sum(r => r.GorduraTotal);
        double totalFibraConsumido = registros.Sum(r => r.FibraTotal);
        double totalAguaConsumido = registros.Sum(r => r.AguaTotal);

        double mediaCal = diasComRegistro > 0 ? totalCalConsumido / diasComRegistro : 0;
        double mediaProt = diasComRegistro > 0 ? totalProtConsumido / diasComRegistro : 0;
        double mediaCarb = diasComRegistro > 0 ? totalCarbConsumido / diasComRegistro : 0;
        double mediaGord = diasComRegistro > 0 ? totalGordConsumido / diasComRegistro : 0;
        double mediaFibra = diasComRegistro > 0 ? totalFibraConsumido / diasComRegistro : 0;
        double mediaAgua = diasComRegistro > 0 ? totalAguaConsumido / diasComRegistro : 0;

        // Aderência por refeição
        var tiposRefeicao = System.Enum.GetValues<ETipoRefeicao>();
        var aderenciaPorRefeicao = tiposRefeicao.Select(tipo =>
        {
            var registrosTipo = registros.Where(r => r.Refeicao == tipo).ToList();
            double totalCalTipo = registrosTipo.Sum(r => r.EnergiaKcalTotal);
            int diasComTipo = registrosTipo.GroupBy(r => r.DataConsumo.Date).Count();
            double mediaCalDia = diasComTipo > 0 ? totalCalTipo / diasComTipo : 0;

            // Estimar meta por refeição (dividir igualmente se não temos dados específicos)
            double metaPorRefeicao = metaCal / tiposRefeicao.Length;

            return new AderenciaPorRefeicaoDto
            {
                TipoRefeicao = tipo,
                AderenciaMediaPercent = metaPorRefeicao > 0
                    ? Math.Round(mediaCalDia / metaPorRefeicao * 100, 1)
                    : 0,
                TotalRegistros = registrosTipo.Count
            };
        }).Where(a => a.TotalRegistros > 0).ToList();

        // Histórico diário
        var historicoDiario = new List<AderenciaDiariaDto>();
        for (var dia = dataInicio.Date; dia <= dataFim.Date; dia = dia.AddDays(1))
        {
            var registrosDia = registrosPorDia.GetValueOrDefault(dia, new List<RegistroAlimentar>());
            double calDia = registrosDia.Sum(r => r.EnergiaKcalTotal);
            historicoDiario.Add(new AderenciaDiariaDto
            {
                Data = dia,
                CaloriasConsumidas = Math.Round(calDia, 1),
                CaloriasMeta = metaCal,
                AderenciaPercent = metaCal > 0 ? Math.Round(calDia / metaCal * 100, 1) : 0,
                TotalRegistros = registrosDia.Count
            });
        }

        return new RelatorioAdesaoDto
        {
            DataInicio = dataInicio.Date,
            DataFim = dataFim.Date,
            TotalDias = totalDias,
            DiasComRegistro = diasComRegistro,
            AderenciaCaloricoMediaPercent = metaCal > 0 ? Math.Round(mediaCal / metaCal * 100, 1) : 0,
            AderenciaProteinaMediaPercent = metaProt > 0 ? Math.Round(mediaProt / metaProt * 100, 1) : 0,
            AderenciaCarboidratoMediaPercent = metaCarb > 0 ? Math.Round(mediaCarb / metaCarb * 100, 1) : 0,
            AderenciaGorduraMediaPercent = metaGord > 0 ? Math.Round(mediaGord / metaGord * 100, 1) : 0,
            MediaDiariaConsumida = new MacrosDiariosPlanoDto
            {
                CaloriasKcal = Math.Round(mediaCal, 1),
                ProteinaG = Math.Round(mediaProt, 1),
                CarboidratoG = Math.Round(mediaCarb, 1),
                GorduraG = Math.Round(mediaGord, 1),
                FibraG = Math.Round(mediaFibra, 1),
                AguaL = Math.Round(mediaAgua, 2)
            },
            MetaDiaria = new MacrosDiariosPlanoDto
            {
                CaloriasKcal = metaCal,
                ProteinaG = metaProt,
                CarboidratoG = metaCarb,
                GorduraG = metaGord,
                FibraG = metaFibra,
                AguaL = metaAgua
            },
            AderenciaPorRefeicao = aderenciaPorRefeicao,
            HistoricoDiario = historicoDiario
        };
    }

    private RegistroAlimentar CriarRegistroAlimentar(
        string userId, AlimentoResumoDto alimento, RegistroConsumoDto dto)
    {
        double porcaoRef = alimento.PorcaoReferencia > 0 ? alimento.PorcaoReferencia : 100.0;
        double fator = dto.QuantidadeConsumidaG / porcaoRef;

        var dataConsumo = DateTimeHelper.EnsureUtcDateTime(dto.DataConsumo); // Garantir que a data de consumo seja UTC

        return new RegistroAlimentar
        {
            UserId = userId,
            AlimentoIdOrigem = dto.AlimentoId,
            NomeAlimentoSnapshot = alimento.Nome,
            TipoTabela = dto.TipoTabela,
            QuantidadeConsumidaG = dto.QuantidadeConsumidaG,
            DataConsumo = dataConsumo,
            Refeicao = dto.TipoRefeicao,
            EnergiaKcalTotal = Math.Round(alimento.Macros.EnergiaKcal * fator, 1),
            ProteinaTotal = Math.Round(alimento.Macros.Proteina * fator, 1),
            CarboTotal = Math.Round(alimento.Macros.CarboDisponivel * fator, 1),
            GorduraTotal = Math.Round(alimento.Macros.LipidiosG * fator, 1),
            FibraTotal = Math.Round(alimento.Macros.Fibras * fator, 1),
            AguaTotal = Math.Round(alimento.Macros.Umidade * fator, 2),
            DadosNutricionaisCompletosJson = JsonSerializer.Serialize(alimento),
            PlanoAlimentarId = null, // Vinculado ao plano ativo se existir
            ItemRefeicaoPlanoId = dto.ItemRefeicaoPlanoId,
            CodigoBarras = dto.CodigoBarras
        };
    }

    private static RegistroConsumoResultadoDto MapearRegistro(RegistroAlimentar r)
    {
        return new RegistroConsumoResultadoDto
        {
            Id = r.Id,
            AlimentoId = r.AlimentoIdOrigem,
            TipoTabela = r.TipoTabela,
            NomeAlimento = r.NomeAlimentoSnapshot,
            QuantidadeConsumidaG = r.QuantidadeConsumidaG,
            DataConsumo = r.DataConsumo,
            Refeicao = r.Refeicao,
            EnergiaKcal = r.EnergiaKcalTotal,
            ProteinaG = r.ProteinaTotal,
            CarboidratoG = r.CarboTotal,
            GorduraG = r.GorduraTotal,
            FibraG = r.FibraTotal,
            CodigoBarras = r.CodigoBarras,
            ItemPlanoVinculado = r.ItemRefeicaoPlano?.NomeAlimentoSnapshot
        };
    }
}
