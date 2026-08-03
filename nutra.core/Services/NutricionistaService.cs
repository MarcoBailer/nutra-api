using Microsoft.Extensions.Logging;
using Nutra.Enum;
using Nutra.Interfaces;
using Nutra.Models;
using Nutra.Models.Dtos;
using Nutra.Models.Usuario;

namespace Nutra.Services;

public class NutricionistaService : INutricionista
{
    private readonly IApplicationUserRepository _applicationUserService;
    private readonly IPerfilProfissionalRepository _perfilProfissionalRepository;
    private readonly IBaseRepository<Assinatura> _assinaturaRepository;
    private readonly IClinicaRepository _clinicaRepository;
    private readonly IVinculoPacienteProfissionalRepository _vinculoRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<NutricionistaService> _logger;

    public NutricionistaService(
        IApplicationUserRepository applicationUserService,
        IPerfilProfissionalRepository perfilProfissionalRepository,
        IBaseRepository<Assinatura> assinaturaRepository,
        IClinicaRepository clinicaRepository,
        IVinculoPacienteProfissionalRepository vinculoRepository,
        IUnitOfWork unitOfWork,
        ILogger<NutricionistaService> logger)
    {
        _applicationUserService = applicationUserService;
        _perfilProfissionalRepository = perfilProfissionalRepository;
        _assinaturaRepository = assinaturaRepository;
        _clinicaRepository = clinicaRepository;
        _vinculoRepository = vinculoRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    // ===================== PERFIL PROFISSIONAL =====================

    public async Task<RetornoPadrao> CadastrarNutricionistaAsync(CadastroNutricionistaDto dto)
    {
        // Verifica se já existe user com esse e-mail
        var user = await _applicationUserService.FindByEmailAsync(dto.Email);

        if (user == null)
        {
            user = new ApplicationUser
            {
                Email = dto.Email,
                NomeCompleto = dto.NomeCompleto,
                CPF = dto.CPF,
                Telefone = dto.Telefone,
            };

            await _applicationUserService.CreateAsync(user);
        }
        else
        {
            // Se já existe mas já é nutricionista, retorna conflito
            var perfilExistente = await _perfilProfissionalRepository
                .ExistePorUsuarioIdAsync(user.Id);

            if (perfilExistente)
                return RetornoPadrao.Conflito("Este usuário já possui um perfil profissional cadastrado.");

            user.NomeCompleto = dto.NomeCompleto;
            user.CPF = dto.CPF;
            user.Telefone = dto.Telefone;
            await _applicationUserService.UpdateAsync(user);
        }

        // Verifica unicidade do CRN
        var crnExiste = await _perfilProfissionalRepository
            .ExistePorCrnAsync(dto.CRN);

        if (crnExiste)
            return RetornoPadrao.Conflito("Já existe um profissional cadastrado com este CRN.");

        var perfilProfissional = new PerfilProfissional
        {
            UserId = user.Id,
            CRN = dto.CRN,
            CRNRegiao = dto.CRNRegiao,
            Especialidade = dto.Especialidade,
            BioProfissional = dto.BioProfissional,
            AnosExperiencia = dto.AnosExperiencia,
            MaxPacientes = 5 // Plano gratuito
        };

        // Cria assinatura trial
        var assinatura = new Assinatura
        {
            PerfilProfissional = perfilProfissional,
            Plano = EPlanoAssinatura.Gratuito,
            Status = EStatusAssinatura.Trial,
            DataInicio = DateTime.UtcNow,
            ValorMensal = 0
        };

        _perfilProfissionalRepository.Add(perfilProfissional);
        _assinaturaRepository.Add(assinatura);
        await _unitOfWork.SaveChangesAsync();

        return RetornoPadrao.Criado("Perfil profissional cadastrado com sucesso.");
    }

    public async Task<RetornoPadrao<PerfilProfissionalDto>> ObterPerfilProfissionalAsync(string userId)
    {
        var perfil = await _perfilProfissionalRepository.ObterComUsuarioEAssinaturaAsync(userId);

        // Perfil ausente é primeiro acesso do profissional: o client redireciona ao cadastro.
        if (perfil == null)
            return RetornoPadrao<PerfilProfissionalDto>.NaoEncontrado(
                "Perfil profissional não encontrado. Conclua o cadastro profissional primeiro.");

        // Contagens vêm do banco: carregar as coleções inteiras só para chamar
        // Count seria trazer linha por linha sem necessidade.
        var totalPacientesAtivos = await _vinculoRepository.ContarAtivosPorPerfilAsync(perfil.Id);
        var totalClinicas = await _clinicaRepository.ContarAtivasPorPerfilAsync(perfil.Id);

        var dto = new PerfilProfissionalDto
        {
            Id = perfil.Id,
            UserId = perfil.UserId,
            NomeCompleto = perfil.User.NomeCompleto,
            Email = perfil.User.Email ?? string.Empty,
            CRN = perfil.CRN,
            CRNRegiao = perfil.CRNRegiao,
            CRNVerificado = perfil.CRNVerificado,
            Especialidade = perfil.Especialidade,
            BioProfissional = perfil.BioProfissional,
            AnosExperiencia = perfil.AnosExperiencia,
            PlanoAtual = perfil.AssinaturaAtiva?.Plano ?? EPlanoAssinatura.Gratuito,
            StatusAssinatura = perfil.AssinaturaAtiva?.Status ?? EStatusAssinatura.Trial,
            TotalPacientesAtivos = totalPacientesAtivos,
            MaxPacientes = perfil.MaxPacientes,
            MultiClinicaHabilitado = perfil.MultiClinicaHabilitado,
            TotalClinicas = totalClinicas,
            CriadoEm = perfil.CriadoEm
        };

        return RetornoPadrao<PerfilProfissionalDto>.Ok(dto);
    }

    public async Task<RetornoPadrao> AtualizarPerfilProfissionalAsync(string userId, UpdatePerfilProfissionalDto dto)
    {
        var perfil = await _perfilProfissionalRepository.ObterComUsuarioEAssinaturaAsync(userId);

        if (perfil == null)
            return RetornoPadrao.NaoEncontrado("Perfil profissional não encontrado.");

        if (dto.Especialidade != null) perfil.Especialidade = dto.Especialidade;
        if (dto.BioProfissional != null) perfil.BioProfissional = dto.BioProfissional;
        if (dto.AnosExperiencia.HasValue) perfil.AnosExperiencia = dto.AnosExperiencia;
        if (dto.NomeCompleto != null) perfil.User.NomeCompleto = dto.NomeCompleto;
        if (dto.Telefone != null) perfil.User.Telefone = dto.Telefone;

        perfil.AtualizadoEm = DateTime.UtcNow;
        perfil.User.AtualizadoEm = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync();

        return RetornoPadrao.Ok("Perfil profissional atualizado com sucesso.");
    }

    // ===================== CLÍNICAS =====================

    public async Task<RetornoPadrao> CriarClinicaAsync(string userId, ClinicaDto dto)
    {
        var perfil = await _perfilProfissionalRepository.ObterPorUsuarioIdAsync(userId);

        if (perfil == null)
            return RetornoPadrao.NaoEncontrado("Perfil profissional não encontrado.");

        // Limite do plano: 403 para o client oferecer upgrade em vez de tratar como erro de dados.
        if (!perfil.MultiClinicaHabilitado && await _clinicaRepository.ContarAtivasPorPerfilAsync(perfil.Id) > 0)
            return RetornoPadrao.Proibido(
                "Seu plano atual não permite múltiplas clínicas. Faça upgrade para o plano Enterprise.");

        var clinica = new Clinica
        {
            PerfilProfissionalId = perfil.Id,
            Nome = dto.Nome,
            CNPJ = dto.CNPJ,
            Telefone = dto.Telefone,
            Email = dto.Email,
            Logradouro = dto.Logradouro,
            Numero = dto.Numero,
            Complemento = dto.Complemento,
            Bairro = dto.Bairro,
            Cidade = dto.Cidade,
            Estado = dto.Estado,
            CEP = dto.CEP
        };

        _clinicaRepository.Add(clinica);
        await _unitOfWork.SaveChangesAsync();

        return RetornoPadrao.Criado("Clínica criada com sucesso.");
    }

    public async Task<RetornoPadrao> AtualizarClinicaAsync(string userId, int clinicaId, ClinicaDto dto)
    {
        var clinica = await _clinicaRepository.ObterPorIdEUsuarioAsync(clinicaId, userId);

        if (clinica == null)
            return RetornoPadrao.NaoEncontrado("Clínica não encontrada ou sem permissão.");

        clinica.Nome = dto.Nome;
        clinica.CNPJ = dto.CNPJ;
        clinica.Telefone = dto.Telefone;
        clinica.Email = dto.Email;
        clinica.Logradouro = dto.Logradouro;
        clinica.Numero = dto.Numero;
        clinica.Complemento = dto.Complemento;
        clinica.Bairro = dto.Bairro;
        clinica.Cidade = dto.Cidade;
        clinica.Estado = dto.Estado;
        clinica.CEP = dto.CEP;
        clinica.AtualizadoEm = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync();

        return RetornoPadrao.Ok("Clínica atualizada com sucesso.");
    }

    public async Task<RetornoPadrao> RemoverClinicaAsync(string userId, int clinicaId)
    {
        var clinica = await _clinicaRepository.ObterPorIdEUsuarioAsync(clinicaId, userId);

        if (clinica == null)
            return RetornoPadrao.NaoEncontrado("Clínica não encontrada ou sem permissão.");

        clinica.Ativo = false;
        clinica.AtualizadoEm = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync();

        return RetornoPadrao.Ok("Clínica removida com sucesso.");
    }

    public async Task<RetornoPadrao<List<ClinicaDto>>> ListarClinicasAsync(string userId)
    {
        var ativas = await _clinicaRepository.ListarAtivasPorUsuarioAsync(userId);

        var clinicas = ativas
            .Select(c => new ClinicaDto
            {
                Nome = c.Nome,
                CNPJ = c.CNPJ,
                Telefone = c.Telefone,
                Email = c.Email,
                Logradouro = c.Logradouro,
                Numero = c.Numero,
                Complemento = c.Complemento,
                Bairro = c.Bairro,
                Cidade = c.Cidade,
                Estado = c.Estado,
                CEP = c.CEP
            })
            .ToList();

        return RetornoPadrao<List<ClinicaDto>>.Ok(clinicas);
    }

    // ===================== GESTÃO DE PACIENTES (VÍNCULOS) =====================

    public async Task<RetornoPadrao> EnviarConvitePacienteAsync(string nutricionistaUserId, ConviteVinculoDto dto)
    {
        var perfil = await _perfilProfissionalRepository.ObterPorUsuarioIdAsync(nutricionistaUserId);

        if (perfil == null)
            return RetornoPadrao.NaoEncontrado("Perfil profissional não encontrado.");

        // Limite do plano: 403 para o client oferecer upgrade.
        var pacientesEmAberto = await _vinculoRepository.ContarEmAbertoPorPerfilAsync(perfil.Id);
        if (pacientesEmAberto >= perfil.MaxPacientes)
            return RetornoPadrao.Proibido(
                $"Limite de pacientes atingido ({perfil.MaxPacientes}). Faça upgrade do seu plano.");

        // Busca o paciente pelo e-mail
        var paciente = await _applicationUserService.FindByEmailAsync(dto.EmailPaciente);
        if (paciente == null)
            return RetornoPadrao.NaoEncontrado("Paciente não encontrado com este e-mail.");

        // Verifica se já existe vínculo ativo ou pendente
        var vinculoExistente = await _vinculoRepository
            .ExisteVinculoEmAbertoPorPerfilAsync(perfil.Id, paciente.Id);

        if (vinculoExistente)
            return RetornoPadrao.Conflito("Já existe um vínculo ativo ou pendente com este paciente.");

        // Valida clínica (se informada)
        if (dto.ClinicaId.HasValue)
        {
            var clinicaValida = await _clinicaRepository
                .ExisteAtivaAsync(dto.ClinicaId.Value, perfil.Id);

            if (!clinicaValida)
                return RetornoPadrao.NaoEncontrado("Clínica informada não encontrada ou sem permissão.");
        }

        var vinculo = new VinculoPacienteProfissional
        {
            PacienteUserId = paciente.Id,
            PerfilProfissionalId = perfil.Id,
            ClinicaId = dto.ClinicaId,
            Observacoes = dto.Observacoes,
            Status = EStatusVinculo.Pendente,
            DataConvite = DateTime.UtcNow
        };

        _vinculoRepository.Add(vinculo);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Convite de vínculo enviado de {NutricionistaId} para {PacienteEmail}",
            nutricionistaUserId, dto.EmailPaciente);

        return RetornoPadrao.Criado("Convite enviado com sucesso.");
    }

