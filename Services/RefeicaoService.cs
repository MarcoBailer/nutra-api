using Microsoft.EntityFrameworkCore;
using Nutra.Data;
using Nutra.Enum;
using Nutra.Interfaces;
using Nutra.Models;
using Nutra.Models.Dtos;
using Nutra.Models.RegraNutricional;
using System.Text.Json;

namespace Nutra.Services;

public class RefeicaoService : IRefeicao
{
    private readonly AlimentosContext _context;
    private readonly IBusca _buscaService;

    public RefeicaoService(AlimentosContext context, IBusca buscaService)
    {
        _context = context;
        _buscaService = buscaService;
    }

    public async Task<RetornoPadrao> RegistrarConsumoAsync(string userId, int alimentoId, ETipoTabela tabela, double quantidadeIngeridaG, ETipoRefeicao nomeRefeicao)
    {
        var alimentoInfo = await _buscaService.BuscaAlimentoPorIdAsync(alimentoId, tabela);
        
        var retorno = new RetornoPadrao();

        if (alimentoInfo == null) throw new Exception("Alimento não encontrado");

        double baseReferencia = alimentoInfo.PorcaoReferencia > 0 ? alimentoInfo.PorcaoReferencia : 100.0;
        double fator = quantidadeIngeridaG / baseReferencia;

        var novoRegistro = new RegistroAlimentar
        {
            UserId = userId,
            AlimentoIdOrigem = alimentoId,
            TipoTabela = tabela,
            NomeAlimentoSnapshot = alimentoInfo.Nome,
            QuantidadeConsumidaG = quantidadeIngeridaG,
            DataConsumo = DateTime.UtcNow,
            Refeicao = nomeRefeicao,

            EnergiaKcalTotal = alimentoInfo.Macros.EnergiaKcal * fator,
            ProteinaTotal = alimentoInfo.Macros.Proteina * fator,
            CarboTotal = alimentoInfo.Macros.CarboTotal * fator,
            GorduraTotal = alimentoInfo.Gorduras.Totais * fator,
            FibraTotal = alimentoInfo.Macros.Fibras * fator,
            AguaTotal = alimentoInfo.Macros.Umidade * fator,

            // TODO: lembrar de aplicar o 'fator' nas vitaminas.
            DadosNutricionaisCompletosJson = JsonSerializer.Serialize(alimentoInfo)
        };

        _context.RegistroAlimentar.Add(novoRegistro);
        await _context.SaveChangesAsync();

        retorno.Sucesso = true;
        retorno.Mensagem = "Consumo registrado com sucesso.";
        return retorno;
    }

    public async Task<StatusDiarioDto> ObterStatusDiario(string userId)
    {
        var hoje = DateTime.UtcNow.Date;

        var consumidoHoje = await _context.RegistroAlimentar
            .Where(r => r.UserId == userId && r.DataConsumo.Date == hoje)
            .GroupBy(r => r.UserId)
            .Select(g => new
            {
                Calorias = g.Sum(x => x.EnergiaKcalTotal),
                Proteinas = g.Sum(x => x.ProteinaTotal),
                Agua = g.Sum(x => x.AguaTotal)
            })
            .FirstOrDefaultAsync();

        var perfil = await _context.PerfilNutricional
            .Include(p => p.MetaNutricional)
            .FirstOrDefaultAsync(p => p.UserId == userId);

        var meta = perfil.MetaNutricional;

        return new StatusDiarioDto
        {
            CaloriasConsumidas = consumidoHoje?.Calorias ?? 0,
            CaloriasMeta = meta.CaloriasDiarias,
            SaldoCalorico = (meta.CaloriasDiarias - (consumidoHoje?.Calorias ?? 0)),

            AguaConsumida = consumidoHoje?.Agua ?? 0,
            AguaMeta = meta.AguaDiaria
        };
    }
}