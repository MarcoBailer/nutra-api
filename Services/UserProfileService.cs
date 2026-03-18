using Microsoft.EntityFrameworkCore;
using Nutra.Data;
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
    private readonly IApplicationUserService _applicationUserService;
    private readonly ICalculadoraNutricional _calculadora;
    private readonly IBusca _busca;
    private readonly AlimentosContext _context;

    public UserProfileService(IApplicationUserService applicationUserService,
        AlimentosContext context,
        ICalculadoraNutricional calculadora,
        IBusca busca
        )
    {
        _applicationUserService = applicationUserService;
        _context = context;
        _calculadora = calculadora;
        _busca = busca;
    }

    // ===================== PERFIL NUTRICIONAL =====================

    public async Task<RetornoPadrao> PostPerfilNutricional(PerfilNutricionalDto perfil)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var user = await _applicationUserService.FindByEmailAsync(perfil.UserEmail);
            if (user == null)
                throw new Exception("Usuário não encontrado.");

            bool perfilJaExiste = _context.PerfilNutricional.Any(p => p.User.Email == perfil.UserEmail);
            if (perfilJaExiste)
                throw new Exception("Perfil nutricional já existe para este usuário.");

            var retorno = new RetornoPadrao();

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

            _context.PerfilNutricional.Add(novoPerfil);
            await _context.SaveChangesAsync();


            var metaNutricional = _calculadora.GerarMetaInicial(novoPerfil);
            metaNutricional.PerfilNutricionalId = novoPerfil.Id;
            _context.MetasNutricionais.Add(metaNutricional);
            await _context.SaveChangesAsync();

            novoPerfil.MetaNutricionalAtualId = metaNutricional.Id;
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();

            retorno.Sucesso = true;
            retorno.Mensagem = "Perfil nutricional criado com sucesso.";

            return retorno;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<RetornoPadrao> AtualizarPerfilNutricional(string userId, PerfilNutricionalDto dto)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var retorno = new RetornoPadrao();

            var perfil = await _context.PerfilNutricional
                .Include(p => p.RestricoesAlimentares)
                .Include(p => p.EquipamentoDisponivel)
                .Include(p => p.HistoricosClinico)
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (perfil == null)
            {
                retorno.Sucesso = false;
                retorno.Mensagem = "Perfil nutricional não encontrado para o usuário.";
                return retorno;
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
            _context.RestricaoAlimentar.RemoveRange(perfil.RestricoesAlimentares);
            perfil.RestricoesAlimentares = dto.RestricoesIds
                .Select(alergiaEnum => new RestricaoAlimentar
                {
                    PerfilNutricionalId = perfil.Id,
                    CompostoOrganico = alergiaEnum
                }).ToList();

            // Atualiza equipamentos (remove e recria)
            _context.PerfisEquipamentos.RemoveRange(perfil.EquipamentoDisponivel);
            perfil.EquipamentoDisponivel = dto.EquipamentosIds
                .Select(enumValue => new PerfilEquipamento
                {
                    PerfilNutricionalId = perfil.Id,
                    Equipamento = enumValue
                }).ToList();

            // Atualiza histórico clínico (remove e recria)
            _context.HistoricoClinicos.RemoveRange(perfil.HistoricosClinico);
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
            _context.MetasNutricionais.Add(novaMeta);
            await _context.SaveChangesAsync();

            perfil.MetaNutricionalAtualId = novaMeta.Id;

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            retorno.Sucesso = true;
            retorno.Mensagem = "Perfil nutricional atualizado com sucesso.";
            return retorno;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    // ===================== PREFERÊNCIAS =====================

    public async Task<RetornoPadrao> PostPreferenciaAlimentar(string userId, int id, ETipoTabela tabela, ETipoPreferencia afinidade)
    {
        var retorno = new RetornoPadrao();

        var perfil = await _context.PerfilNutricional
            .FirstOrDefaultAsync(p => p.UserId == userId);

        if (perfil == null)
        {
            retorno.Sucesso = false;
            retorno.Mensagem = "Perfil nutricional não encontrado para o usuário.";
            return retorno;
        }

        var alimento = await _busca.BuscaAlimentoPorIdAsync(id, tabela);

        if (alimento == null)
        {
            retorno.Sucesso = false;
            retorno.Mensagem = "Alimento não encontrado.";
            return retorno;
        }

        // Verifica se já existe preferência para este alimento
        var existente = await _context.PreferenciaAlimentar
            .FirstOrDefaultAsync(p => p.PerfilNutricionalId == perfil.Id
                                   && p.AlimentoId == id
                                   && p.Tabela == tabela);

        if (existente != null)
        {
            // Atualiza a preferência existente
            existente.Tipo = afinidade;
            retorno.Mensagem = "Preferência alimentar atualizada com sucesso.";
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
            _context.PreferenciaAlimentar.Add(preferencia);
            retorno.Mensagem = "Preferência alimentar registrada com sucesso.";
        }

        await _context.SaveChangesAsync();

        retorno.Sucesso = true;
        return retorno;
    }

    public async Task<RetornoPadrao> RemoverPreferenciaAlimentar(string userId, int preferenciaId)
    {
        var retorno = new RetornoPadrao();

        var preferencia = await _context.PreferenciaAlimentar
            .Include(p => p.Perfil)
            .FirstOrDefaultAsync(p => p.Id == preferenciaId && p.Perfil.UserId == userId);

        if (preferencia == null)
        {
            retorno.Sucesso = false;
            retorno.Mensagem = "Preferência alimentar não encontrada.";
            return retorno;
        }

        _context.PreferenciaAlimentar.Remove(preferencia);
        await _context.SaveChangesAsync();

        retorno.Sucesso = true;
        retorno.Mensagem = "Preferência alimentar removida com sucesso.";
        return retorno;
    }

    // ===================== BIOMÉTRICO =====================

    public async Task<RetornoPadrao> PostRegistroBiometrico(string userId, RegistroBiometricoDto registroBiometricoDto)
    {
        var user = await _applicationUserService.FindByIdAsync(userId);
        if (user == null)
            throw new Exception("Usuário não encontrado.");

        var retorno = new RetornoPadrao();
        var perfil = await _context.PerfilNutricional
            .FirstOrDefaultAsync(p => p.UserId == userId);

        if (perfil == null)
            throw new Exception("Perfil nutricional não encontrado para o usuário.");

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

        _context.RegistroBiometrico.Add(novoRegistroBiometrico);
        await _context.SaveChangesAsync();

        retorno.Sucesso = true;
        retorno.Mensagem = "Registro biométrico adicionado com sucesso.";

        return retorno;
    }

    public async Task<List<RegistroBiometricoDto>> ListarHistoricoBiometrico(string userId)
    {
        var perfil = await _context.PerfilNutricional
            .FirstOrDefaultAsync(p => p.UserId == userId);

        if (perfil == null)
            throw new Exception("Perfil nutricional não encontrado.");

        return await _context.RegistroBiometrico
            .Where(r => r.PerfilNutricionalId == perfil.Id)
            .OrderByDescending(r => r.Data)
            .Select(r => new RegistroBiometricoDto
            {
                PesoKg = r.PesoKg,
                PercentualGordura = r.PercentualGordura,
                CircunferenciaCinturaCm = r.CircunferenciaCinturaCm
            })
            .ToListAsync();
    }

    // ===================== HISTÓRICO CLÍNICO =====================

    public async Task<RetornoPadrao> AdicionarHistoricoClinico(string userId, HistoricoClinicoDto dto)
    {
        var retorno = new RetornoPadrao();

        var perfil = await _context.PerfilNutricional
            .FirstOrDefaultAsync(p => p.UserId == userId);

        if (perfil == null)
        {
            retorno.Sucesso = false;
            retorno.Mensagem = "Perfil nutricional não encontrado.";
            return retorno;
        }

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

        _context.HistoricoClinicos.Add(historico);
        await _context.SaveChangesAsync();

        retorno.Sucesso = true;
        retorno.Mensagem = "Condição clínica registrada com sucesso.";
        return retorno;
    }

    public async Task<RetornoPadrao> AtualizarHistoricoClinico(string userId, int id, HistoricoClinicoDto dto)
    {
        var retorno = new RetornoPadrao();

        var historico = await _context.HistoricoClinicos
            .Include(h => h.PerfilNutricional)
            .FirstOrDefaultAsync(h => h.Id == id && h.PerfilNutricional.UserId == userId);

        if (historico == null)
        {
            retorno.Sucesso = false;
            retorno.Mensagem = "Registro clínico não encontrado.";
            return retorno;
        }

        historico.Condicao = dto.Condicao;
        historico.DescricaoOutra = dto.DescricaoOutra;
        historico.DataDiagnostico = dto.DataDiagnostico;
        historico.AtivaAtualmente = dto.AtivaAtualmente;
        historico.MedicamentosEmUso = dto.MedicamentosEmUso;
        historico.Observacoes = dto.Observacoes;
        historico.AtualizadoEm = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        retorno.Sucesso = true;
        retorno.Mensagem = "Registro clínico atualizado com sucesso.";
        return retorno;
    }

    public async Task<RetornoPadrao> RemoverHistoricoClinico(string userId, int id)
    {
        var retorno = new RetornoPadrao();

        var historico = await _context.HistoricoClinicos
            .Include(h => h.PerfilNutricional)
            .FirstOrDefaultAsync(h => h.Id == id && h.PerfilNutricional.UserId == userId);

        if (historico == null)
        {
            retorno.Sucesso = false;
            retorno.Mensagem = "Registro clínico não encontrado.";
            return retorno;
        }

        _context.HistoricoClinicos.Remove(historico);
        await _context.SaveChangesAsync();

        retorno.Sucesso = true;
        retorno.Mensagem = "Registro clínico removido com sucesso.";
        return retorno;
    }

    public async Task<List<HistoricoClinicoDto>> ListarHistoricoClinico(string userId)
    {
        var perfil = await _context.PerfilNutricional
            .FirstOrDefaultAsync(p => p.UserId == userId);

        if (perfil == null)
            throw new Exception("Perfil nutricional não encontrado.");

        return await _context.HistoricoClinicos
            .Where(h => h.PerfilNutricionalId == perfil.Id)
            .OrderByDescending(h => h.CriadoEm)
            .Select(h => new HistoricoClinicoDto
            {
                Condicao = h.Condicao,
                DescricaoOutra = h.DescricaoOutra,
                DataDiagnostico = h.DataDiagnostico,
                AtivaAtualmente = h.AtivaAtualmente,
                MedicamentosEmUso = h.MedicamentosEmUso,
                Observacoes = h.Observacoes
            })
            .ToListAsync();
    }

    // ===================== ANAMNESE ALIMENTAR =====================

    public async Task<RetornoPadrao> SalvarAnamneseAlimentar(string userId, AnamneseAlimentarDto dto)
    {
        var retorno = new RetornoPadrao();

        var perfil = await _context.PerfilNutricional
            .FirstOrDefaultAsync(p => p.UserId == userId);

        if (perfil == null)
        {
            retorno.Sucesso = false;
            retorno.Mensagem = "Perfil nutricional não encontrado.";
            return retorno;
        }

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

        _context.AnamnesesAlimentares.Add(anamnese);
        await _context.SaveChangesAsync();

        retorno.Sucesso = true;
        retorno.Mensagem = "Anamnese alimentar registrada com sucesso.";
        return retorno;
    }

    public async Task<AnamneseAlimentarDto?> ObterUltimaAnamnese(string userId)
    {
        var perfil = await _context.PerfilNutricional
            .FirstOrDefaultAsync(p => p.UserId == userId);

        if (perfil == null)
            return null;

        var anamnese = await _context.AnamnesesAlimentares
            .Where(a => a.PerfilNutricionalId == perfil.Id)
            .OrderByDescending(a => a.DataPreenchimento)
            .FirstOrDefaultAsync();

        if (anamnese == null) return null;

        return MapAnamneseToDto(anamnese);
    }

    public async Task<List<AnamneseAlimentarDto>> ListarAnamneses(string userId)
    {
        var perfil = await _context.PerfilNutricional
            .FirstOrDefaultAsync(p => p.UserId == userId);

        if (perfil == null)
            throw new Exception("Perfil nutricional não encontrado.");

        return await _context.AnamnesesAlimentares
            .Where(a => a.PerfilNutricionalId == perfil.Id)
            .OrderByDescending(a => a.DataPreenchimento)
            .Select(a => MapAnamneseToDto(a))
            .ToListAsync();
    }

    // ===================== PERFIL NUTRICIONAL GET =====================

    public async Task<PerfilNutricionalDto> GetPerfilNutricional(string userId)
    {
        var perfil = await _context.PerfilNutricional
            .Include(p => p.RestricoesAlimentares)
            .Include(p => p.EquipamentoDisponivel)
            .Include(p => p.PreferenciasAlimentares)
            .Include(p => p.HistoricosClinico)
            .FirstOrDefaultAsync(p => p.UserId == userId);

        if (perfil == null)
            throw new Exception("Perfil nutricional não encontrado para o usuário.");

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

        return perfilDto;
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
