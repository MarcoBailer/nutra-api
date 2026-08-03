using Nutra.Enum;
using Nutra.Helper;
using Nutra.Interfaces;
using Nutra.Models;
using Nutra.Models.Dtos;
using Nutra.Models.RegraNutricional;
using Nutra.Models.Usuario;

namespace Nutra.Services;

public class UserProfileService : IUserProfile
{
    private readonly IApplicationUserRepository _applicationUserService;
    private readonly ICalculadoraNutricional _calculadora;
    private readonly IBusca _busca;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPerfilNutricionalRepository _perfilRepository;
    private readonly IMetaNutricionalRepository _metaRepository;
    private readonly IBaseRepository<RestricaoAlimentar> _restricaoRepository;
    private readonly IBaseRepository<PerfilEquipamento> _equipamentoRepository;
    private readonly IHistoricoClinicoRepository _historicoRepository;
    private readonly IPreferenciaAlimentarRepository _preferenciaRepository;
    private readonly IRegistroBiometricoRepository _biometricoRepository;
    private readonly IAnamneseAlimentarRepository _anamneseRepository;

    public UserProfileService(
        IApplicationUserRepository applicationUserService,
        ICalculadoraNutricional calculadora,
        IBusca busca,
        IUnitOfWork unitOfWork,
        IPerfilNutricionalRepository perfilRepository,
        IMetaNutricionalRepository metaRepository,
        IBaseRepository<RestricaoAlimentar> restricaoRepository,
        IBaseRepository<PerfilEquipamento> equipamentoRepository,
        IHistoricoClinicoRepository historicoRepository,
        IPreferenciaAlimentarRepository preferenciaRepository,
        IRegistroBiometricoRepository biometricoRepository,
        IAnamneseAlimentarRepository anamneseRepository)
    {
        _applicationUserService = applicationUserService;
        _calculadora = calculadora;
        _busca = busca;
        _unitOfWork = unitOfWork;
        _perfilRepository = perfilRepository;
        _metaRepository = metaRepository;
        _restricaoRepository = restricaoRepository;
        _equipamentoRepository = equipamentoRepository;
        _historicoRepository = historicoRepository;
        _preferenciaRepository = preferenciaRepository;
        _biometricoRepository = biometricoRepository;
        _anamneseRepository = anamneseRepository;
    }

    // ===================== PERFIL NUTRICIONAL =====================

