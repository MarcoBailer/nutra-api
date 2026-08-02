using Microsoft.EntityFrameworkCore;
using Nutra.Data;
using Nutra.Enum;
using Nutra.Interfaces;
using Nutra.Models;
using Nutra.Models.Dtos;
using Nutra.Models.RegraNutricional;
using Nutra.Models.Usuario;

namespace Nutra.Services;

/// <summary>
/// Serviço de Avaliação Nutricional — orquestra antropometria, cálculos automáticos e fotos de progresso.
/// </summary>
public class AvaliacaoNutricionalService : IAvaliacaoNutricional
{
    private readonly AlimentosContext _context;
    private readonly ICalculadoraNutricional _calculadora;

    public AvaliacaoNutricionalService(AlimentosContext context, ICalculadoraNutricional calculadora)
    {
        _context = context;
        _calculadora = calculadora;
    }

    // =====================================================================
    //  REGISTRAR AVALIAÇÃO (auto-avaliação)
    // =====================================================================

    public async Task<RetornoPadrao<AvaliacaoAntropometricaResultadoDto>> RegistrarAvaliacaoAsync(
        string userId, AvaliacaoAntropometricaDto dto)
    {
        var perfil = await ObterPerfil(userId);
        if (perfil == null)
            return RetornoPadrao<AvaliacaoAntropometricaResultadoDto>.NaoEncontrado(PerfilAusente);

        var avaliacao = CriarEntidadeAvaliacao(perfil, dto, profissionalId: null);

        _context.AvaliacoesAntropometricas.Add(avaliacao);

        // Atualiza peso e medidas no perfil nutricional ativo
        AtualizarPerfilComAvaliacao(perfil, avaliacao);

        await _context.SaveChangesAsync();

        return RetornoPadrao<AvaliacaoAntropometricaResultadoDto>.Criado(
            MapearResultado(avaliacao, perfil), "Avaliação registrada com sucesso.");
    }

    // =====================================================================
    //  REGISTRAR AVALIAÇÃO POR PROFISSIONAL
    // =====================================================================

    public async Task<RetornoPadrao<AvaliacaoAntropometricaResultadoDto>> RegistrarAvaliacaoPorProfissionalAsync(
        string profissionalUserId, string pacienteUserId, AvaliacaoAntropometricaDto dto)
    {
        // Verifica que o profissional tem vínculo ativo com o paciente
        var vinculo = await _context.VinculosPacienteProfissional
            .Include(v => v.Profissional)
            .FirstOrDefaultAsync(v =>
                v.Profissional.UserId == profissionalUserId &&
                v.PacienteUserId == pacienteUserId &&
                v.Status == EStatusVinculo.Ativo);

        if (vinculo == null)
            return RetornoPadrao<AvaliacaoAntropometricaResultadoDto>.Proibido(
                "Profissional não possui vínculo ativo com este paciente.");

        var perfil = await ObterPerfil(pacienteUserId);
        if (perfil == null)
            return RetornoPadrao<AvaliacaoAntropometricaResultadoDto>.NaoEncontrado(
                "Paciente não possui perfil nutricional.");

        var avaliacao = CriarEntidadeAvaliacao(perfil, dto, profissionalUserId);

        _context.AvaliacoesAntropometricas.Add(avaliacao);
        AtualizarPerfilComAvaliacao(perfil, avaliacao);

        await _context.SaveChangesAsync();

        return RetornoPadrao<AvaliacaoAntropometricaResultadoDto>.Criado(
            MapearResultado(avaliacao, perfil), "Avaliação registrada com sucesso.");
    }

    // =====================================================================
    //  OBTER AVALIAÇÃO POR ID
    // =====================================================================

    public async Task<RetornoPadrao<AvaliacaoAntropometricaResultadoDto>> ObterAvaliacaoPorIdAsync(string userId, int avaliacaoId)
    {
        var perfil = await ObterPerfil(userId);
        if (perfil == null)
            return RetornoPadrao<AvaliacaoAntropometricaResultadoDto>.NaoEncontrado(PerfilAusente);

        var avaliacao = await _context.AvaliacoesAntropometricas
            .Include(a => a.FotosProgresso)
            .Include(a => a.ProfissionalResponsavel)
            .FirstOrDefaultAsync(a => a.Id == avaliacaoId && a.PerfilNutricionalId == perfil.Id);

        if (avaliacao == null)
            return RetornoPadrao<AvaliacaoAntropometricaResultadoDto>.NaoEncontrado("Avaliação não encontrada.");

        return RetornoPadrao<AvaliacaoAntropometricaResultadoDto>.Ok(MapearResultado(avaliacao, perfil));
    }