    public async Task<RetornoPadrao> ResponderConviteAsync(string pacienteUserId, int vinculoId, bool aceitar)
    {
        var vinculo = await _vinculoRepository
            .ObterConvitePendenteAsync(vinculoId, pacienteUserId);

        if (vinculo == null)
            return RetornoPadrao.NaoEncontrado("Convite não encontrado ou já respondido.");

        string mensagem;
        if (aceitar)
        {
            vinculo.Status = EStatusVinculo.Ativo;
            vinculo.DataAceite = DateTime.UtcNow;
            mensagem = "Convite aceito. Vínculo ativo com o nutricionista.";
        }
        else
        {
            vinculo.Status = EStatusVinculo.Recusado;
            mensagem = "Convite recusado.";
        }

        await _unitOfWork.SaveChangesAsync();

        return RetornoPadrao.Ok(mensagem);
    }

    public async Task<RetornoPadrao> EncerrarVinculoAsync(string userId, int vinculoId)
    {
        var vinculo = await _vinculoRepository
            .ObterVinculoAtivoDoParticipanteAsync(vinculoId, userId);

        if (vinculo == null)
            return RetornoPadrao.NaoEncontrado("Vínculo não encontrado ou já encerrado.");

        vinculo.Status = EStatusVinculo.Inativo;
        vinculo.DataEncerramento = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync();

        return RetornoPadrao.Ok("Vínculo encerrado com sucesso.");
    }