    public async Task<RetornoPadrao> PostPerfilNutricional(PerfilNutricionalDto perfil)
    {
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            var user = await _applicationUserService.FindByEmailAsync(perfil.UserEmail);
            if (user == null)
            {
                await _unitOfWork.RollbackAsync();
                return RetornoPadrao.NaoEncontrado("Usuário não encontrado.");
            }

            if (await _perfilRepository.ExistePorEmailAsync(perfil.UserEmail))
            {
                await _unitOfWork.RollbackAsync();
                return RetornoPadrao.Conflito("Perfil nutricional já existe para este usuário.");
            }

            var registroInicial = new RegistroBiometrico
            {
                CircunferenciaCinturaCm = perfil.CircunferenciaCinturaCm,
                PercentualGordura = perfil.PercentualGorduraCorporal,
                PesoKg = perfil.PesoAtualKg,
                Data = DateTime.UtcNow
            };

            var novoPerfil = new PerfilNutricional
            {
                UserId = user.Id,
                User = user,
                AlturaCm = perfil.AlturaCm,
                PesoAtualKg = perfil.PesoAtualKg,
                PercentualGorduraCorporal = perfil.PercentualGorduraCorporal,
                FatorAtividade = perfil.FatorAtividade,
                OcupacaoProfissional = perfil.OcupacaoProfissional,
                PossuiDoencasPreExistentes = perfil.PossuiDoencasPreExistentes,
                DescricaoCondicoesMedicas = perfil.DescricaoCondicoesMedicas,
                PesoDesejadoKg = perfil.PesoDesejadoKg,
                RefeicoesPorDiaDesejadas = perfil.RefeicoesPorDiaDesejadas,
                TempoDisponivelPreparoMinutos = perfil.TempoDisponivelPreparoMinutos,
                CircunferenciaCinturaCm = perfil.CircunferenciaCinturaCm,
                CircunferenciaQuadrilCm = perfil.CircunferenciaQuadrilCm,
                CircunferenciaBracoCm = perfil.CircunferenciaBracoCm,
                DataNascimento = DateTimeHelper.EnsureUtcDateTime(perfil.DataNascimento),
                Genero = perfil.Genero,
                Objetivo = perfil.Objetivo,
                NivelAtividade = perfil.NivelAtividade,
                PreferenciaDieta = perfil.PreferenciaDieta,
                HabilidadeCulinaria = perfil.HabilidadeCulinaria,
                OrcamentoMensal = perfil.OrcamentoMensal,
                Fumante = perfil.Fumante,
                QualidadeSono = perfil.QualidadeSono,
                HorasSonoPorNoite = perfil.HorasSonoPorNoite,
                RestricoesAlimentares = perfil.RestricoesIds
                    .Select(alergiaEnum => new RestricaoAlimentar
                    {
                        CompostoOrganico = alergiaEnum
                    }).ToList(),
                EquipamentoDisponivel = perfil.EquipamentosIds
                    .Select(enumValue => new PerfilEquipamento
                    {
                        Equipamento = enumValue
                    }).ToList(),
                PreferenciasAlimentares = perfil.Preferencias
                    .Select(pref => new PreferenciaAlimentar
                    {
                        AlimentoId = pref.AlimentoId,
                        Tabela = pref.Tabela,
                        Tipo = pref.Tipo
                    }).ToList(),
                HistoricosClinico = perfil.HistoricoClinicos
                    .Select(hc => new HistoricoClinico
                    {
                        Condicao = hc.Condicao,
                        DescricaoOutra = hc.DescricaoOutra,
                        DataDiagnostico = hc.DataDiagnostico,
                        AtivaAtualmente = hc.AtivaAtualmente,
                        MedicamentosEmUso = hc.MedicamentosEmUso,
                        Observacoes = hc.Observacoes
                    }).ToList(),
                HistoricoMedidas = new List<RegistroBiometrico>
                {
                    registroInicial
                },
            };

            _perfilRepository.Add(novoPerfil);
            await _unitOfWork.SaveChangesAsync();


            var metaNutricional = _calculadora.GerarMetaInicial(novoPerfil);
            metaNutricional.PerfilNutricionalId = novoPerfil.Id;
            _metaRepository.Add(metaNutricional);
            await _unitOfWork.SaveChangesAsync();

            novoPerfil.MetaNutricionalAtualId = metaNutricional.Id;
            await _unitOfWork.SaveChangesAsync();

            await _unitOfWork.CommitAsync();

            return RetornoPadrao.Criado("Perfil nutricional criado com sucesso.");
        }
        catch (Exception)
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }

    public async Task<RetornoPadrao> AtualizarPerfilNutricional(string userId, PerfilNutricionalDto dto)
    {
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            var perfil = await _perfilRepository.ObterComColecoesAsync(userId);

            if (perfil == null)
            {
                await _unitOfWork.RollbackAsync();
                return RetornoPadrao.NaoEncontrado("Perfil nutricional não encontrado para o usuário.");
            }

            // Atualiza campos simples
            perfil.AlturaCm = dto.AlturaCm;
            perfil.PesoAtualKg = dto.PesoAtualKg;
            perfil.PercentualGorduraCorporal = dto.PercentualGorduraCorporal;
            perfil.FatorAtividade = dto.FatorAtividade;
            perfil.OcupacaoProfissional = dto.OcupacaoProfissional;
            perfil.PossuiDoencasPreExistentes = dto.PossuiDoencasPreExistentes;
            perfil.DescricaoCondicoesMedicas = dto.DescricaoCondicoesMedicas;
            perfil.PesoDesejadoKg = dto.PesoDesejadoKg;
            perfil.RefeicoesPorDiaDesejadas = dto.RefeicoesPorDiaDesejadas;
            perfil.TempoDisponivelPreparoMinutos = dto.TempoDisponivelPreparoMinutos;
            perfil.CircunferenciaCinturaCm = dto.CircunferenciaCinturaCm;
            perfil.CircunferenciaQuadrilCm = dto.CircunferenciaQuadrilCm;
            perfil.CircunferenciaBracoCm = dto.CircunferenciaBracoCm;
            perfil.DataNascimento = dto.DataNascimento;
            perfil.Genero = dto.Genero;
            perfil.Objetivo = dto.Objetivo;
            perfil.NivelAtividade = dto.NivelAtividade;
            perfil.PreferenciaDieta = dto.PreferenciaDieta;
            perfil.HabilidadeCulinaria = dto.HabilidadeCulinaria;
            perfil.OrcamentoMensal = dto.OrcamentoMensal;
            perfil.Fumante = dto.Fumante;
            perfil.QualidadeSono = dto.QualidadeSono;
            perfil.HorasSonoPorNoite = dto.HorasSonoPorNoite;
            perfil.AtualizadoEm = DateTime.UtcNow;

            // Atualiza restrições (remove e recria)
            _restricaoRepository.RemoveRange(perfil.RestricoesAlimentares);
            perfil.RestricoesAlimentares = dto.RestricoesIds
                .Select(alergiaEnum => new RestricaoAlimentar
                {
                    PerfilNutricionalId = perfil.Id,
                    CompostoOrganico = alergiaEnum
                }).ToList();

            // Atualiza equipamentos (remove e recria)
            _equipamentoRepository.RemoveRange(perfil.EquipamentoDisponivel);
            perfil.EquipamentoDisponivel = dto.EquipamentosIds
                .Select(enumValue => new PerfilEquipamento
                {
                    PerfilNutricionalId = perfil.Id,
                    Equipamento = enumValue
                }).ToList();

            // Atualiza histórico clínico (remove e recria)
            _historicoRepository.RemoveRange(perfil.HistoricosClinico);
            perfil.HistoricosClinico = dto.HistoricoClinicos
                .Select(hc => new HistoricoClinico
                {
                    PerfilNutricionalId = perfil.Id,
                    Condicao = hc.Condicao,
                    DescricaoOutra = hc.DescricaoOutra,
                    DataDiagnostico = hc.DataDiagnostico,
                    AtivaAtualmente = hc.AtivaAtualmente,
                    MedicamentosEmUso = hc.MedicamentosEmUso,
                    Observacoes = hc.Observacoes
                }).ToList();

            // Recalcula meta nutricional
            var novaMeta = _calculadora.GerarMetaInicial(perfil);
            novaMeta.PerfilNutricionalId = perfil.Id;
            _metaRepository.Add(novaMeta);
            await _unitOfWork.SaveChangesAsync();

            perfil.MetaNutricionalAtualId = novaMeta.Id;

            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitAsync();

            return RetornoPadrao.Ok("Perfil nutricional atualizado com sucesso.");
        }
        catch (Exception)
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }

    // ===================== PREFERÊNCIAS =====================

    public async Task<RetornoPadrao> PostPreferenciaAlimentar(string userId, int id, ETipoTabela tabela, ETipoPreferencia afinidade)
    {
        var perfil = await _perfilRepository.ObterPorUsuarioIdAsync(userId);

        if (perfil == null)
            return RetornoPadrao.NaoEncontrado("Perfil nutricional não encontrado para o usuário.");

        var alimento = await _busca.BuscaAlimentoPorIdAsync(id, tabela);

        if (alimento == null)
            return RetornoPadrao.NaoEncontrado("Alimento não encontrado.");

        // Verifica se já existe preferência para este alimento
        var existente = await _preferenciaRepository
            .ObterPorPerfilEAlimentoAsync(perfil.Id, id, tabela);

        string mensagem;
        if (existente != null)
        {
            // Atualiza a preferência existente
            existente.Tipo = afinidade;
            mensagem = "Preferência alimentar atualizada com sucesso.";
        }
        else
        {
            var preferencia = new PreferenciaAlimentar
            {
                PerfilNutricionalId = perfil.Id,
                AlimentoId = alimento.Id,
                Tabela = tabela,
                Tipo = afinidade
            };
            _preferenciaRepository.Add(preferencia);
            mensagem = "Preferência alimentar registrada com sucesso.";
        }

        await _unitOfWork.SaveChangesAsync();

        return RetornoPadrao.Ok(mensagem);
    }

    public async Task<RetornoPadrao> RemoverPreferenciaAlimentar(string userId, int preferenciaId)
    {
        var preferencia = await _preferenciaRepository
            .ObterPorIdEUsuarioAsync(preferenciaId, userId);

        if (preferencia == null)
            return RetornoPadrao.NaoEncontrado("Preferência alimentar não encontrada.");

        _preferenciaRepository.Remove(preferencia);
        await _unitOfWork.SaveChangesAsync();

        return RetornoPadrao.Ok("Preferência alimentar removida com sucesso.");
    }

    // ===================== BIOMÉTRICO =====================

    public async Task<RetornoPadrao> PostRegistroBiometrico(string userId, RegistroBiometricoDto registroBiometricoDto)
    {
        var user = await _applicationUserService.FindByIdAsync(userId);
        if (user == null)
            return RetornoPadrao.NaoEncontrado("Usuário não encontrado.");

        var perfil = await _perfilRepository.ObterPorUsuarioIdAsync(userId);

        if (perfil == null)
            return RetornoPadrao.NaoEncontrado("Perfil nutricional não encontrado para o usuário.");

        var novoRegistroBiometrico = new RegistroBiometrico
        {
            CircunferenciaCinturaCm = registroBiometricoDto.CircunferenciaCinturaCm,
            PercentualGordura = registroBiometricoDto.PercentualGordura,
            PesoKg = registroBiometricoDto.PesoKg,
            Data = DateTime.UtcNow,
            PerfilNutricionalId = perfil.Id
        };

        // Atualiza o peso atual do perfil
        perfil.PesoAtualKg = registroBiometricoDto.PesoKg;
        if (registroBiometricoDto.PercentualGordura.HasValue)
            perfil.PercentualGorduraCorporal = registroBiometricoDto.PercentualGordura;
        if (registroBiometricoDto.CircunferenciaCinturaCm.HasValue)
            perfil.CircunferenciaCinturaCm = registroBiometricoDto.CircunferenciaCinturaCm;

        perfil.AtualizadoEm = DateTime.UtcNow;

        _biometricoRepository.Add(novoRegistroBiometrico);
        await _unitOfWork.SaveChangesAsync();

        return RetornoPadrao.Criado("Registro biométrico adicionado com sucesso.");
    }

    public async Task<RetornoPadrao<List<RegistroBiometricoDto>>> ListarHistoricoBiometrico(string userId)
    {
        var perfil = await _perfilRepository.ObterPorUsuarioIdAsync(userId);

        if (perfil == null)
            return RetornoPadrao<List<RegistroBiometricoDto>>.NaoEncontrado(
                "Perfil nutricional não encontrado. Conclua o onboarding primeiro.");

        var registros = await _biometricoRepository.ListarPorPerfilAsync(perfil.Id);

        var historico = registros
            .Select(r => new RegistroBiometricoDto
            {
                PesoKg = r.PesoKg,
                PercentualGordura = r.PercentualGordura,
                CircunferenciaCinturaCm = r.CircunferenciaCinturaCm
            })
            .ToList();

        return RetornoPadrao<List<RegistroBiometricoDto>>.Ok(historico);
    }

    // ===================== HISTÓRICO CLÍNICO =====================

    public async Task<RetornoPadrao> AdicionarHistoricoClinico(string userId, HistoricoClinicoDto dto)
    {
        var perfil = await _perfilRepository.ObterPorUsuarioIdAsync(userId);

        if (perfil == null)
            return RetornoPadrao.NaoEncontrado("Perfil nutricional não encontrado.");

        var historico = new HistoricoClinico
        {
            PerfilNutricionalId = perfil.Id,
            Condicao = dto.Condicao,
            DescricaoOutra = dto.DescricaoOutra,
            DataDiagnostico = dto.DataDiagnostico,
            AtivaAtualmente = dto.AtivaAtualmente,
            MedicamentosEmUso = dto.MedicamentosEmUso,
            Observacoes = dto.Observacoes
        };

        _historicoRepository.Add(historico);
        await _unitOfWork.SaveChangesAsync();

        return RetornoPadrao.Criado("Condição clínica registrada com sucesso.");
    }

    public async Task<RetornoPadrao> AtualizarHistoricoClinico(string userId, int id, HistoricoClinicoDto dto)
    {
        var historico = await _historicoRepository.ObterPorIdEUsuarioAsync(id, userId);

        if (historico == null)
            return RetornoPadrao.NaoEncontrado("Registro clínico não encontrado.");

        historico.Condicao = dto.Condicao;
        historico.DescricaoOutra = dto.DescricaoOutra;
        historico.DataDiagnostico = dto.DataDiagnostico;
        historico.AtivaAtualmente = dto.AtivaAtualmente;
        historico.MedicamentosEmUso = dto.MedicamentosEmUso;
        historico.Observacoes = dto.Observacoes;
        historico.AtualizadoEm = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync();

        return RetornoPadrao.Ok("Registro clínico atualizado com sucesso.");
    }

    public async Task<RetornoPadrao> RemoverHistoricoClinico(string userId, int id)
    {
        var historico = await _historicoRepository.ObterPorIdEUsuarioAsync(id, userId);

        if (historico == null)
            return RetornoPadrao.NaoEncontrado("Registro clínico não encontrado.");

        _historicoRepository.Remove(historico);
        await _unitOfWork.SaveChangesAsync();

        return RetornoPadrao.Ok("Registro clínico removido com sucesso.");
    }

    public async Task<RetornoPadrao<List<HistoricoClinicoDto>>> ListarHistoricoClinico(string userId)
    {
        var perfil = await _perfilRepository.ObterPorUsuarioIdAsync(userId);

        if (perfil == null)
            return RetornoPadrao<List<HistoricoClinicoDto>>.NaoEncontrado(
                "Perfil nutricional não encontrado. Conclua o onboarding primeiro.");

        var registros = await _historicoRepository.ListarPorPerfilAsync(perfil.Id);

        var historicos = registros
            .Select(h => new HistoricoClinicoDto
            {
                Condicao = h.Condicao,
                DescricaoOutra = h.DescricaoOutra,
                DataDiagnostico = h.DataDiagnostico,
                AtivaAtualmente = h.AtivaAtualmente,
                MedicamentosEmUso = h.MedicamentosEmUso,
                Observacoes = h.Observacoes
            })
            .ToList();

        return RetornoPadrao<List<HistoricoClinicoDto>>.Ok(historicos);
    }

    // ===================== ANAMNESE ALIMENTAR =====================

    public async Task<RetornoPadrao> SalvarAnamneseAlimentar(string userId, AnamneseAlimentarDto dto)
    {
        var perfil = await _perfilRepository.ObterPorUsuarioIdAsync(userId);

        if (perfil == null)
            return RetornoPadrao.NaoEncontrado("Perfil nutricional não encontrado.");

        var anamnese = new AnamneseAlimentar
        {
            PerfilNutricionalId = perfil.Id,
            DataPreenchimento = DateTime.UtcNow,
            RefeicoesPorDia = dto.RefeicoesPorDia,
            HorarioCafeManha = dto.HorarioCafeManha,
            HorarioAlmoco = dto.HorarioAlmoco,
            HorarioLancheTarde = dto.HorarioLancheTarde,
            HorarioJantar = dto.HorarioJantar,
            HorarioCeia = dto.HorarioCeia,
            RefeicoesPuladas = dto.RefeicoesPuladas,
            ConsumoAguaLitrosDia = dto.ConsumoAguaLitrosDia,
            ConsumoRefrigerantes = dto.ConsumoRefrigerantes,
            ConsumoAlcool = dto.ConsumoAlcool,
            ConsumoCafeCha = dto.ConsumoCafeCha,
            ConsumoFastFood = dto.ConsumoFastFood,
            ConsumoFrutas = dto.ConsumoFrutas,
            ConsumoVerduras = dto.ConsumoVerduras,
            ConsumoDoces = dto.ConsumoDoces,
            ConsumoFrituras = dto.ConsumoFrituras,
            ComeComDistracao = dto.ComeComDistracao,
            CompulsaoAlimentar = dto.CompulsaoAlimentar,
            HistoricoDietasRestritivas = dto.HistoricoDietasRestritivas,
            DescricaoDietasAnteriores = dto.DescricaoDietasAnteriores,
            SuplementosEmUso = dto.SuplementosEmUso,
            IntestinoRegular = dto.IntestinoRegular,
            FrequenciaEvacuacaoSemana = dto.FrequenciaEvacuacaoSemana,
            QueixasDigestivas = dto.QueixasDigestivas,
            AlimentosQueNaoGosta = dto.AlimentosQueNaoGosta,
            AlimentosPreferidos = dto.AlimentosPreferidos,
            ObservacoesGerais = dto.ObservacoesGerais
        };

        _anamneseRepository.Add(anamnese);
        await _unitOfWork.SaveChangesAsync();

        return RetornoPadrao.Criado("Anamnese alimentar registrada com sucesso.");
    }

    public async Task<RetornoPadrao<AnamneseAlimentarDto>> ObterUltimaAnamnese(string userId)
    {
        var perfil = await _perfilRepository.ObterPorUsuarioIdAsync(userId);

        if (perfil == null)
            return RetornoPadrao<AnamneseAlimentarDto>.NaoEncontrado(
                "Perfil nutricional não encontrado. Conclua o onboarding primeiro.");

        var anamnese = await _anamneseRepository.ObterUltimaPorPerfilAsync(perfil.Id);

        if (anamnese == null)
            return RetornoPadrao<AnamneseAlimentarDto>.NaoEncontrado("Nenhuma anamnese encontrada.");

        return RetornoPadrao<AnamneseAlimentarDto>.Ok(MapAnamneseToDto(anamnese));
    }

    public async Task<RetornoPadrao<List<AnamneseAlimentarDto>>> ListarAnamneses(string userId)
    {
        var perfil = await _perfilRepository.ObterPorUsuarioIdAsync(userId);

        if (perfil == null)
            return RetornoPadrao<List<AnamneseAlimentarDto>>.NaoEncontrado(
                "Perfil nutricional não encontrado. Conclua o onboarding primeiro.");

        var registros = await _anamneseRepository.ListarPorPerfilAsync(perfil.Id);

        var anamneses = registros.Select(MapAnamneseToDto).ToList();

        return RetornoPadrao<List<AnamneseAlimentarDto>>.Ok(anamneses);
    }

    // ===================== PERFIL NUTRICIONAL GET =====================

    public async Task<RetornoPadrao<PerfilNutricionalDto>> GetPerfilNutricional(string userId)
    {
        var perfil = await _perfilRepository.ObterComColecoesAsync(userId);

        // Perfil ausente é o caso de primeiro acesso: 404 para o client redirecionar ao onboarding.
        if (perfil == null)
            return RetornoPadrao<PerfilNutricionalDto>.NaoEncontrado(
                "Perfil nutricional não encontrado. Conclua o onboarding primeiro.");

        var perfilDto = new PerfilNutricionalDto
        {
            AlturaCm = perfil.AlturaCm,
            PesoAtualKg = perfil.PesoAtualKg,
            PercentualGorduraCorporal = perfil.PercentualGorduraCorporal,
            FatorAtividade = perfil.FatorAtividade,
            OcupacaoProfissional = perfil.OcupacaoProfissional,
            PossuiDoencasPreExistentes = perfil.PossuiDoencasPreExistentes,
            DescricaoCondicoesMedicas = perfil.DescricaoCondicoesMedicas,
            PesoDesejadoKg = perfil.PesoDesejadoKg,
            RefeicoesPorDiaDesejadas = perfil.RefeicoesPorDiaDesejadas,
            TempoDisponivelPreparoMinutos = perfil.TempoDisponivelPreparoMinutos,
            CircunferenciaCinturaCm = perfil.CircunferenciaCinturaCm,
            CircunferenciaQuadrilCm = perfil.CircunferenciaQuadrilCm,
            CircunferenciaBracoCm = perfil.CircunferenciaBracoCm,
            DataNascimento = perfil.DataNascimento,
            Genero = perfil.Genero,
            Objetivo = perfil.Objetivo,
            NivelAtividade = perfil.NivelAtividade,
            PreferenciaDieta = perfil.PreferenciaDieta,
            HabilidadeCulinaria = perfil.HabilidadeCulinaria,
            OrcamentoMensal = perfil.OrcamentoMensal,
            Fumante = perfil.Fumante,
            QualidadeSono = perfil.QualidadeSono,
            HorasSonoPorNoite = perfil.HorasSonoPorNoite,
            RestricoesIds = perfil.RestricoesAlimentares
                .Select(r => r.CompostoOrganico)
                .ToList(),
            EquipamentosIds = perfil.EquipamentoDisponivel
                .Select(e => e.Equipamento)
                .ToList(),
            Preferencias = perfil.PreferenciasAlimentares != null
                ? perfil.PreferenciasAlimentares
                    .Select(pa => new PreferenciaCadastroDto
                    {
                        AlimentoId = pa.AlimentoId,
                        Tabela = pa.Tabela,
                        Tipo = pa.Tipo
                    }).ToList()
                : new List<PreferenciaCadastroDto>(),
            HistoricoClinicos = perfil.HistoricosClinico
                .Select(hc => new HistoricoClinicoDto
                {
                    Condicao = hc.Condicao,
                    DescricaoOutra = hc.DescricaoOutra,
                    DataDiagnostico = hc.DataDiagnostico,
                    AtivaAtualmente = hc.AtivaAtualmente,
                    MedicamentosEmUso = hc.MedicamentosEmUso,
                    Observacoes = hc.Observacoes
                }).ToList()
        };

        return RetornoPadrao<PerfilNutricionalDto>.Ok(perfilDto);
    }

    // ===================== HELPERS =====================

    private static AnamneseAlimentarDto MapAnamneseToDto(AnamneseAlimentar a)
    {
        return new AnamneseAlimentarDto
        {
            RefeicoesPorDia = a.RefeicoesPorDia,
            HorarioCafeManha = a.HorarioCafeManha,
            HorarioAlmoco = a.HorarioAlmoco,
            HorarioLancheTarde = a.HorarioLancheTarde,
            HorarioJantar = a.HorarioJantar,
            HorarioCeia = a.HorarioCeia,
            RefeicoesPuladas = a.RefeicoesPuladas,
            ConsumoAguaLitrosDia = a.ConsumoAguaLitrosDia,
            ConsumoRefrigerantes = a.ConsumoRefrigerantes,
            ConsumoAlcool = a.ConsumoAlcool,
            ConsumoCafeCha = a.ConsumoCafeCha,
            ConsumoFastFood = a.ConsumoFastFood,
            ConsumoFrutas = a.ConsumoFrutas,
            ConsumoVerduras = a.ConsumoVerduras,
            ConsumoDoces = a.ConsumoDoces,
            ConsumoFrituras = a.ConsumoFrituras,
            ComeComDistracao = a.ComeComDistracao,
            CompulsaoAlimentar = a.CompulsaoAlimentar,
            HistoricoDietasRestritivas = a.HistoricoDietasRestritivas,
            DescricaoDietasAnteriores = a.DescricaoDietasAnteriores,
            SuplementosEmUso = a.SuplementosEmUso,
            IntestinoRegular = a.IntestinoRegular,
            FrequenciaEvacuacaoSemana = a.FrequenciaEvacuacaoSemana,
            QueixasDigestivas = a.QueixasDigestivas,
            AlimentosQueNaoGosta = a.AlimentosQueNaoGosta,
            AlimentosPreferidos = a.AlimentosPreferidos,
            ObservacoesGerais = a.ObservacoesGerais
        };
    }
}
