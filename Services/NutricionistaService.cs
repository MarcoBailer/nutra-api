using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Nutra.Data;
using Nutra.Enum;
using Nutra.Interfaces;
using Nutra.Models;
using Nutra.Models.Dtos;
using Nutra.Models.Usuario;

namespace Nutra.Services;

public class NutricionistaService : INutricionista
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly AlimentosContext _context;
    private readonly ILogger<NutricionistaService> _logger;

    public NutricionistaService(
        UserManager<ApplicationUser> userManager,
        AlimentosContext context,
        ILogger<NutricionistaService> logger)
    {
        _userManager = userManager;
        _context = context;
        _logger = logger;
    }

    // ===================== PERFIL PROFISSIONAL =====================

    public async Task<RetornoPadrao> CadastrarNutricionistaAsync(CadastroNutricionistaDto dto)
    {
        var retorno = new RetornoPadrao();

        // Verifica se já existe user com esse e-mail
        var user = await _userManager.FindByEmailAsync(dto.Email);

        if (user == null)
        {
            user = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                NomeCompleto = dto.NomeCompleto,
                CPF = dto.CPF,
                Telefone = dto.Telefone,
                Role = ETipoRole.Nutricionista,
                EmailConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            var createResult = await _userManager.CreateAsync(user);
            if (!createResult.Succeeded)
            {
                retorno.Sucesso = false;
                retorno.Mensagem = "Erro ao criar conta: " + string.Join("; ", createResult.Errors.Select(e => e.Description));
                return retorno;
            }
        }
        else
        {
            // Se já existe mas já é nutricionista, retorna erro
            var perfilExistente = await _context.PerfisProfissionais
                .AnyAsync(p => p.UserId == user.Id);

            if (perfilExistente)
            {
                retorno.Sucesso = false;
                retorno.Mensagem = "Este usuário já possui um perfil profissional cadastrado.";
                return retorno;
            }

            user.Role = ETipoRole.Nutricionista;
            user.NomeCompleto = dto.NomeCompleto;
            user.CPF = dto.CPF;
            user.Telefone = dto.Telefone;
            await _userManager.UpdateAsync(user);
        }

        // Verifica unicidade do CRN
        var crnExiste = await _context.PerfisProfissionais
            .AnyAsync(p => p.CRN == dto.CRN);

        if (crnExiste)
        {
            retorno.Sucesso = false;
            retorno.Mensagem = "Já existe um profissional cadastrado com este CRN.";
            return retorno;
        }

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

        _context.PerfisProfissionais.Add(perfilProfissional);
        _context.Assinaturas.Add(assinatura);
        await _context.SaveChangesAsync();

        // Adiciona role no Identity
        await _userManager.AddToRoleAsync(user, "Nutricionista");

        retorno.Sucesso = true;
        retorno.Mensagem = "Perfil profissional cadastrado com sucesso.";
        return retorno;
    }

    public async Task<PerfilProfissionalDto> ObterPerfilProfissionalAsync(string userId)
    {
        var perfil = await _context.PerfisProfissionais
            .Include(p => p.User)
            .Include(p => p.AssinaturaAtiva)
            .Include(p => p.Clinicas.Where(c => c.Ativo))
            .Include(p => p.Pacientes.Where(v => v.Status == EStatusVinculo.Ativo))
            .FirstOrDefaultAsync(p => p.UserId == userId);

        if (perfil == null)
            throw new Exception("Perfil profissional não encontrado.");

        return new PerfilProfissionalDto
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
            TotalPacientesAtivos = perfil.Pacientes.Count,
            MaxPacientes = perfil.MaxPacientes,
            MultiClinicaHabilitado = perfil.MultiClinicaHabilitado,
            TotalClinicas = perfil.Clinicas.Count,
            CriadoEm = perfil.CriadoEm
        };
    }

    public async Task<RetornoPadrao> AtualizarPerfilProfissionalAsync(string userId, UpdatePerfilProfissionalDto dto)
    {
        var retorno = new RetornoPadrao();

        var perfil = await _context.PerfisProfissionais
            .Include(p => p.User)
            .FirstOrDefaultAsync(p => p.UserId == userId);

        if (perfil == null)
        {
            retorno.Sucesso = false;
            retorno.Mensagem = "Perfil profissional não encontrado.";
            return retorno;
        }

        if (dto.Especialidade != null) perfil.Especialidade = dto.Especialidade;
        if (dto.BioProfissional != null) perfil.BioProfissional = dto.BioProfissional;
        if (dto.AnosExperiencia.HasValue) perfil.AnosExperiencia = dto.AnosExperiencia;
        if (dto.NomeCompleto != null) perfil.User.NomeCompleto = dto.NomeCompleto;
        if (dto.Telefone != null) perfil.User.Telefone = dto.Telefone;

        perfil.AtualizadoEm = DateTime.UtcNow;
        perfil.User.AtualizadoEm = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        retorno.Sucesso = true;
        retorno.Mensagem = "Perfil profissional atualizado com sucesso.";
        return retorno;
    }

    // ===================== CLÍNICAS =====================

    public async Task<RetornoPadrao> CriarClinicaAsync(string userId, ClinicaDto dto)
    {
        var retorno = new RetornoPadrao();

        var perfil = await _context.PerfisProfissionais
            .Include(p => p.Clinicas.Where(c => c.Ativo))
            .FirstOrDefaultAsync(p => p.UserId == userId);

        if (perfil == null)
        {
            retorno.Sucesso = false;
            retorno.Mensagem = "Perfil profissional não encontrado.";
            return retorno;
        }

        // Verifica se pode ter múltiplas clínicas
        if (!perfil.MultiClinicaHabilitado && perfil.Clinicas.Any())
        {
            retorno.Sucesso = false;
            retorno.Mensagem = "Seu plano atual não permite múltiplas clínicas. Faça upgrade para o plano Enterprise.";
            return retorno;
        }

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

        _context.Clinicas.Add(clinica);
        await _context.SaveChangesAsync();

        retorno.Sucesso = true;
        retorno.Mensagem = "Clínica criada com sucesso.";
        return retorno;
    }

    public async Task<RetornoPadrao> AtualizarClinicaAsync(string userId, int clinicaId, ClinicaDto dto)
    {
        var retorno = new RetornoPadrao();

        var clinica = await _context.Clinicas
            .Include(c => c.PerfilProfissional)
            .FirstOrDefaultAsync(c => c.Id == clinicaId && c.PerfilProfissional.UserId == userId);

        if (clinica == null)
        {
            retorno.Sucesso = false;
            retorno.Mensagem = "Clínica não encontrada ou sem permissão.";
            return retorno;
        }

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

        await _context.SaveChangesAsync();

        retorno.Sucesso = true;
        retorno.Mensagem = "Clínica atualizada com sucesso.";
        return retorno;
    }

    public async Task<RetornoPadrao> RemoverClinicaAsync(string userId, int clinicaId)
    {
        var retorno = new RetornoPadrao();

        var clinica = await _context.Clinicas
            .Include(c => c.PerfilProfissional)
            .FirstOrDefaultAsync(c => c.Id == clinicaId && c.PerfilProfissional.UserId == userId);

        if (clinica == null)
        {
            retorno.Sucesso = false;
            retorno.Mensagem = "Clínica não encontrada ou sem permissão.";
            return retorno;
        }

        clinica.Ativo = false;
        clinica.AtualizadoEm = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        retorno.Sucesso = true;
        retorno.Mensagem = "Clínica removida com sucesso.";
        return retorno;
    }

    public async Task<List<ClinicaDto>> ListarClinicasAsync(string userId)
    {
        var clinicas = await _context.Clinicas
            .Where(c => c.PerfilProfissional.UserId == userId && c.Ativo)
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
            .ToListAsync();

        return clinicas;
    }

    // ===================== GESTÃO DE PACIENTES (VÍNCULOS) =====================

    public async Task<RetornoPadrao> EnviarConvitePacienteAsync(string nutricionistaUserId, ConviteVinculoDto dto)
    {
        var retorno = new RetornoPadrao();

        var perfil = await _context.PerfisProfissionais
            .Include(p => p.Pacientes.Where(v => v.Status == EStatusVinculo.Ativo || v.Status == EStatusVinculo.Pendente))
            .FirstOrDefaultAsync(p => p.UserId == nutricionistaUserId);

        if (perfil == null)
        {
            retorno.Sucesso = false;
            retorno.Mensagem = "Perfil profissional não encontrado.";
            return retorno;
        }

        // Verifica limite de pacientes
        if (perfil.Pacientes.Count >= perfil.MaxPacientes)
        {
            retorno.Sucesso = false;
            retorno.Mensagem = $"Limite de pacientes atingido ({perfil.MaxPacientes}). Faça upgrade do seu plano.";
            return retorno;
        }

        // Busca o paciente pelo e-mail
        var paciente = await _userManager.FindByEmailAsync(dto.EmailPaciente);
        if (paciente == null)
        {
            retorno.Sucesso = false;
            retorno.Mensagem = "Paciente não encontrado com este e-mail.";
            return retorno;
        }

        // Verifica se já existe vínculo ativo ou pendente
        var vinculoExistente = await _context.VinculosPacienteProfissional
            .AnyAsync(v => v.PacienteUserId == paciente.Id
                        && v.PerfilProfissionalId == perfil.Id
                        && (v.Status == EStatusVinculo.Ativo || v.Status == EStatusVinculo.Pendente));

        if (vinculoExistente)
        {
            retorno.Sucesso = false;
            retorno.Mensagem = "Já existe um vínculo ativo ou pendente com este paciente.";
            return retorno;
        }

        // Valida clínica (se informada)
        if (dto.ClinicaId.HasValue)
        {
            var clinicaValida = await _context.Clinicas
                .AnyAsync(c => c.Id == dto.ClinicaId.Value && c.PerfilProfissionalId == perfil.Id && c.Ativo);

            if (!clinicaValida)
            {
                retorno.Sucesso = false;
                retorno.Mensagem = "Clínica informada não encontrada ou sem permissão.";
                return retorno;
            }
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

        _context.VinculosPacienteProfissional.Add(vinculo);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Convite de vínculo enviado de {NutricionistaId} para {PacienteEmail}",
            nutricionistaUserId, dto.EmailPaciente);

        retorno.Sucesso = true;
        retorno.Mensagem = "Convite enviado com sucesso.";
        return retorno;
    }

    public async Task<RetornoPadrao> ResponderConviteAsync(string pacienteUserId, int vinculoId, bool aceitar)
    {
        var retorno = new RetornoPadrao();

        var vinculo = await _context.VinculosPacienteProfissional
            .FirstOrDefaultAsync(v => v.Id == vinculoId
                                   && v.PacienteUserId == pacienteUserId
                                   && v.Status == EStatusVinculo.Pendente);

        if (vinculo == null)
        {
            retorno.Sucesso = false;
            retorno.Mensagem = "Convite não encontrado ou já respondido.";
            return retorno;
        }

        if (aceitar)
        {
            vinculo.Status = EStatusVinculo.Ativo;
            vinculo.DataAceite = DateTime.UtcNow;
            retorno.Mensagem = "Convite aceito. Vínculo ativo com o nutricionista.";
        }
        else
        {
            vinculo.Status = EStatusVinculo.Recusado;
            retorno.Mensagem = "Convite recusado.";
        }

        await _context.SaveChangesAsync();

        retorno.Sucesso = true;
        return retorno;
    }

    public async Task<RetornoPadrao> EncerrarVinculoAsync(string userId, int vinculoId)
    {
        var retorno = new RetornoPadrao();

        var vinculo = await _context.VinculosPacienteProfissional
            .Include(v => v.Profissional)
            .FirstOrDefaultAsync(v => v.Id == vinculoId
                                   && (v.PacienteUserId == userId || v.Profissional.UserId == userId)
                                   && v.Status == EStatusVinculo.Ativo);

        if (vinculo == null)
        {
            retorno.Sucesso = false;
            retorno.Mensagem = "Vínculo não encontrado ou já encerrado.";
            return retorno;
        }

        vinculo.Status = EStatusVinculo.Inativo;
        vinculo.DataEncerramento = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        retorno.Sucesso = true;
        retorno.Mensagem = "Vínculo encerrado com sucesso.";
        return retorno;
    }

    public async Task<List<PacienteResumoDto>> ListarPacientesAsync(string nutricionistaUserId)
    {
        return await _context.VinculosPacienteProfissional
            .Include(v => v.Paciente)
            .Include(v => v.Clinica)
            .Where(v => v.Profissional.UserId == nutricionistaUserId
                     && (v.Status == EStatusVinculo.Ativo || v.Status == EStatusVinculo.Pendente))
            .Select(v => new PacienteResumoDto
            {
                UserId = v.PacienteUserId,
                NomeCompleto = v.Paciente.NomeCompleto,
                Email = v.Paciente.Email,
                StatusVinculo = v.Status,
                DataVinculo = v.DataConvite,
                Clinica = v.Clinica != null ? v.Clinica.Nome : null,
                PerfilNutricionalCriado = v.Paciente.PerfilAtivo != null
            })
            .ToListAsync();
    }

    public async Task<List<PacienteResumoDto>> ListarNutricionistasAsync(string pacienteUserId)
    {
        return await _context.VinculosPacienteProfissional
            .Include(v => v.Profissional)
                .ThenInclude(p => p.User)
            .Include(v => v.Clinica)
            .Where(v => v.PacienteUserId == pacienteUserId
                     && (v.Status == EStatusVinculo.Ativo || v.Status == EStatusVinculo.Pendente))
            .Select(v => new PacienteResumoDto
            {
                UserId = v.Profissional.UserId,
                NomeCompleto = v.Profissional.User.NomeCompleto,
                Email = v.Profissional.User.Email,
                StatusVinculo = v.Status,
                DataVinculo = v.DataConvite,
                Clinica = v.Clinica != null ? v.Clinica.Nome : null,
                PerfilNutricionalCriado = true
            })
            .ToListAsync();
    }

    // ===================== ASSINATURA =====================

    public async Task<RetornoPadrao> AtualizarPlanoAsync(string userId, EPlanoAssinatura novoPlano)
    {
        var retorno = new RetornoPadrao();

        var perfil = await _context.PerfisProfissionais
            .Include(p => p.AssinaturaAtiva)
            .FirstOrDefaultAsync(p => p.UserId == userId);

        if (perfil == null)
        {
            retorno.Sucesso = false;
            retorno.Mensagem = "Perfil profissional não encontrado.";
            return retorno;
        }

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
            _context.Assinaturas.Add(novaAssinatura);
        }

        perfil.AtualizadoEm = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        retorno.Sucesso = true;
        retorno.Mensagem = $"Plano atualizado para {novoPlano} com sucesso.";
        return retorno;
    }
}