    public async Task<RetornoPadrao<List<PacienteResumoDto>>> ListarPacientesAsync(string nutricionistaUserId)
    {
        var vinculos = await _vinculoRepository
            .ListarPacientesDoProfissionalAsync(nutricionistaUserId);

        var pacientes = vinculos
            .Select(v => new PacienteResumoDto
            {
                UserId = v.PacienteUserId,
                NomeCompleto = v.Paciente.NomeCompleto,
                Email = v.Paciente.Email,
                StatusVinculo = v.Status,
                DataVinculo = v.DataConvite,
                Clinica = v.Clinica?.Nome,
                PerfilNutricionalCriado = v.Paciente.PerfilAtivo != null
            })
            .ToList();

        return RetornoPadrao<List<PacienteResumoDto>>.Ok(pacientes);
    }

    public async Task<RetornoPadrao<List<PacienteResumoDto>>> ListarNutricionistasAsync(string pacienteUserId)
    {
        var vinculos = await _vinculoRepository
            .ListarProfissionaisDoPacienteAsync(pacienteUserId);

        var nutricionistas = vinculos
            .Select(v => new PacienteResumoDto
            {
                UserId = v.Profissional.UserId,
                NomeCompleto = v.Profissional.User.NomeCompleto,
                Email = v.Profissional.User.Email,
                StatusVinculo = v.Status,
                DataVinculo = v.DataConvite,
                Clinica = v.Clinica?.Nome,
                PerfilNutricionalCriado = true
            })
            .ToList();

        return RetornoPadrao<List<PacienteResumoDto>>.Ok(nutricionistas);
    }

