using Microsoft.EntityFrameworkCore;
using Nutra.Data;
using Nutra.Enum;
using Nutra.Interfaces;
using Nutra.Models;
using Nutra.Models.Dtos;
using Nutra.Models.RegraNutricional;
using Nutra.Models.Usuario;

namespace Nutra.Services;

public class PlanoAlimentarService : IPlanoAlimentar
{
    private readonly AlimentosContext _context;
    private readonly IBusca _busca;

    public PlanoAlimentarService(AlimentosContext context, IBusca busca)
    {
        _context = context;
        _busca = busca;
    }

    // ============================================================
    // CRUD Plano
    // ============================================================

    public async Task<PlanoAlimentarResultadoDto> CriarPlanoAsync(string userId, CriarPlanoAlimentarDto dto)
    {
        var perfil = await ObterPerfilOuErro(userId);
        var plano = await ConstruirPlano(perfil, dto, profissionalId: null);
        _context.PlanosAlimentares.Add(plano);
        await _context.SaveChangesAsync();
        return await MapearPlanoResultado(plano);
    }

    public async Task<PlanoAlimentarResultadoDto> CriarPlanoPorProfissionalAsync(
        string profissionalUserId, CriarPlanoProfissionalDto dto)
    {
        await ValidarVinculoProfissional(profissionalUserId, dto.PacienteUserId);
        var perfil = await ObterPerfilOuErro(dto.PacienteUserId);
        var plano = await ConstruirPlano(perfil, dto, profissionalUserId);
        _context.PlanosAlimentares.Add(plano);
        await _context.SaveChangesAsync();
        return await MapearPlanoResultado(plano);
    }

    public async Task<PlanoAlimentarResultadoDto> ObterPlanoAsync(string userId, int planoId)
    {
        var plano = await CarregarPlanoCompleto()
            .FirstOrDefaultAsync(p => p.Id == planoId && p.PerfilNutricional.UserId == userId);
        if (plano == null) 
            //TODO:
            //Nao devemos jogar excecao
            //retornar status codigo, bool e mensagem
            //aqui o client que consome decide se redireciona ou nao
            throw new InvalidOperationException("Plano alimentar não encontrado.");
        return await MapearPlanoResultado(plano);
    }

    public async Task<PlanoAlimentarResultadoDto?> ObterPlanoAtivoAsync(string userId)
    {
        var plano = await CarregarPlanoCompleto()
            .FirstOrDefaultAsync(p => p.PerfilNutricional.UserId == userId && p.Status == EStatusPlano.Ativo);
        return plano == null ? null : await MapearPlanoResultado(plano);
    }

    public async Task<List<PlanoAlimentarResumoDto>> ListarPlanosAsync(string userId)
    {
        var perfil = await ObterPerfilOuErro(userId);
        return await _context.PlanosAlimentares
            .Where(p => p.PerfilNutricionalId == perfil.Id)
            .OrderByDescending(p => p.CriadoEm)
            .Select(p => new PlanoAlimentarResumoDto
            {
                Id = p.Id,
                Nome = p.Nome,
                Status = p.Status,
                DataInicio = p.DataInicio,
                DataFim = p.DataFim,
                CaloriasAlvoDiarias = p.CaloriasAlvoDiarias,
                TotalRefeicoes = p.RefeicoesPlanejadas.Count,
                TotalItens = p.RefeicoesPlanejadas.SelectMany(r => r.Itens).Count(),
                ProfissionalResponsavel = p.ProfissionalResponsavel != null ? p.ProfissionalResponsavel.NomeCompleto : null,
                CriadoEm = p.CriadoEm
            })
            .ToListAsync();
    }

    public async Task<PlanoAlimentarResultadoDto> AtualizarPlanoAsync(
        string userId, int planoId, AtualizarPlanoAlimentarDto dto)
    {
        var perfil = await ObterPerfilOuErro(userId);
        var plano = await CarregarPlanoCompleto()
            .FirstOrDefaultAsync(p => p.Id == planoId && p.PerfilNutricionalId == perfil.Id);
        if (plano == null) 
            //TODO:
            //Nao devemos jogar excecao
            //retornar status codigo, bool e mensagem
            //aqui o client que consome decide se redireciona ou nao
            throw new InvalidOperationException("Plano alimentar não encontrado.");

        if (dto.Nome != null) plano.Nome = dto.Nome;
        if (dto.Descricao != null) plano.Descricao = dto.Descricao;
        if (dto.DataFim.HasValue) plano.DataFim = dto.DataFim.Value;
        if (dto.Status.HasValue) plano.Status = dto.Status.Value;
        if (dto.Observacoes != null) plano.Observacoes = dto.Observacoes;
        plano.AtualizadoEm = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return await MapearPlanoResultado(plano);
    }

