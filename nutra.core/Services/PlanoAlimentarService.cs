using Nutra.Enum;
using Nutra.Interfaces;
using Nutra.Models;
using Nutra.Models.Dtos;
using Nutra.Models.RegraNutricional;
using Nutra.Models.Usuario;

namespace Nutra.Services;

public class PlanoAlimentarService : IPlanoAlimentar
{
    private readonly IPlanoAlimentarRepository _planoRepository;
    private readonly IRefeicaoPlanoRepository _refeicaoRepository;
    private readonly IItemRefeicaoRepository _itemRefeicaoRepository;
    private readonly ISubstituicaoEquivalenteRepository _substituicaoRepository;
    private readonly IModeloDietaRepository _modeloDietaRepository;
    private readonly IMetaNutricionalRepository _metaRepository;
    private readonly IPerfilNutricionalRepository _perfilRepository;
    private readonly IVinculoPacienteProfissionalRepository _vinculoRepository;
    private readonly IBusca _busca;
    private readonly IUnitOfWork _unitOfWork;

    public PlanoAlimentarService(
        IPlanoAlimentarRepository planoRepository,
        IRefeicaoPlanoRepository refeicaoRepository,
        IItemRefeicaoRepository itemRefeicaoRepository,
        ISubstituicaoEquivalenteRepository substituicaoRepository,
        IModeloDietaRepository modeloDietaRepository,
        IMetaNutricionalRepository metaRepository,
        IPerfilNutricionalRepository perfilRepository,
        IVinculoPacienteProfissionalRepository vinculoRepository,
        IBusca busca,
        IUnitOfWork unitOfWork)
    {
        _planoRepository = planoRepository;
        _refeicaoRepository = refeicaoRepository;
        _itemRefeicaoRepository = itemRefeicaoRepository;
        _substituicaoRepository = substituicaoRepository;
        _modeloDietaRepository = modeloDietaRepository;
        _metaRepository = metaRepository;
        _perfilRepository = perfilRepository;
        _vinculoRepository = vinculoRepository;
        _busca = busca;
        _unitOfWork = unitOfWork;
    }

    // ============================================================
    // CRUD Plano
    // ============================================================

    public async Task<RetornoPadrao<PlanoAlimentarResultadoDto>> CriarPlanoAsync(string userId, CriarPlanoAlimentarDto dto)
    {
        var perfil = await ObterPerfil(userId);
        if (perfil == null)
            return RetornoPadrao<PlanoAlimentarResultadoDto>.NaoEncontrado(PerfilAusente);

        var planoRetorno = await ConstruirPlano(perfil, dto, profissionalId: null);
        if (!planoRetorno.Sucesso)
            return RetornoPadrao<PlanoAlimentarResultadoDto>.Falha(planoRetorno);

        var plano = planoRetorno.Dados!;
        _planoRepository.Add(plano);
        await _unitOfWork.SaveChangesAsync();
        return RetornoPadrao<PlanoAlimentarResultadoDto>.Criado(
            await MapearPlanoResultado(plano), "Plano alimentar criado com sucesso.");
    }

    public async Task<RetornoPadrao<PlanoAlimentarResultadoDto>> CriarPlanoPorProfissionalAsync(
        string profissionalUserId, CriarPlanoProfissionalDto dto)
    {
        if (!await ExisteVinculoAtivo(profissionalUserId, dto.PacienteUserId))
            return RetornoPadrao<PlanoAlimentarResultadoDto>.Proibido(
                "Profissional não possui vínculo ativo com este paciente.");

        var perfil = await ObterPerfil(dto.PacienteUserId);
        if (perfil == null)
            return RetornoPadrao<PlanoAlimentarResultadoDto>.NaoEncontrado(
                "Paciente não possui perfil nutricional.");

        var planoRetorno = await ConstruirPlano(perfil, dto, profissionalUserId);
        if (!planoRetorno.Sucesso)
            return RetornoPadrao<PlanoAlimentarResultadoDto>.Falha(planoRetorno);

        var plano = planoRetorno.Dados!;
        _planoRepository.Add(plano);
        await _unitOfWork.SaveChangesAsync();
        return RetornoPadrao<PlanoAlimentarResultadoDto>.Criado(
            await MapearPlanoResultado(plano), "Plano alimentar criado com sucesso.");
    }