    // ===================== ASSINATURA =====================

    public async Task<RetornoPadrao> AtualizarPlanoAsync(string userId, EPlanoAssinatura novoPlano)
    {
        var perfil = await _perfilProfissionalRepository.ObterComUsuarioEAssinaturaAsync(userId);

        if (perfil == null)
            return RetornoPadrao.NaoEncontrado("Perfil profissional não encontrado.");

        // Define limites por plano
        var (maxPacientes, multiClinica, valor) = novoPlano switch
        {
            EPlanoAssinatura.Gratuito => (5, false, 0m),
            EPlanoAssinatura.Basico => (30, false, 49.90m),
            EPlanoAssinatura.Profissional => (100, false, 99.90m),
            EPlanoAssinatura.Enterprise => (int.MaxValue, true, 199.90m),
            _ => (5, false, 0m)
        };

        perfil.MaxPacientes = maxPacientes;
        perfil.MultiClinicaHabilitado = multiClinica;

        if (perfil.AssinaturaAtiva != null)
        {
            perfil.AssinaturaAtiva.Plano = novoPlano;
            perfil.AssinaturaAtiva.Status = EStatusAssinatura.Ativa;
            perfil.AssinaturaAtiva.ValorMensal = valor;
            perfil.AssinaturaAtiva.AtualizadoEm = DateTime.UtcNow;
        }
        else
        {
            var novaAssinatura = new Assinatura
            {
                PerfilProfissionalId = perfil.Id,
                Plano = novoPlano,
                Status = EStatusAssinatura.Ativa,
                DataInicio = DateTime.UtcNow,
                ValorMensal = valor
            };
            _assinaturaRepository.Add(novaAssinatura);
        }

        perfil.AtualizadoEm = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync();

        return RetornoPadrao.Ok($"Plano atualizado para {novoPlano} com sucesso.");
    }
}