    // =====================================================================
    //  LISTAR AVALIAÇÕES
    // =====================================================================

    public async Task<RetornoPadrao<List<AvaliacaoResumoDto>>> ListarAvaliacoesAsync(string userId)
    {
        var perfil = await ObterPerfil(userId);
        if (perfil == null)
            return RetornoPadrao<List<AvaliacaoResumoDto>>.NaoEncontrado(PerfilAusente);

        var lista = await _context.AvaliacoesAntropometricas
            .Where(a => a.PerfilNutricionalId == perfil.Id)
            .OrderByDescending(a => a.DataAvaliacao)
            .Select(a => new AvaliacaoResumoDto
            {
                Id = a.Id,
                DataAvaliacao = a.DataAvaliacao,
                PesoKg = a.PesoKg,
                IMC = a.IMC,
                ClassificacaoIMC = a.ClassificacaoIMC,
                PercentualGordura = a.PercentualGorduraEstimado,
                GET = a.GET,
                PossuiBioimpedancia = a.PossuiBioimpedancia,
                PossuiDobrasCutaneas = a.ProtocoloDobrasCutaneas != null,
                TotalFotos = a.FotosProgresso.Count
            })
            .ToListAsync();

        return RetornoPadrao<List<AvaliacaoResumoDto>>.Ok(lista);
    }

    public async Task<RetornoPadrao<List<AvaliacaoResumoDto>>> ListarAvaliacoesDoPacienteAsync(
        string profissionalUserId, string pacienteUserId)
    {
        // Verifica vínculo
        var vinculoExiste = await _context.VinculosPacienteProfissional
            .Include(v => v.Profissional)
            .AnyAsync(v =>
                v.Profissional.UserId == profissionalUserId &&
                v.PacienteUserId == pacienteUserId &&
                (v.Status == EStatusVinculo.Ativo || v.Status == EStatusVinculo.Pendente));

        if (!vinculoExiste)
            return RetornoPadrao<List<AvaliacaoResumoDto>>.Proibido(
                "Profissional não possui vínculo com este paciente.");

        var perfil = await _context.PerfilNutricional
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.UserId == pacienteUserId);

        if (perfil == null)
            return RetornoPadrao<List<AvaliacaoResumoDto>>.NaoEncontrado(
                "Paciente não possui perfil nutricional.");

        var lista = await _context.AvaliacoesAntropometricas
            .Where(a => a.PerfilNutricionalId == perfil.Id)
            .OrderByDescending(a => a.DataAvaliacao)
            .Select(a => new AvaliacaoResumoDto
            {
                Id = a.Id,
                DataAvaliacao = a.DataAvaliacao,
                PesoKg = a.PesoKg,
                IMC = a.IMC,
                ClassificacaoIMC = a.ClassificacaoIMC,
                PercentualGordura = a.PercentualGorduraEstimado,
                GET = a.GET,
                PossuiBioimpedancia = a.PossuiBioimpedancia,
                PossuiDobrasCutaneas = a.ProtocoloDobrasCutaneas != null,
                TotalFotos = a.FotosProgresso.Count
            })
            .ToListAsync();