    public async Task<RetornoPadrao<PlanoAlimentarResultadoDto>> ObterPlanoAsync(string userId, int planoId)
    {
        var plano = await _planoRepository.ObterCompletoPorIdEUsuarioAsync(planoId, userId);
        if (plano == null)
            return RetornoPadrao<PlanoAlimentarResultadoDto>.NaoEncontrado("Plano alimentar não encontrado.");

        return RetornoPadrao<PlanoAlimentarResultadoDto>.Ok(await MapearPlanoResultado(plano));
    }

    public async Task<RetornoPadrao<PlanoAlimentarResultadoDto>> ObterPlanoAtivoAsync(string userId)
    {
        var plano = await _planoRepository.ObterCompletoAtivoPorUsuarioAsync(userId);
        if (plano == null)
            return RetornoPadrao<PlanoAlimentarResultadoDto>.NaoEncontrado(
                "Nenhum plano alimentar ativo encontrado.");

        return RetornoPadrao<PlanoAlimentarResultadoDto>.Ok(await MapearPlanoResultado(plano));
    }

    public async Task<RetornoPadrao<List<PlanoAlimentarResumoDto>>> ListarPlanosAsync(string userId)
    {
        var perfil = await ObterPerfil(userId);
        if (perfil == null)
            return RetornoPadrao<List<PlanoAlimentarResumoDto>>.NaoEncontrado(PerfilAusente);

        var entidades = await _planoRepository.ListarPorPerfilAsync(perfil.Id);

        var planos = entidades
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
                ProfissionalResponsavel = p.ProfissionalResponsavel?.NomeCompleto,
                CriadoEm = p.CriadoEm
            })
            .ToList();

        return RetornoPadrao<List<PlanoAlimentarResumoDto>>.Ok(planos);
    }

    public async Task<RetornoPadrao<PlanoAlimentarResultadoDto>> AtualizarPlanoAsync(
        string userId, int planoId, AtualizarPlanoAlimentarDto dto)
    {
        var perfil = await ObterPerfil(userId);
        if (perfil == null)
            return RetornoPadrao<PlanoAlimentarResultadoDto>.NaoEncontrado(PerfilAusente);

        var plano = await _planoRepository.ObterCompletoPorIdEPerfilAsync(planoId, perfil.Id);
        if (plano == null)
            return RetornoPadrao<PlanoAlimentarResultadoDto>.NaoEncontrado("Plano alimentar não encontrado.");

        if (dto.Nome != null) plano.Nome = dto.Nome;
        if (dto.Descricao != null) plano.Descricao = dto.Descricao;
        if (dto.DataFim.HasValue) plano.DataFim = dto.DataFim.Value;
        if (dto.Status.HasValue) plano.Status = dto.Status.Value;
        if (dto.Observacoes != null) plano.Observacoes = dto.Observacoes;
        plano.AtualizadoEm = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync();
        return RetornoPadrao<PlanoAlimentarResultadoDto>.Ok(
            await MapearPlanoResultado(plano), "Plano alimentar atualizado com sucesso.");
    }

    public async Task<RetornoPadrao> ExcluirPlanoAsync(string userId, int planoId)
    {
        var perfil = await ObterPerfil(userId);
        if (perfil == null)
            return RetornoPadrao.NaoEncontrado(PerfilAusente);

        var plano = await _planoRepository.ObterCompletoPorIdEPerfilAsync(planoId, perfil.Id);
        if (plano == null)
            return RetornoPadrao.NaoEncontrado("Plano alimentar não encontrado.");

        _planoRepository.Remove(plano);
        await _unitOfWork.SaveChangesAsync();
        return RetornoPadrao.Ok("Plano alimentar excluído com sucesso.");
    }

    public async Task<RetornoPadrao> AtivarPlanoAsync(string userId, int planoId)
    {
        var perfil = await ObterPerfil(userId);
        if (perfil == null)
            return RetornoPadrao.NaoEncontrado(PerfilAusente);

        // Desativar plano ativo anterior
        var planosAtivos = await _planoRepository.ListarAtivosPorPerfilAsync(perfil.Id);
        foreach (var ativo in planosAtivos)
        {
            ativo.Status = EStatusPlano.Pausado;
            ativo.AtualizadoEm = DateTime.UtcNow;
        }

        var plano = await _planoRepository.ObterPorIdEPerfilAsync(planoId, perfil.Id);
        if (plano == null)
            return RetornoPadrao.NaoEncontrado("Plano alimentar não encontrado.");

        plano.Status = EStatusPlano.Ativo;
        plano.AtualizadoEm = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync();
        return RetornoPadrao.Ok("Plano alimentar ativado com sucesso.");
    }

    // ============================================================
    // Refeições
    // ============================================================

    public async Task<RetornoPadrao<PlanoAlimentarResultadoDto>> AdicionarRefeicaoAsync(
        string userId, int planoId, AdicionarRefeicaoDto dto)
    {
        var perfil = await ObterPerfil(userId);
        if (perfil == null)
            return RetornoPadrao<PlanoAlimentarResultadoDto>.NaoEncontrado(PerfilAusente);

        var plano = await _planoRepository.ObterCompletoPorIdEPerfilAsync(planoId, perfil.Id);
        if (plano == null)
            return RetornoPadrao<PlanoAlimentarResultadoDto>.NaoEncontrado("Plano alimentar não encontrado.");

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
                var itemRetorno = await ConstruirItem(itemDto);
                if (!itemRetorno.Sucesso)
                    return RetornoPadrao<PlanoAlimentarResultadoDto>.Falha(itemRetorno);
                refeicao.Itens.Add(itemRetorno.Dados!);
            }
            RecalcularTotaisRefeicao(refeicao);
        }

        plano.RefeicoesPlanejadas.Add(refeicao);
        plano.AtualizadoEm = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync();
        return RetornoPadrao<PlanoAlimentarResultadoDto>.Criado(
            await MapearPlanoResultado(plano), "Refeição adicionada com sucesso.");
    }

    public async Task<RetornoPadrao> RemoverRefeicaoAsync(string userId, int refeicaoId)
    {
        var perfil = await ObterPerfil(userId);
        if (perfil == null)
            return RetornoPadrao.NaoEncontrado(PerfilAusente);

        var refeicao = await _refeicaoRepository
            .ObterComItensESubstituicoesAsync(refeicaoId, perfil.Id);
        if (refeicao == null)
            return RetornoPadrao.NaoEncontrado("Refeição não encontrada.");

        _refeicaoRepository.Remove(refeicao);
        refeicao.PlanoAlimentar.AtualizadoEm = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync();
        return RetornoPadrao.Ok("Refeição removida com sucesso.");
    }

    // ============================================================
    // Itens
    // ============================================================

    public async Task<RetornoPadrao<PlanoAlimentarResultadoDto>> AdicionarItemAsync(
        string userId, int refeicaoId, AdicionarItemDto dto)
    {
        var perfil = await ObterPerfil(userId);
        if (perfil == null)
            return RetornoPadrao<PlanoAlimentarResultadoDto>.NaoEncontrado(PerfilAusente);

        var refeicao = await _refeicaoRepository.ObterComItensAsync(refeicaoId, perfil.Id);
        if (refeicao == null)
            return RetornoPadrao<PlanoAlimentarResultadoDto>.NaoEncontrado("Refeição não encontrada.");

        var alimento = await _busca.BuscaAlimentoPorIdAsync(dto.AlimentoId, dto.TipoTabela);
        if (alimento == null)
            return RetornoPadrao<PlanoAlimentarResultadoDto>.NaoEncontrado("Alimento não encontrado.");

        var item = CriarItemComMacros(alimento, dto.AlimentoId, dto.TipoTabela, dto.QuantidadeG, dto.Ordem, dto.Observacoes);
        refeicao.Itens.Add(item);
        RecalcularTotaisRefeicao(refeicao);
        refeicao.PlanoAlimentar.AtualizadoEm = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync();

        var plano = await _planoRepository.ObterCompletoPorIdAsync(refeicao.PlanoAlimentarId);
        return RetornoPadrao<PlanoAlimentarResultadoDto>.Criado(
            await MapearPlanoResultado(plano!), "Item adicionado com sucesso.");
    }

    public async Task<RetornoPadrao> RemoverItemAsync(string userId, int itemId)
    {
        var perfil = await ObterPerfil(userId);
        if (perfil == null)
            return RetornoPadrao.NaoEncontrado(PerfilAusente);

        var item = await _itemRefeicaoRepository
            .ObterComRefeicaoEPlanoAsync(itemId, perfil.Id);
        if (item == null)
            return RetornoPadrao.NaoEncontrado("Item não encontrado.");

        var refeicao = item.RefeicaoPlano;
        _itemRefeicaoRepository.Remove(item);
        refeicao.PlanoAlimentar.AtualizadoEm = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync();

        // Recalcular totais da refeição sem o item removido
        var refeicaoAtualizada = await _refeicaoRepository.ObterComItensPorIdAsync(refeicao.Id);
        if (refeicaoAtualizada != null) RecalcularTotaisRefeicao(refeicaoAtualizada);
        await _unitOfWork.SaveChangesAsync();

        return RetornoPadrao.Ok("Item removido com sucesso.");
    }

    // ============================================================
    // Substituições Equivalentes
    // ============================================================

    public async Task<RetornoPadrao> AdicionarSubstituicaoAsync(
        string userId, int itemId, AdicionarSubstituicaoDto dto)
    {
        var perfil = await ObterPerfil(userId);
        if (perfil == null)
            return RetornoPadrao.NaoEncontrado(PerfilAusente);

        var item = await _itemRefeicaoRepository
            .ObterComRefeicaoEPlanoAsync(itemId, perfil.Id);
        if (item == null)
            return RetornoPadrao.NaoEncontrado("Item não encontrado.");

        var alimento = await _busca.BuscaAlimentoPorIdAsync(dto.AlimentoId, dto.TipoTabela);
        if (alimento == null)
            return RetornoPadrao.NaoEncontrado("Alimento substituto não encontrado.");

        var sub = CriarSubstituicaoComMacros(alimento, dto.AlimentoId, dto.TipoTabela, dto.QuantidadeG);
        item.SubstituicoesEquivalentes.Add(sub);
        item.RefeicaoPlano.PlanoAlimentar.AtualizadoEm = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync();

        return RetornoPadrao.Criado("Substituição adicionada com sucesso.");
    }

    public async Task<RetornoPadrao> RemoverSubstituicaoAsync(string userId, int substituicaoId)
    {
        var perfil = await ObterPerfil(userId);
        if (perfil == null)
            return RetornoPadrao.NaoEncontrado(PerfilAusente);

        var sub = await _substituicaoRepository
            .ObterComItemRefeicaoEPlanoAsync(substituicaoId, perfil.Id);
        if (sub == null)
            return RetornoPadrao.NaoEncontrado("Substituição não encontrada.");

        _substituicaoRepository.Remove(sub);
        sub.ItemRefeicao.RefeicaoPlano.PlanoAlimentar.AtualizadoEm = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync();
        return RetornoPadrao.Ok("Substituição removida com sucesso.");
    }

    // ============================================================
    // Modelos de Dieta (Templates)
    // ============================================================

    public async Task<RetornoPadrao<ModeloDietaResultadoDto>> CriarModeloDietaAsync(
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

        _modeloDietaRepository.Add(modelo);
        await _unitOfWork.SaveChangesAsync();
        return RetornoPadrao<ModeloDietaResultadoDto>.Criado(
            MapearModeloResultado(modelo), "Modelo de dieta criado com sucesso.");
    }

    public async Task<RetornoPadrao<List<ModeloDietaResumoDto>>> ListarModelosDietaAsync(string? profissionalUserId)
    {
        var lista = await _modeloDietaRepository.ListarDisponiveisAsync(profissionalUserId);

        var dto = lista
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
            .ToList();

        return RetornoPadrao<List<ModeloDietaResumoDto>>.Ok(dto);
    }

    public async Task<RetornoPadrao<ModeloDietaResultadoDto>> ObterModeloDietaAsync(int modeloId)
    {
        var modelo = await _modeloDietaRepository.ObterCompletoAtivoAsync(modeloId);
        if (modelo == null)
            return RetornoPadrao<ModeloDietaResultadoDto>.NaoEncontrado("Modelo de dieta não encontrado.");

        return RetornoPadrao<ModeloDietaResultadoDto>.Ok(MapearModeloResultado(modelo));
    }

    public async Task<RetornoPadrao> ExcluirModeloDietaAsync(string profissionalUserId, int modeloId)
    {
        var modelo = await _modeloDietaRepository
            .ObterPorIdEProfissionalAsync(modeloId, profissionalUserId);
        if (modelo == null)
            return RetornoPadrao.NaoEncontrado("Modelo de dieta não encontrado.");

        modelo.Ativo = false;
        modelo.AtualizadoEm = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync();
        return RetornoPadrao.Ok("Modelo de dieta excluído com sucesso.");
    }

    public async Task<RetornoPadrao<PlanoAlimentarResultadoDto>> CriarPlanoAPartirDeModeloAsync(
        string userId, int modeloId, DateTime dataInicio, DateTime? dataFim)
    {
        var perfil = await ObterPerfil(userId);
        if (perfil == null)
            return RetornoPadrao<PlanoAlimentarResultadoDto>.NaoEncontrado(PerfilAusente);

        var modelo = await _modeloDietaRepository.ObterCompletoAtivoAsync(modeloId);
        if (modelo == null)
            return RetornoPadrao<PlanoAlimentarResultadoDto>.NaoEncontrado("Modelo de dieta não encontrado.");

        // Buscar metas do perfil para escalonar se necessário
        var meta = await _metaRepository.ObterPorPerfilIdAsync(perfil.Id);

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

        _planoRepository.Add(plano);
        await _unitOfWork.SaveChangesAsync();
        return RetornoPadrao<PlanoAlimentarResultadoDto>.Criado(
            await MapearPlanoResultado(plano), "Plano alimentar criado a partir do modelo.");
    }

    // ============================================================
    // Helpers privados
    // ============================================================

    /// <summary>
    /// Exceção deliberada à regra "helper retorna nullable": a falha aqui carrega
    /// qual alimento não foi encontrado, informação que um <c>null</c> descartaria.
    /// Só 2 call sites pagam o desembrulho.
    /// </summary>
    private async Task<RetornoPadrao<PlanoAlimentar>> ConstruirPlano(
        PerfilNutricional perfil, CriarPlanoAlimentarDto dto, string? profissionalId)
    {
        // Buscar metas nutricionais para auto-preenchimento
        var meta = await _metaRepository.ObterPorPerfilIdAsync(perfil.Id);

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
                    var itemRetorno = await ConstruirItem(itemDto);
                    if (!itemRetorno.Sucesso)
                        return RetornoPadrao<PlanoAlimentar>.Falha(itemRetorno);
                    refeicao.Itens.Add(itemRetorno.Dados!);
                }

                RecalcularTotaisRefeicao(refeicao);
                plano.RefeicoesPlanejadas.Add(refeicao);
            }
        }

        return RetornoPadrao<PlanoAlimentar>.Ok(plano);
    }

    private async Task<RetornoPadrao<ItemRefeicao>> ConstruirItem(ItemRefeicaoDto dto)
    {
        var alimento = await _busca.BuscaAlimentoPorIdAsync(dto.AlimentoId, dto.TipoTabela);
        if (alimento == null)
            return RetornoPadrao<ItemRefeicao>.NaoEncontrado(
                $"Alimento {dto.AlimentoId} ({dto.TipoTabela}) não encontrado.");

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

        return RetornoPadrao<ItemRefeicao>.Ok(item);
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

    /// <summary>
    /// Helper privado: retorna null quando o perfil não existe. Quem envelopa a falha
    /// e escolhe o status HTTP é o método público — helper não conhece HTTP.
    /// </summary>
    private Task<PerfilNutricional?> ObterPerfil(string userId) =>
        _perfilRepository.ObterPorUsuarioIdAsync(userId);

    private const string PerfilAusente =
        "Perfil nutricional não encontrado. Crie o perfil antes de gerenciar planos alimentares.";

    private Task<bool> ExisteVinculoAtivo(string profissionalUserId, string pacienteUserId) =>
        _vinculoRepository.ExisteVinculoAtivoAsync(profissionalUserId, pacienteUserId);
}