    public async Task<RetornoPadrao> ExcluirPlanoAsync(string userId, int planoId)
    {
        var perfil = await ObterPerfilOuErro(userId);
        var plano = await _context.PlanosAlimentares
            .Include(p => p.RefeicoesPlanejadas)
                .ThenInclude(r => r.Itens)
                    .ThenInclude(i => i.SubstituicoesEquivalentes)
            .FirstOrDefaultAsync(p => p.Id == planoId && p.PerfilNutricionalId == perfil.Id);
        if (plano == null)
            return new RetornoPadrao { Sucesso = false, Mensagem = "Plano alimentar não encontrado." };

        _context.PlanosAlimentares.Remove(plano);
        await _context.SaveChangesAsync();
        return new RetornoPadrao { Sucesso = true, Mensagem = "Plano alimentar excluído com sucesso." };
    }

    public async Task<RetornoPadrao> AtivarPlanoAsync(string userId, int planoId)
    {
        var perfil = await ObterPerfilOuErro(userId);

        // Desativar plano ativo anterior
        var planosAtivos = await _context.PlanosAlimentares
            .Where(p => p.PerfilNutricionalId == perfil.Id && p.Status == EStatusPlano.Ativo)
            .ToListAsync();
        foreach (var ativo in planosAtivos)
        {
            ativo.Status = EStatusPlano.Pausado;
            ativo.AtualizadoEm = DateTime.UtcNow;
        }

        var plano = await _context.PlanosAlimentares
            .FirstOrDefaultAsync(p => p.Id == planoId && p.PerfilNutricionalId == perfil.Id);
        if (plano == null)
            return new RetornoPadrao { Sucesso = false, Mensagem = "Plano alimentar não encontrado." };

        plano.Status = EStatusPlano.Ativo;
        plano.AtualizadoEm = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return new RetornoPadrao { Sucesso = true, Mensagem = "Plano alimentar ativado com sucesso." };
    }

    // ============================================================
    // Refeições
    // ============================================================

    public async Task<PlanoAlimentarResultadoDto> AdicionarRefeicaoAsync(
        string userId, int planoId, AdicionarRefeicaoDto dto)
    {
        var perfil = await ObterPerfilOuErro(userId);
        var plano = await CarregarPlanoCompleto()
            .FirstOrDefaultAsync(p => p.Id == planoId && p.PerfilNutricionalId == perfil.Id);
        if (plano == null)
            //TODO:
            //Nao devemos jogar excecao
            //retornar status codigo, bool e mensagem
            //aqui o client que consome decide se redireciona ou nao
            throw new InvalidOperationException("Plano alimentar não encontrado.");

        var refeicao = new RefeicaoPlano
        {
            TipoRefeicao = dto.TipoRefeicao,
            HorarioSugerido = dto.HorarioSugerido,
            Ordem = dto.Ordem,
            Observacoes = dto.Observacoes
        };

        if (dto.Itens?.Any() == true)
        {
            foreach (var itemDto in dto.Itens)
            {
                var item = await ConstruirItem(itemDto);
                refeicao.Itens.Add(item);
            }
            RecalcularTotaisRefeicao(refeicao);
        }

        plano.RefeicoesPlanejadas.Add(refeicao);
        plano.AtualizadoEm = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return await MapearPlanoResultado(plano);
    }

    public async Task<RetornoPadrao> RemoverRefeicaoAsync(string userId, int refeicaoId)
    {
        var perfil = await ObterPerfilOuErro(userId);
        var refeicao = await _context.Set<RefeicaoPlano>()
            .Include(r => r.PlanoAlimentar)
            .Include(r => r.Itens).ThenInclude(i => i.SubstituicoesEquivalentes)
            .FirstOrDefaultAsync(r => r.Id == refeicaoId && r.PlanoAlimentar.PerfilNutricionalId == perfil.Id);
        if (refeicao == null)
            return new RetornoPadrao { Sucesso = false, Mensagem = "Refeição não encontrada." };

        _context.Set<RefeicaoPlano>().Remove(refeicao);
        refeicao.PlanoAlimentar.AtualizadoEm = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return new RetornoPadrao { Sucesso = true, Mensagem = "Refeição removida com sucesso." };
    }

    // ============================================================
    // Itens
    // ============================================================