        return RetornoPadrao<List<AvaliacaoResumoDto>>.Ok(lista);
    }

    // =====================================================================
    //  COMPARAR AVALIAÇÕES
    // =====================================================================

    public async Task<RetornoPadrao<ComparacaoAvaliacoesDto>> CompararAvaliacoesAsync(
        string userId, int avaliacaoAnteriorId, int avaliacaoAtualId)
    {
        var anteriorRetorno = await ObterAvaliacaoPorIdAsync(userId, avaliacaoAnteriorId);
        if (!anteriorRetorno.Sucesso)
            return RetornoPadrao<ComparacaoAvaliacoesDto>.Falha(anteriorRetorno);

        var atualRetorno = await ObterAvaliacaoPorIdAsync(userId, avaliacaoAtualId);
        if (!atualRetorno.Sucesso)
            return RetornoPadrao<ComparacaoAvaliacoesDto>.Falha(atualRetorno);

        var anterior = anteriorRetorno.Dados!;
        var atual = atualRetorno.Dados!;

        var comparacao = new ComparacaoAvaliacoesDto
        {
            AvaliacaoAnterior = anterior,
            AvaliacaoAtual = atual,
            Evolucao = new EvolucaoDto
            {
                DeltaPesoKg = Math.Round(atual.PesoKg - anterior.PesoKg, 2),
                DeltaIMC = Math.Round(atual.IMC - anterior.IMC, 2),
                DeltaPercentualGordura = (atual.ComposicaoCorporal.PercentualGordura.HasValue && anterior.ComposicaoCorporal.PercentualGordura.HasValue)
                    ? Math.Round(atual.ComposicaoCorporal.PercentualGordura.Value - anterior.ComposicaoCorporal.PercentualGordura.Value, 2)
                    : null,
                DeltaMassaMagraKg = (atual.ComposicaoCorporal.MassaMagraKg.HasValue && anterior.ComposicaoCorporal.MassaMagraKg.HasValue)
                    ? Math.Round(atual.ComposicaoCorporal.MassaMagraKg.Value - anterior.ComposicaoCorporal.MassaMagraKg.Value, 2)
                    : null,
                DeltaMassaGordaKg = (atual.ComposicaoCorporal.MassaGordaKg.HasValue && anterior.ComposicaoCorporal.MassaGordaKg.HasValue)
                    ? Math.Round(atual.ComposicaoCorporal.MassaGordaKg.Value - anterior.ComposicaoCorporal.MassaGordaKg.Value, 2)
                    : null,
                DeltaGET = (atual.Calculos.GET > 0 && anterior.Calculos.GET > 0)
                    ? Math.Round(atual.Calculos.GET - anterior.Calculos.GET, 1)
                    : null,
                DeltaCinturaCm = (atual.Circunferencias.CinturaCm.HasValue && anterior.Circunferencias.CinturaCm.HasValue)
                    ? Math.Round(atual.Circunferencias.CinturaCm.Value - anterior.Circunferencias.CinturaCm.Value, 1)
                    : null,
                DeltaQuadrilCm = (atual.Circunferencias.QuadrilCm.HasValue && anterior.Circunferencias.QuadrilCm.HasValue)
                    ? Math.Round(atual.Circunferencias.QuadrilCm.Value - anterior.Circunferencias.QuadrilCm.Value, 1)
                    : null,
                DiasEntreAvaliacoes = (int)(atual.DataAvaliacao - anterior.DataAvaliacao).TotalDays
            }
        };

        return RetornoPadrao<ComparacaoAvaliacoesDto>.Ok(comparacao);
    }

    // =====================================================================
    //  EXCLUIR AVALIAÇÃO
    // =====================================================================

    public async Task<RetornoPadrao> ExcluirAvaliacaoAsync(string userId, int avaliacaoId)
    {
        var perfil = await ObterPerfil(userId);
        if (perfil == null)
            return RetornoPadrao.NaoEncontrado(PerfilAusente);

        var avaliacao = await _context.AvaliacoesAntropometricas
            .Include(a => a.FotosProgresso)
            .FirstOrDefaultAsync(a => a.Id == avaliacaoId && a.PerfilNutricionalId == perfil.Id);

        if (avaliacao == null)
            return RetornoPadrao.NaoEncontrado("Avaliação não encontrada.");

        _context.AvaliacoesAntropometricas.Remove(avaliacao);
        await _context.SaveChangesAsync();

        return RetornoPadrao.Ok("Avaliação excluída com sucesso.");
    }

    // =====================================================================
    //  FOTOS DE PROGRESSO
    // =====================================================================

    public async Task<RetornoPadrao> AdicionarFotosAsync(string userId, int avaliacaoId, List<FotoProgressoDto> fotos)
    {
        var perfil = await ObterPerfil(userId);
        if (perfil == null)
            return RetornoPadrao.NaoEncontrado(PerfilAusente);

        var avaliacao = await _context.AvaliacoesAntropometricas
            .FirstOrDefaultAsync(a => a.Id == avaliacaoId && a.PerfilNutricionalId == perfil.Id);

        if (avaliacao == null)
            return RetornoPadrao.NaoEncontrado("Avaliação não encontrada.");

        foreach (var fotoDto in fotos)
        {
            avaliacao.FotosProgresso.Add(new FotoProgresso
            {
                Url = fotoDto.Url,
                Tipo = fotoDto.Tipo,
                Descricao = fotoDto.Descricao,
                DataFoto = fotoDto.DataFoto ?? DateTime.UtcNow
            });
        }

        await _context.SaveChangesAsync();
        return RetornoPadrao.Criado($"{fotos.Count} foto(s) adicionada(s) com sucesso.");
    }

    public async Task<RetornoPadrao> RemoverFotoAsync(string userId, int fotoId)
    {
        var perfil = await ObterPerfil(userId);
        if (perfil == null)
            return RetornoPadrao.NaoEncontrado(PerfilAusente);

        var foto = await _context.FotosProgresso
            .Include(f => f.AvaliacaoAntropometrica)
            .FirstOrDefaultAsync(f => f.Id == fotoId && f.AvaliacaoAntropometrica.PerfilNutricionalId == perfil.Id);

        if (foto == null)
            return RetornoPadrao.NaoEncontrado("Foto não encontrada.");

        _context.FotosProgresso.Remove(foto);
        await _context.SaveChangesAsync();

        return RetornoPadrao.Ok("Foto removida com sucesso.");
    }

    // =====================================================================
    //  MÉTODOS PRIVADOS — MOTOR DE CÁLCULOS
    // =====================================================================

    /// <summary>
    /// Cria a entidade AvaliacaoAntropometrica preenchendo todos os campos calculados.
    /// </summary>
    private AvaliacaoAntropometrica CriarEntidadeAvaliacao(
        PerfilNutricional perfil, AvaliacaoAntropometricaDto dto, string? profissionalId)
    {
        int idade = CalculadoraNutricionalService.CalcularIdade(perfil.DataNascimento);
        var genero = perfil.Genero;

        // 1. IMC
        var (imc, classificacaoIMC) = _calculadora.CalcularIMC(dto.PesoKg, dto.AlturaCm);

        // 2. TMB — 3 fórmulas
        double tmbMifflin = _calculadora.CalcularTMB_MifflinStJeor(dto.PesoKg, dto.AlturaCm, idade, genero);
        double tmbHarris = _calculadora.CalcularTMB_HarrisBenedict(dto.PesoKg, dto.AlturaCm, idade, genero);
        double? tmbKatch = null;

        // 3. GET (usa Mifflin como padrão)
        double get = _calculadora.CalcularGET(tmbMifflin, perfil.NivelAtividade);

        // 4. Peso ideal
        double pesoIdealDevine = _calculadora.CalcularPesoIdeal_Devine(dto.AlturaCm, genero);
        double pesoIdealIMC = _calculadora.CalcularPesoIdeal_IMC(dto.AlturaCm);

        // 5. Taxa metabólica ajustada ao objetivo
        double taxaAjustada = _calculadora.AjustarCaloriasPeloObjetivo(get, perfil.Objetivo);

        // 6. Macronutrientes
        var (protG, carbG, gordG, fibraG, aguaL) = _calculadora.CalcularMacronutrientes(dto.PesoKg, taxaAjustada, perfil.Objetivo);

        // 7. Composição corporal — determina melhor fonte de %gordura
        decimal? percentualGorduraEstimado = null;
        double? massaMagraKg = null;
        double? massaGordaKg = null;
        decimal? densidadeCorporal = null;
        decimal? percentualGorduraDobras = null;
        double? somatorioDobras = null;

        // 7a. Se tem bioimpedância, usa como fonte primária
        if (dto.PossuiBioimpedancia && dto.BioPercentualGordura.HasValue)
        {
            percentualGorduraEstimado = dto.BioPercentualGordura;
            massaMagraKg = dto.BioMassaMagraKg ?? dto.PesoKg * (1 - (double)dto.BioPercentualGordura.Value / 100);
            massaGordaKg = dto.BioMassaGordaKg ?? dto.PesoKg * ((double)dto.BioPercentualGordura.Value / 100);

            // Calcula Katch-McArdle com massa magra
            tmbKatch = _calculadora.CalcularTMB_KatchMcArdle(massaMagraKg.Value);
        }

        // 7b. Dobras cutâneas
        if (dto.ProtocoloDobrasCutaneas.HasValue)
        {
            var dobrasResult = CalcularDobrasCutaneas(dto, idade, genero);
            if (dobrasResult.HasValue)
            {
                densidadeCorporal = dobrasResult.Value.densidade;
                percentualGorduraDobras = dobrasResult.Value.percentualGordura;
                somatorioDobras = dobrasResult.Value.somatorio;

                // Se não tinha bioimpedância, usa dobras como fonte
                if (!percentualGorduraEstimado.HasValue)
                {
                    percentualGorduraEstimado = percentualGorduraDobras;
                    massaGordaKg = dto.PesoKg * ((double)percentualGorduraDobras.Value / 100);
                    massaMagraKg = dto.PesoKg - massaGordaKg;

                    tmbKatch = _calculadora.CalcularTMB_KatchMcArdle(massaMagraKg.Value);
                }
            }
        }

        // 8. Relação Cintura/Quadril
        decimal? rcq = null;
        if (dto.CircunferenciaCinturaCm.HasValue && dto.CircunferenciaQuadrilCm.HasValue)
        {
            var (rcqCalc, _) = _calculadora.CalcularRCQ(
                dto.CircunferenciaCinturaCm.Value, dto.CircunferenciaQuadrilCm.Value, genero);
            rcq = rcqCalc;
        }

        // Monta a entidade
        var avaliacao = new AvaliacaoAntropometrica
        {
            PerfilNutricionalId = perfil.Id,
            ProfissionalResponsavelId = profissionalId,
            DataAvaliacao = DateTime.UtcNow,
            Observacoes = dto.Observacoes,

            // Básicas
            PesoKg = dto.PesoKg,
            AlturaCm = dto.AlturaCm,
            IMC = imc,
            ClassificacaoIMC = classificacaoIMC,

            // Circunferências
            CircunferenciaPescocoCm = dto.CircunferenciaPescocoCm,
            CircunferenciaToraxCm = dto.CircunferenciaToraxCm,
            CircunferenciaCinturaCm = dto.CircunferenciaCinturaCm,
            CircunferenciaAbdomenCm = dto.CircunferenciaAbdomenCm,
            CircunferenciaQuadrilCm = dto.CircunferenciaQuadrilCm,
            CircunferenciaBracoDireitoCm = dto.CircunferenciaBracoDireitoCm,
            CircunferenciaBracoEsquerdoCm = dto.CircunferenciaBracoEsquerdoCm,
            CircunferenciaAntebracoDireitoCm = dto.CircunferenciaAntebracoDireitoCm,
            CircunferenciaAntebracoEsquerdoCm = dto.CircunferenciaAntebracoEsquerdoCm,
            CircunferenciaCoxaDireitaCm = dto.CircunferenciaCoxaDireitaCm,
            CircunferenciaCoxaEsquerdaCm = dto.CircunferenciaCoxaEsquerdaCm,
            CircunferenciaPanturrilhaDireitaCm = dto.CircunferenciaPanturrilhaDireitaCm,
            CircunferenciaPanturrilhaEsquerdaCm = dto.CircunferenciaPanturrilhaEsquerdaCm,
            RCQ = rcq,

            // Dobras
            ProtocoloDobrasCutaneas = dto.ProtocoloDobrasCutaneas,
            DobraTricepsMm = dto.DobraTricepsMm,
            DobraBicepsMm = dto.DobraBicepsMm,
            DobraSubescapularMm = dto.DobraSubescapularMm,
            DobraSuprailiacaMm = dto.DobraSuprailiacaMm,
            DobraAbdominalMm = dto.DobraAbdominalMm,
            DobraCoxaMm = dto.DobraCoxaMm,
            DobraPanturrilhaMm = dto.DobraPanturrilhaMm,
            DobraAxilarMediaMm = dto.DobraAxilarMediaMm,
            DobraPeitoralMm = dto.DobraPeitoralMm,
            SomatorioDobras = somatorioDobras,
            DensidadeCorporal = densidadeCorporal,
            PercentualGorduraDobrasCutaneas = percentualGorduraDobras,

            // Bioimpedância
            PossuiBioimpedancia = dto.PossuiBioimpedancia,
            BioPercentualGordura = dto.BioPercentualGordura,
            BioMassaMagraKg = dto.BioMassaMagraKg,
            BioMassaGordaKg = dto.BioMassaGordaKg,
            BioAguaCorporalLitros = dto.BioAguaCorporalLitros,
            BioPercentualAgua = dto.BioPercentualAgua,
            BioTMBKcal = dto.BioTMBKcal,
            BioGorduraVisceralNivel = dto.BioGorduraVisceralNivel,
            BioIdadeMetabolica = dto.BioIdadeMetabolica,
            BioMassaOsseaKg = dto.BioMassaOsseaKg,

            // Cálculos
            TMBMifflinStJeor = tmbMifflin,
            TMBHarrisBenedict = tmbHarris,
            TMBKatchMcArdle = tmbKatch,
            GET = get,
            PercentualGorduraEstimado = percentualGorduraEstimado,
            MassaMagraEstimadaKg = massaMagraKg.HasValue ? Math.Round(massaMagraKg.Value, 2) : null,
            MassaGordaEstimadaKg = massaGordaKg.HasValue ? Math.Round(massaGordaKg.Value, 2) : null,
            PesoIdealDevineKg = pesoIdealDevine,
            PesoIdealIMCKg = pesoIdealIMC,
            TaxaMetabolicaAjustada = taxaAjustada,
            ProteinaRecomendadaG = protG,
            CarboidratoRecomendadoG = carbG,
            GorduraRecomendadaG = gordG,
        };

        // Fotos de progresso
        if (dto.FotosProgresso != null)
        {
            foreach (var fotoDto in dto.FotosProgresso)
            {
                avaliacao.FotosProgresso.Add(new FotoProgresso
                {
                    Url = fotoDto.Url,
                    Tipo = fotoDto.Tipo,
                    Descricao = fotoDto.Descricao,
                    DataFoto = fotoDto.DataFoto ?? DateTime.UtcNow
                });
            }
        }

        return avaliacao;
    }

    /// <summary>
    /// Calcula %gordura por dobras cutâneas de acordo com o protocolo selecionado.
    /// </summary>
    private (decimal densidade, decimal percentualGordura, double somatorio)? CalcularDobrasCutaneas(
        AvaliacaoAntropometricaDto dto, int idade, EGeneroBiologico genero)
    {
        try
        {
            double[] dobras;
            decimal densidade;
            decimal percentual;

            switch (dto.ProtocoloDobrasCutaneas)
            {
                case EProtocoloDobrasCutaneas.JacksonPollock3:
                    if (genero == EGeneroBiologico.Masculino)
                    {
                        if (!dto.DobraPeitoralMm.HasValue || !dto.DobraAbdominalMm.HasValue || !dto.DobraCoxaMm.HasValue)
                            return null;
                        dobras = new[] { dto.DobraPeitoralMm.Value, dto.DobraAbdominalMm.Value, dto.DobraCoxaMm.Value };
                    }
                    else
                    {
                        if (!dto.DobraTricepsMm.HasValue || !dto.DobraSuprailiacaMm.HasValue || !dto.DobraCoxaMm.HasValue)
                            return null;
                        dobras = new[] { dto.DobraTricepsMm.Value, dto.DobraSuprailiacaMm.Value, dto.DobraCoxaMm.Value };
                    }

                    (densidade, percentual) = _calculadora.CalcularGorduraPorDobras_JP3(dobras, idade, genero);
                    return (densidade, percentual, dobras.Sum());

                case EProtocoloDobrasCutaneas.JacksonPollock7:
                    if (!dto.DobraPeitoralMm.HasValue || !dto.DobraAxilarMediaMm.HasValue ||
                        !dto.DobraTricepsMm.HasValue || !dto.DobraSubescapularMm.HasValue ||
                        !dto.DobraAbdominalMm.HasValue || !dto.DobraSuprailiacaMm.HasValue ||
                        !dto.DobraCoxaMm.HasValue)
                        return null;

                    dobras = new[]
                    {
                        dto.DobraPeitoralMm.Value, dto.DobraAxilarMediaMm.Value,
                        dto.DobraTricepsMm.Value, dto.DobraSubescapularMm.Value,
                        dto.DobraAbdominalMm.Value, dto.DobraSuprailiacaMm.Value,
                        dto.DobraCoxaMm.Value
                    };

                    (densidade, percentual) = _calculadora.CalcularGorduraPorDobras_JP7(dobras, idade, genero);
                    return (densidade, percentual, dobras.Sum());

                default:
                    // Guedes3 e Petroski: usam JP3 como fallback com mesmas dobras de tríceps, suprailíaca, coxa
                    if (!dto.DobraTricepsMm.HasValue || !dto.DobraSuprailiacaMm.HasValue || !dto.DobraCoxaMm.HasValue)
                        return null;

                    dobras = genero == EGeneroBiologico.Masculino
                        ? new[] { dto.DobraTricepsMm.Value, dto.DobraSuprailiacaMm.Value, dto.DobraAbdominalMm ?? 0 }
                        : new[] { dto.DobraTricepsMm.Value, dto.DobraSuprailiacaMm.Value, dto.DobraCoxaMm.Value };

                    (densidade, percentual) = _calculadora.CalcularGorduraPorDobras_JP3(dobras, idade, genero);
                    return (densidade, percentual, dobras.Sum());
            }
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Atualiza o perfil nutricional com os dados mais recentes da avaliação.
    /// </summary>
    private void AtualizarPerfilComAvaliacao(PerfilNutricional perfil, AvaliacaoAntropometrica avaliacao)
    {
        perfil.PesoAtualKg = avaliacao.PesoKg;
        perfil.AlturaCm = avaliacao.AlturaCm;
        perfil.CircunferenciaCinturaCm = avaliacao.CircunferenciaCinturaCm;
        perfil.CircunferenciaQuadrilCm = avaliacao.CircunferenciaQuadrilCm;
        perfil.CircunferenciaBracoCm = avaliacao.CircunferenciaBracoDireitoCm;

        if (avaliacao.PercentualGorduraEstimado.HasValue)
            perfil.PercentualGorduraCorporal = (double)avaliacao.PercentualGorduraEstimado.Value;

        perfil.AtualizadoEm = DateTime.UtcNow;
    }

    /// <summary>
    /// Mapeia a entidade persistida para o DTO de resultado completo.
    /// </summary>
    private AvaliacaoAntropometricaResultadoDto MapearResultado(
        AvaliacaoAntropometrica a, PerfilNutricional perfil)
    {
        var resultado = new AvaliacaoAntropometricaResultadoDto
        {
            Id = a.Id,
            DataAvaliacao = a.DataAvaliacao,
            ProfissionalResponsavel = a.ProfissionalResponsavel?.NomeCompleto,
            Observacoes = a.Observacoes,

            PesoKg = a.PesoKg,
            AlturaCm = a.AlturaCm,
            IMC = a.IMC,
            ClassificacaoIMC = a.ClassificacaoIMC,

            Circunferencias = new CircunferenciasDto
            {
                PescocoCm = a.CircunferenciaPescocoCm,
                ToraxCm = a.CircunferenciaToraxCm,
                CinturaCm = a.CircunferenciaCinturaCm,
                AbdomenCm = a.CircunferenciaAbdomenCm,
                QuadrilCm = a.CircunferenciaQuadrilCm,
                BracoDireitoCm = a.CircunferenciaBracoDireitoCm,
                BracoEsquerdoCm = a.CircunferenciaBracoEsquerdoCm,
                AntebracoDireitoCm = a.CircunferenciaAntebracoDireitoCm,
                AntebracoEsquerdoCm = a.CircunferenciaAntebracoEsquerdoCm,
                CoxaDireitaCm = a.CircunferenciaCoxaDireitaCm,
                CoxaEsquerdaCm = a.CircunferenciaCoxaEsquerdaCm,
                PanturrilhaDireitaCm = a.CircunferenciaPanturrilhaDireitaCm,
                PanturrilhaEsquerdaCm = a.CircunferenciaPanturrilhaEsquerdaCm,
                RCQ = a.RCQ,
            },

            Calculos = new CalculosMetabolicosDto
            {
                TMBMifflinStJeor = a.TMBMifflinStJeor ?? 0,
                TMBHarrisBenedict = a.TMBHarrisBenedict ?? 0,
                TMBKatchMcArdle = a.TMBKatchMcArdle,
                GET = a.GET ?? 0,
                TaxaMetabolicaAjustada = a.TaxaMetabolicaAjustada ?? 0
            },

            ComposicaoCorporal = new ComposicaoCorporalDto
            {
                PercentualGordura = a.PercentualGorduraEstimado,
                FontePercentualGordura = DeterminarFonteGordura(a),
                MassaMagraKg = a.MassaMagraEstimadaKg,
                MassaGordaKg = a.MassaGordaEstimadaKg,
                PesoIdealDevineKg = a.PesoIdealDevineKg ?? 0,
                PesoIdealIMCKg = a.PesoIdealIMCKg ?? 0,
                DiferencaPesoIdealKg = Math.Round(a.PesoKg - (a.PesoIdealIMCKg ?? a.PesoKg), 1)
            },

            MacrosRecomendados = new MacronutrientesRecomendadosDto
            {
                CaloriasAlvo = a.TaxaMetabolicaAjustada ?? 0,
                ProteinaG = a.ProteinaRecomendadaG ?? 0,
                CarboidratoG = a.CarboidratoRecomendadoG ?? 0,
                GorduraG = a.GorduraRecomendadaG ?? 0,
                FibraG = Math.Round(((a.TaxaMetabolicaAjustada ?? 0) / 1000) * 14),
                AguaLitros = Math.Round(a.PesoKg * 0.035, 1),
            },

            FotosProgresso = a.FotosProgresso?.Select(f => new FotoProgressoDto
            {
                Url = f.Url,
                Tipo = f.Tipo,
                Descricao = f.Descricao,
                DataFoto = f.DataFoto
            }).ToList() ?? new()
        };

        // Calcula percentuais dos macros
        double totalCals = resultado.MacrosRecomendados.CaloriasAlvo;
        if (totalCals > 0)
        {
            resultado.MacrosRecomendados.PercentualProteina = Math.Round((resultado.MacrosRecomendados.ProteinaG * 4 / totalCals) * 100, 1);
            resultado.MacrosRecomendados.PercentualCarboidrato = Math.Round((resultado.MacrosRecomendados.CarboidratoG * 4 / totalCals) * 100, 1);
            resultado.MacrosRecomendados.PercentualGordura = Math.Round((resultado.MacrosRecomendados.GorduraG * 9 / totalCals) * 100, 1);
        }

        // Classificação RCQ
        if (a.RCQ.HasValue && a.CircunferenciaCinturaCm.HasValue && a.CircunferenciaQuadrilCm.HasValue)
        {
            var (_, classRcq) = _calculadora.CalcularRCQ(
                a.CircunferenciaCinturaCm.Value, a.CircunferenciaQuadrilCm.Value, perfil.Genero);
            resultado.Circunferencias.ClassificacaoRCQ = classRcq;
        }

        // Dobras cutâneas
        if (a.ProtocoloDobrasCutaneas.HasValue && a.DensidadeCorporal.HasValue)
        {
            resultado.DobrasCutaneas = new DobrasCutaneasResultadoDto
            {
                Protocolo = a.ProtocoloDobrasCutaneas.Value,
                TricepsMm = a.DobraTricepsMm,
                BicepsMm = a.DobraBicepsMm,
                SubescapularMm = a.DobraSubescapularMm,
                SuprailiacaMm = a.DobraSuprailiacaMm,
                AbdominalMm = a.DobraAbdominalMm,
                CoxaMm = a.DobraCoxaMm,
                PanturrilhaMm = a.DobraPanturrilhaMm,
                AxilarMediaMm = a.DobraAxilarMediaMm,
                PeitoralMm = a.DobraPeitoralMm,
                SomatorioDobras = a.SomatorioDobras ?? 0,
                DensidadeCorporal = a.DensidadeCorporal.Value,
                PercentualGorduraEstimado = a.PercentualGorduraDobrasCutaneas ?? 0
            };
        }

        // Bioimpedância
        if (a.PossuiBioimpedancia && a.BioPercentualGordura.HasValue)
        {
            resultado.Bioimpedancia = new BioimpedanciaResultadoDto
            {
                PercentualGordura = a.BioPercentualGordura.Value,
                MassaMagraKg = a.BioMassaMagraKg ?? 0,
                MassaGordaKg = a.BioMassaGordaKg ?? 0,
                AguaCorporalLitros = a.BioAguaCorporalLitros,
                PercentualAgua = a.BioPercentualAgua,
                TMBKcal = a.BioTMBKcal,
                GorduraVisceralNivel = a.BioGorduraVisceralNivel,
                IdadeMetabolica = a.BioIdadeMetabolica,
                MassaOsseaKg = a.BioMassaOsseaKg
            };
        }

        return resultado;
    }

    private static string DeterminarFonteGordura(AvaliacaoAntropometrica a)
    {
        if (a.PossuiBioimpedancia && a.BioPercentualGordura.HasValue)
            return "Bioimpedância";
        if (a.PercentualGorduraDobrasCutaneas.HasValue)
            return $"Dobras cutâneas ({a.ProtocoloDobrasCutaneas})";
        return "Não disponível";
    }

    /// <summary>
    /// Helper privado: null quando o perfil não existe. Quem envelopa e escolhe
    /// o status HTTP é o método público.
    /// </summary>
    private Task<PerfilNutricional?> ObterPerfil(string userId) =>
        _context.PerfilNutricional.FirstOrDefaultAsync(p => p.UserId == userId);

    private const string PerfilAusente =
        "Perfil nutricional não encontrado. Crie o perfil antes de registrar uma avaliação.";
}