    public async Task<PlanoAlimentarResultadoDto> AdicionarItemAsync(
        string userId, int refeicaoId, AdicionarItemDto dto)
    {
        var perfil = await ObterPerfilOuErro(userId);
        var refeicao = await _context.Set<RefeicaoPlano>()
            .Include(r => r.PlanoAlimentar)
            .Include(r => r.Itens)
            .FirstOrDefaultAsync(r => r.Id == refeicaoId && r.PlanoAlimentar.PerfilNutricionalId == perfil.Id);
        if (refeicao == null)
            //TODO:
            //Nao devemos jogar excecao
            //retornar status codigo, bool e mensagem
            //aqui o client que consome decide se redireciona ou nao
            throw new InvalidOperationException("Refeição não encontrada.");

        var alimento = await _busca.BuscaAlimentoPorIdAsync(dto.AlimentoId, dto.TipoTabela);
        if (alimento == null) 
            //TODO:
            //Nao devemos jogar excecao
            //retornar status codigo, bool e mensagem
            //aqui o client que consome decide se redireciona ou nao
            throw new InvalidOperationException("Alimento não encontrado.");

        var item = CriarItemComMacros(alimento, dto.AlimentoId, dto.TipoTabela, dto.QuantidadeG, dto.Ordem, dto.Observacoes);
        refeicao.Itens.Add(item);
        RecalcularTotaisRefeicao(refeicao);
        refeicao.PlanoAlimentar.AtualizadoEm = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        var plano = await CarregarPlanoCompleto()
            .FirstAsync(p => p.Id == refeicao.PlanoAlimentarId);
        return await MapearPlanoResultado(plano);
    }

    public async Task<RetornoPadrao> RemoverItemAsync(string userId, int itemId)
    {
        var perfil = await ObterPerfilOuErro(userId);
        var item = await _context.Set<ItemRefeicao>()
            .Include(i => i.RefeicaoPlano).ThenInclude(r => r.PlanoAlimentar)
            .Include(i => i.SubstituicoesEquivalentes)
            .FirstOrDefaultAsync(i => i.Id == itemId && i.RefeicaoPlano.PlanoAlimentar.PerfilNutricionalId == perfil.Id);
        if (item == null)
            return new RetornoPadrao { Sucesso = false, Mensagem = "Item não encontrado." };

        var refeicao = item.RefeicaoPlano;
        _context.Set<ItemRefeicao>().Remove(item);
        refeicao.PlanoAlimentar.AtualizadoEm = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        // Recalcular totais da refeição sem o item removido
        var refeicaoAtualizada = await _context.Set<RefeicaoPlano>()
            .Include(r => r.Itens)
            .FirstOrDefaultAsync(r => r.Id == refeicao.Id);
        if (refeicaoAtualizada != null) RecalcularTotaisRefeicao(refeicaoAtualizada);
        await _context.SaveChangesAsync();

        return new RetornoPadrao { Sucesso = true, Mensagem = "Item removido com sucesso." };
    }

    // ============================================================
    // Substituições Equivalentes
    // ============================================================

    public async Task<RetornoPadrao> AdicionarSubstituicaoAsync(
        string userId, int itemId, AdicionarSubstituicaoDto dto)
    {
        var perfil = await ObterPerfilOuErro(userId);
        var item = await _context.Set<ItemRefeicao>()
            .Include(i => i.RefeicaoPlano).ThenInclude(r => r.PlanoAlimentar)
            .Include(i => i.SubstituicoesEquivalentes)
            .FirstOrDefaultAsync(i => i.Id == itemId && i.RefeicaoPlano.PlanoAlimentar.PerfilNutricionalId == perfil.Id);
        if (item == null)
            return new RetornoPadrao { Sucesso = false, Mensagem = "Item não encontrado." };

        var alimento = await _busca.BuscaAlimentoPorIdAsync(dto.AlimentoId, dto.TipoTabela);
        if (alimento == null)
            return new RetornoPadrao { Sucesso = false, Mensagem = "Alimento substituto não encontrado." };

        var sub = CriarSubstituicaoComMacros(alimento, dto.AlimentoId, dto.TipoTabela, dto.QuantidadeG);
        item.SubstituicoesEquivalentes.Add(sub);
        item.RefeicaoPlano.PlanoAlimentar.AtualizadoEm = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return new RetornoPadrao { Sucesso = true, Mensagem = "Substituição adicionada com sucesso." };
    }

    public async Task<RetornoPadrao> RemoverSubstituicaoAsync(string userId, int substituicaoId)
    {
        var perfil = await ObterPerfilOuErro(userId);
        var sub = await _context.Set<SubstituicaoEquivalente>()
            .Include(s => s.ItemRefeicao)
                .ThenInclude(i => i.RefeicaoPlano)
                    .ThenInclude(r => r.PlanoAlimentar)
            .FirstOrDefaultAsync(s => s.Id == substituicaoId &&
                s.ItemRefeicao.RefeicaoPlano.PlanoAlimentar.PerfilNutricionalId == perfil.Id);
        if (sub == null)
            return new RetornoPadrao { Sucesso = false, Mensagem = "Substituição não encontrada." };

        _context.Set<SubstituicaoEquivalente>().Remove(sub);
        sub.ItemRefeicao.RefeicaoPlano.PlanoAlimentar.AtualizadoEm = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return new RetornoPadrao { Sucesso = true, Mensagem = "Substituição removida com sucesso." };
    }

    // ============================================================
    // Modelos de Dieta (Templates)
    // ============================================================

    public async Task<ModeloDietaResultadoDto> CriarModeloDietaAsync(
        string profissionalUserId, CriarModeloDietaDto dto)
    {
        var modelo = new ModeloDieta
        {
            Nome = dto.Nome,
            Descricao = dto.Descricao,
            ObjetivoAlvo = dto.ObjetivoAlvo,
            PreferenciaAlimentarAlvo = dto.PreferenciaAlimentarAlvo,
            NumeroRefeicoesDia = dto.NumeroRefeicoesDia,
            Publico = dto.Publico,
            CriadoPorProfissionalId = profissionalUserId
        };

        double totalCal = 0, totalProt = 0, totalCarb = 0, totalGord = 0;

        foreach (var refDto in dto.Refeicoes)
        {
            var refeicao = new RefeicaoModeloDieta
            {
                TipoRefeicao = refDto.TipoRefeicao,
                HorarioSugerido = refDto.HorarioSugerido,
                Ordem = refDto.Ordem,
                PercentualCaloricoSugerido = refDto.PercentualCaloricoSugerido,
                Observacoes = refDto.Observacoes
            };

            foreach (var itemDto in refDto.Itens)
            {
                var alimento = await _busca.BuscaAlimentoPorIdAsync(itemDto.AlimentoId, itemDto.TipoTabela);
                if (alimento == null) continue;

                var macros = CalcularMacrosProporcional(alimento, itemDto.QuantidadeG);
                var item = new ItemModeloDieta
                {
                    AlimentoId = itemDto.AlimentoId,
                    TipoTabela = itemDto.TipoTabela,
                    NomeAlimentoSnapshot = alimento.Nome,
                    QuantidadeG = itemDto.QuantidadeG,
                    EnergiaKcal = macros.kcal,
                    ProteinaG = macros.prot,
                    CarboidratoG = macros.carb,
                    GorduraG = macros.gord,
                    FibraG = macros.fibra,
                    Ordem = itemDto.Ordem
                };

                totalCal += macros.kcal;
                totalProt += macros.prot;
                totalCarb += macros.carb;
                totalGord += macros.gord;
                refeicao.Itens.Add(item);
            }

            modelo.Refeicoes.Add(refeicao);
        }

        modelo.CaloriasBase = Math.Round(totalCal, 1);
        modelo.ProteinaBaseG = Math.Round(totalProt, 1);
        modelo.CarboidratoBaseG = Math.Round(totalCarb, 1);
        modelo.GorduraBaseG = Math.Round(totalGord, 1);

        _context.ModelosDieta.Add(modelo);
        await _context.SaveChangesAsync();
        return MapearModeloResultado(modelo);
    }

    public async Task<List<ModeloDietaResumoDto>> ListarModelosDietaAsync(string? profissionalUserId)
    {
        var query = _context.ModelosDieta.Where(m => m.Ativo);

        if (profissionalUserId != null)
            query = query.Where(m => m.Publico || m.CriadoPorProfissionalId == profissionalUserId);
        else
            query = query.Where(m => m.Publico);

        return await query
            .OrderBy(m => m.Nome)
            .Select(m => new ModeloDietaResumoDto
            {
                Id = m.Id,
                Nome = m.Nome,
                Descricao = m.Descricao,
                ObjetivoAlvo = m.ObjetivoAlvo,
                PreferenciaAlimentarAlvo = m.PreferenciaAlimentarAlvo,
                CaloriasBase = m.CaloriasBase,
                NumeroRefeicoesDia = m.NumeroRefeicoesDia,
                Publico = m.Publico
            })
            .ToListAsync();
    }

    public async Task<ModeloDietaResultadoDto> ObterModeloDietaAsync(int modeloId)
    {
        var modelo = await _context.ModelosDieta
            .Include(m => m.Refeicoes).ThenInclude(r => r.Itens)
            .Include(m => m.CriadoPorProfissional)
            .FirstOrDefaultAsync(m => m.Id == modeloId && m.Ativo);
        if (modelo == null) 
            //TODO:
            //Nao devemos jogar excecao
            //retornar status codigo, bool e mensagem
            //aqui o client que consome decide se redireciona ou nao
            throw new InvalidOperationException("Modelo de dieta não encontrado.");
        return MapearModeloResultado(modelo);
    }

    public async Task<RetornoPadrao> ExcluirModeloDietaAsync(string profissionalUserId, int modeloId)
    {
        var modelo = await _context.ModelosDieta
            .FirstOrDefaultAsync(m => m.Id == modeloId && m.CriadoPorProfissionalId == profissionalUserId);
        if (modelo == null)
            return new RetornoPadrao { Sucesso = false, Mensagem = "Modelo de dieta não encontrado." };

        modelo.Ativo = false;
        modelo.AtualizadoEm = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return new RetornoPadrao { Sucesso = true, Mensagem = "Modelo de dieta excluído com sucesso." };
    }

    public async Task<PlanoAlimentarResultadoDto> CriarPlanoAPartirDeModeloAsync(
        string userId, int modeloId, DateTime dataInicio, DateTime? dataFim)
    {
        var perfil = await ObterPerfilOuErro(userId);
        var modelo = await _context.ModelosDieta
            .Include(m => m.Refeicoes).ThenInclude(r => r.Itens)
            .FirstOrDefaultAsync(m => m.Id == modeloId && m.Ativo);
        if (modelo == null) 
            //TODO:
            //Nao devemos jogar excecao
            //retornar status codigo, bool e mensagem
            //aqui o client que consome decide se redireciona ou nao
            throw new InvalidOperationException("Modelo de dieta não encontrado.");

        // Buscar metas do perfil para escalonar se necessário
        var meta = await _context.MetasNutricionais
            .FirstOrDefaultAsync(m => m.PerfilNutricionalId == perfil.Id);

        double fatorEscala = 1.0;
        if (meta != null && modelo.CaloriasBase > 0)
            fatorEscala = meta.CaloriasDiarias / modelo.CaloriasBase;

        var plano = new PlanoAlimentar
        {
            PerfilNutricionalId = perfil.Id,
            Nome = $"{modelo.Nome} (personalizado)",
            Descricao = modelo.Descricao,
            DataInicio = dataInicio,
            DataFim = dataFim,
            Status = EStatusPlano.Rascunho,
            ModeloDietaOrigemId = modeloId,
            CaloriasAlvoDiarias = meta?.CaloriasDiarias ?? modelo.CaloriasBase,
            ProteinaAlvoG = meta?.ProteinasDiarias ?? modelo.ProteinaBaseG,
            CarboidratoAlvoG = meta?.CarboidratosDiarios ?? modelo.CarboidratoBaseG,
            GorduraAlvoG = meta?.GordurasDiarias ?? modelo.GorduraBaseG,
            FibraAlvoG = meta?.FibraDiaria ?? 25,
            AguaAlvoL = meta?.AguaDiaria ?? 2.0,
        };

        foreach (var refModelo in modelo.Refeicoes.OrderBy(r => r.Ordem))
        {
            var refeicao = new RefeicaoPlano
            {
                TipoRefeicao = refModelo.TipoRefeicao,
                HorarioSugerido = refModelo.HorarioSugerido,
                Ordem = refModelo.Ordem,
                Observacoes = refModelo.Observacoes
            };

            foreach (var itemModelo in refModelo.Itens.OrderBy(i => i.Ordem))
            {
                refeicao.Itens.Add(new ItemRefeicao
                {
                    AlimentoId = itemModelo.AlimentoId,
                    TipoTabela = itemModelo.TipoTabela,
                    NomeAlimentoSnapshot = itemModelo.NomeAlimentoSnapshot,
                    QuantidadeG = Math.Round(itemModelo.QuantidadeG * fatorEscala, 1),
                    EnergiaKcal = Math.Round(itemModelo.EnergiaKcal * fatorEscala, 1),
                    ProteinaG = Math.Round(itemModelo.ProteinaG * fatorEscala, 1),
                    CarboidratoG = Math.Round(itemModelo.CarboidratoG * fatorEscala, 1),
                    GorduraG = Math.Round(itemModelo.GorduraG * fatorEscala, 1),
                    FibraG = Math.Round(itemModelo.FibraG * fatorEscala, 1),
                    Ordem = itemModelo.Ordem
                });
            }

            RecalcularTotaisRefeicao(refeicao);
            plano.RefeicoesPlanejadas.Add(refeicao);
        }

        _context.PlanosAlimentares.Add(plano);
        await _context.SaveChangesAsync();
        return await MapearPlanoResultado(plano);
    }

    // ============================================================
    // Helpers privados
    // ============================================================

    private async Task<PlanoAlimentar> ConstruirPlano(
        PerfilNutricional perfil, CriarPlanoAlimentarDto dto, string? profissionalId)
    {
        // Buscar metas nutricionais para auto-preenchimento
        var meta = await _context.MetasNutricionais
            .FirstOrDefaultAsync(m => m.PerfilNutricionalId == perfil.Id);

        var plano = new PlanoAlimentar
        {
            PerfilNutricionalId = perfil.Id,
            ProfissionalResponsavelId = profissionalId,
            Nome = dto.Nome,
            Descricao = dto.Descricao,
            DataInicio = dto.DataInicio,
            DataFim = dto.DataFim,
            Status = EStatusPlano.Rascunho,
            ModeloDietaOrigemId = dto.ModeloDietaOrigemId,
            Observacoes = dto.Observacoes,
            CaloriasAlvoDiarias = dto.CaloriasAlvoDiarias ?? meta?.CaloriasDiarias ?? 2000,
            ProteinaAlvoG = dto.ProteinaAlvoG ?? meta?.ProteinasDiarias ?? 120,
            CarboidratoAlvoG = dto.CarboidratoAlvoG ?? meta?.CarboidratosDiarios ?? 250,
            GorduraAlvoG = dto.GorduraAlvoG ?? meta?.GordurasDiarias ?? 65,
            FibraAlvoG = dto.FibraAlvoG ?? meta?.FibraDiaria ?? 25,
            AguaAlvoL = dto.AguaAlvoL ?? meta?.AguaDiaria ?? 2.0,
        };

        if (dto.Refeicoes?.Any() == true)
        {
            foreach (var refDto in dto.Refeicoes)
            {
                var refeicao = new RefeicaoPlano
                {
                    TipoRefeicao = refDto.TipoRefeicao,
                    HorarioSugerido = refDto.HorarioSugerido,
                    Ordem = refDto.Ordem,
                    Observacoes = refDto.Observacoes
                };

                foreach (var itemDto in refDto.Itens)
                {
                    var item = await ConstruirItem(itemDto);
                    refeicao.Itens.Add(item);
                }

                RecalcularTotaisRefeicao(refeicao);
                plano.RefeicoesPlanejadas.Add(refeicao);
            }
        }

        return plano;
    }

    private async Task<ItemRefeicao> ConstruirItem(ItemRefeicaoDto dto)
    {
        var alimento = await _busca.BuscaAlimentoPorIdAsync(dto.AlimentoId, dto.TipoTabela);
        if (alimento == null) 
            //TODO:
            //Nao devemos jogar excecao
            //retornar status codigo, bool e mensagem
            //aqui o client que consome decide se redireciona ou nao
            throw new InvalidOperationException($"Alimento {dto.AlimentoId} ({dto.TipoTabela}) não encontrado.");

        var item = CriarItemComMacros(alimento, dto.AlimentoId, dto.TipoTabela, dto.QuantidadeG, dto.Ordem, dto.Observacoes);

        if (dto.Substituicoes?.Any() == true)
        {
            foreach (var subDto in dto.Substituicoes)
            {
                var alimentoSub = await _busca.BuscaAlimentoPorIdAsync(subDto.AlimentoId, subDto.TipoTabela);
                if (alimentoSub == null) continue;
                item.SubstituicoesEquivalentes.Add(
                    CriarSubstituicaoComMacros(alimentoSub, subDto.AlimentoId, subDto.TipoTabela, subDto.QuantidadeG));
            }
        }

        return item;
    }

    private static ItemRefeicao CriarItemComMacros(
        AlimentoResumoDto alimento, int alimentoId, ETipoTabela tabela,
        double quantidadeG, int ordem, string? observacoes)
    {
        var macros = CalcularMacrosProporcional(alimento, quantidadeG);
        return new ItemRefeicao
        {
            AlimentoId = alimentoId,
            TipoTabela = tabela,
            NomeAlimentoSnapshot = alimento.Nome,
            QuantidadeG = quantidadeG,
            EnergiaKcal = macros.kcal,
            ProteinaG = macros.prot,
            CarboidratoG = macros.carb,
            GorduraG = macros.gord,
            FibraG = macros.fibra,
            Ordem = ordem,
            Observacoes = observacoes
        };
    }

    private static SubstituicaoEquivalente CriarSubstituicaoComMacros(
        AlimentoResumoDto alimento, int alimentoId, ETipoTabela tabela, double quantidadeG)
    {
        var macros = CalcularMacrosProporcional(alimento, quantidadeG);
        return new SubstituicaoEquivalente
        {
            AlimentoId = alimentoId,
            TipoTabela = tabela,
            NomeAlimento = alimento.Nome,
            QuantidadeG = quantidadeG,
            EnergiaKcal = macros.kcal,
            ProteinaG = macros.prot,
            CarboidratoG = macros.carb,
            GorduraG = macros.gord,
            FibraG = macros.fibra
        };
    }

    /// <summary>
    /// Calcula macros proporcionalmente à quantidade baseado na porção de referência (100g default).
    /// </summary>
    private static (double kcal, double prot, double carb, double gord, double fibra)
        CalcularMacrosProporcional(AlimentoResumoDto alimento, double quantidadeG)
    {
        double porcaoRef = alimento.PorcaoReferencia > 0 ? alimento.PorcaoReferencia : 100.0;
        double fator = quantidadeG / porcaoRef;

        return (
            kcal: Math.Round(alimento.Macros.EnergiaKcal * fator, 1),
            prot: Math.Round(alimento.Macros.Proteina * fator, 1),
            carb: Math.Round(alimento.Macros.CarboDisponivel * fator, 1),
            gord: Math.Round(alimento.Macros.LipidiosG * fator, 1),
            fibra: Math.Round(alimento.Macros.Fibras * fator, 1)
        );
    }

    private static void RecalcularTotaisRefeicao(RefeicaoPlano refeicao)
    {
        refeicao.TotalEnergiaKcal = Math.Round(refeicao.Itens.Sum(i => i.EnergiaKcal), 1);
        refeicao.TotalProteinaG = Math.Round(refeicao.Itens.Sum(i => i.ProteinaG), 1);
        refeicao.TotalCarboidratoG = Math.Round(refeicao.Itens.Sum(i => i.CarboidratoG), 1);
        refeicao.TotalGorduraG = Math.Round(refeicao.Itens.Sum(i => i.GorduraG), 1);
        refeicao.TotalFibraG = Math.Round(refeicao.Itens.Sum(i => i.FibraG), 1);
    }

    private IQueryable<PlanoAlimentar> CarregarPlanoCompleto()
    {
        return _context.PlanosAlimentares
            .Include(p => p.PerfilNutricional)
            .Include(p => p.ProfissionalResponsavel)
            .Include(p => p.ModeloDietaOrigem)
            .Include(p => p.RefeicoesPlanejadas.OrderBy(r => r.Ordem))
                .ThenInclude(r => r.Itens.OrderBy(i => i.Ordem))
                    .ThenInclude(i => i.SubstituicoesEquivalentes);
    }

    private async Task<PlanoAlimentarResultadoDto> MapearPlanoResultado(PlanoAlimentar plano)
    {
        double totalCalPlano = plano.RefeicoesPlanejadas.Sum(r => r.TotalEnergiaKcal);
        double totalProtPlano = plano.RefeicoesPlanejadas.Sum(r => r.TotalProteinaG);
        double totalCarbPlano = plano.RefeicoesPlanejadas.Sum(r => r.TotalCarboidratoG);
        double totalGordPlano = plano.RefeicoesPlanejadas.Sum(r => r.TotalGorduraG);
        double totalFibraPlano = plano.RefeicoesPlanejadas.Sum(r => r.TotalFibraG);

        return new PlanoAlimentarResultadoDto
        {
            Id = plano.Id,
            Nome = plano.Nome,
            Descricao = plano.Descricao,
            DataInicio = plano.DataInicio,
            DataFim = plano.DataFim,
            Status = plano.Status,
            Observacoes = plano.Observacoes,
            ProfissionalResponsavel = plano.ProfissionalResponsavel?.NomeCompleto,
            ModeloDietaOrigem = plano.ModeloDietaOrigem?.Nome,
            CriadoEm = plano.CriadoEm,
            AtualizadoEm = plano.AtualizadoEm,
            MetasDiarias = new MacrosDiariosPlanoDto
            {
                CaloriasKcal = plano.CaloriasAlvoDiarias,
                ProteinaG = plano.ProteinaAlvoG,
                CarboidratoG = plano.CarboidratoAlvoG,
                GorduraG = plano.GorduraAlvoG,
                FibraG = plano.FibraAlvoG,
                AguaL = plano.AguaAlvoL
            },
            TotaisCalculados = new MacrosDiariosPlanoDto
            {
                CaloriasKcal = totalCalPlano,
                ProteinaG = totalProtPlano,
                CarboidratoG = totalCarbPlano,
                GorduraG = totalGordPlano,
                FibraG = totalFibraPlano
            },
            DiferencaMetas = new MacrosDiariosPlanoDto
            {
                CaloriasKcal = Math.Round(plano.CaloriasAlvoDiarias - totalCalPlano, 1),
                ProteinaG = Math.Round(plano.ProteinaAlvoG - totalProtPlano, 1),
                CarboidratoG = Math.Round(plano.CarboidratoAlvoG - totalCarbPlano, 1),
                GorduraG = Math.Round(plano.GorduraAlvoG - totalGordPlano, 1),
                FibraG = Math.Round(plano.FibraAlvoG - totalFibraPlano, 1)
            },
            Refeicoes = plano.RefeicoesPlanejadas.Select(r => new RefeicaoPlanoResultadoDto
            {
                Id = r.Id,
                TipoRefeicao = r.TipoRefeicao,
                HorarioSugerido = r.HorarioSugerido,
                Ordem = r.Ordem,
                Observacoes = r.Observacoes,
                TotalEnergiaKcal = r.TotalEnergiaKcal,
                TotalProteinaG = r.TotalProteinaG,
                TotalCarboidratoG = r.TotalCarboidratoG,
                TotalGorduraG = r.TotalGorduraG,
                TotalFibraG = r.TotalFibraG,
                PercentualCaloricoRefeicao = totalCalPlano > 0
                    ? Math.Round(r.TotalEnergiaKcal / totalCalPlano * 100, 1)
                    : 0,
                Itens = r.Itens.Select(i => new ItemRefeicaoResultadoDto
                {
                    Id = i.Id,
                    AlimentoId = i.AlimentoId,
                    TipoTabela = i.TipoTabela,
                    NomeAlimento = i.NomeAlimentoSnapshot,
                    QuantidadeG = i.QuantidadeG,
                    Ordem = i.Ordem,
                    Observacoes = i.Observacoes,
                    EnergiaKcal = i.EnergiaKcal,
                    ProteinaG = i.ProteinaG,
                    CarboidratoG = i.CarboidratoG,
                    GorduraG = i.GorduraG,
                    FibraG = i.FibraG,
                    Substituicoes = i.SubstituicoesEquivalentes.Select(s => new SubstituicaoResultadoDto
                    {
                        Id = s.Id,
                        AlimentoId = s.AlimentoId,
                        TipoTabela = s.TipoTabela,
                        NomeAlimento = s.NomeAlimento,
                        QuantidadeG = s.QuantidadeG,
                        EnergiaKcal = s.EnergiaKcal,
                        ProteinaG = s.ProteinaG,
                        CarboidratoG = s.CarboidratoG,
                        GorduraG = s.GorduraG,
                        FibraG = s.FibraG
                    }).ToList()
                }).ToList()
            }).ToList()
        };
    }

    private ModeloDietaResultadoDto MapearModeloResultado(ModeloDieta modelo)
    {
        double totalCal = modelo.Refeicoes.SelectMany(r => r.Itens).Sum(i => i.EnergiaKcal);

        return new ModeloDietaResultadoDto
        {
            Id = modelo.Id,
            Nome = modelo.Nome,
            Descricao = modelo.Descricao,
            ObjetivoAlvo = modelo.ObjetivoAlvo,
            PreferenciaAlimentarAlvo = modelo.PreferenciaAlimentarAlvo,
            CaloriasBase = modelo.CaloriasBase,
            ProteinaBaseG = modelo.ProteinaBaseG,
            CarboidratoBaseG = modelo.CarboidratoBaseG,
            GorduraBaseG = modelo.GorduraBaseG,
            NumeroRefeicoesDia = modelo.NumeroRefeicoesDia,
            Publico = modelo.Publico,
            CriadoPorProfissional = modelo.CriadoPorProfissional?.NomeCompleto,
            Refeicoes = modelo.Refeicoes.OrderBy(r => r.Ordem).Select(r => new RefeicaoPlanoResultadoDto
            {
                Id = r.Id,
                TipoRefeicao = r.TipoRefeicao,
                HorarioSugerido = r.HorarioSugerido,
                Ordem = r.Ordem,
                Observacoes = r.Observacoes,
                TotalEnergiaKcal = r.Itens.Sum(i => i.EnergiaKcal),
                TotalProteinaG = r.Itens.Sum(i => i.ProteinaG),
                TotalCarboidratoG = r.Itens.Sum(i => i.CarboidratoG),
                TotalGorduraG = r.Itens.Sum(i => i.GorduraG),
                TotalFibraG = r.Itens.Sum(i => i.FibraG),
                PercentualCaloricoRefeicao = totalCal > 0
                    ? Math.Round(r.Itens.Sum(i => i.EnergiaKcal) / totalCal * 100, 1)
                    : r.PercentualCaloricoSugerido,
                Itens = r.Itens.OrderBy(i => i.Ordem).Select(i => new ItemRefeicaoResultadoDto
                {
                    Id = i.Id,
                    AlimentoId = i.AlimentoId,
                    TipoTabela = i.TipoTabela,
                    NomeAlimento = i.NomeAlimentoSnapshot,
                    QuantidadeG = i.QuantidadeG,
                    Ordem = i.Ordem,
                    EnergiaKcal = i.EnergiaKcal,
                    ProteinaG = i.ProteinaG,
                    CarboidratoG = i.CarboidratoG,
                    GorduraG = i.GorduraG,
                    FibraG = i.FibraG
                }).ToList()
            }).ToList()
        };
    }

    private async Task<PerfilNutricional> ObterPerfilOuErro(string userId)
    {
        var perfil = await _context.PerfilNutricional.FirstOrDefaultAsync(p => p.UserId == userId);
        if (perfil == null)
            //TODO:
            //Nao devemos jogar excecao
            //retornar status codigo, bool e mensagem
            //aqui o client que consome decide se redireciona ou nao
            throw new InvalidOperationException("Perfil nutricional não encontrado. Crie o perfil antes de gerenciar planos alimentares.");
        return perfil;
    }

    private async Task ValidarVinculoProfissional(string profissionalUserId, string pacienteUserId)
    {
        var vinculoExiste = await _context.VinculosPacienteProfissional
            .Include(v => v.Profissional)
            .AnyAsync(v =>
                v.Profissional.UserId == profissionalUserId &&
                v.PacienteUserId == pacienteUserId &&
                v.Status == EStatusVinculo.Ativo);
        if (!vinculoExiste)
            //TODO:
            //Nao devemos jogar excecao
            //retornar status codigo, bool e mensagem
            //aqui o client que consome decide se redireciona ou nao
            throw new InvalidOperationException("Profissional não possui vínculo ativo com este paciente.");
    }
}
