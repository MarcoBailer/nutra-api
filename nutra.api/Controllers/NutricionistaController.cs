using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nutra.Enum;
using Nutra.Interfaces;
using Nutra.Models.Dtos;
using System.Security.Claims;

namespace Nutra.Controllers;

/// <summary>
/// Controller para gestão de nutricionistas: perfil profissional, clínicas, pacientes e assinatura.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Nutricionista,Admin")]
public class NutricionistaController : ControllerBase
{
    private readonly INutricionista _nutricionista;

    public NutricionistaController(INutricionista nutricionista)
    {
        _nutricionista = nutricionista;
    }

    private string GetUserId() =>
        User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? throw new UnauthorizedAccessException("Usuário não autenticado.");

    // ===================== CADASTRO / PERFIL =====================

    /// <summary>
    /// Cadastra um novo profissional nutricionista (pode ser chamado sem role se for auto-cadastro).
    /// </summary>
    [HttpPost("cadastro")]
    [AllowAnonymous]
    public async Task<IActionResult> Cadastrar([FromBody] CadastroNutricionistaDto dto)
    {
        try
        {
            var resultado = await _nutricionista.CadastrarNutricionistaAsync(dto);
            return resultado.Sucesso ? Ok(resultado) : BadRequest(resultado);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Sucesso = false, Mensagem = ex.Message });
        }
    }

    /// <summary>
    /// Obtém o perfil profissional do nutricionista logado.
    /// </summary>
    [HttpGet("perfil")]
    public async Task<IActionResult> ObterPerfil()
    {
        try
        {
            var userId = GetUserId();
            var perfil = await _nutricionista.ObterPerfilProfissionalAsync(userId);
            return Ok(perfil);
        }
        catch (Exception ex)
        {
            return NotFound(new { Sucesso = false, Mensagem = ex.Message });
        }
    }

    /// <summary>
    /// Atualiza dados do perfil profissional.
    /// </summary>
    [HttpPut("perfil")]
    public async Task<IActionResult> AtualizarPerfil([FromBody] UpdatePerfilProfissionalDto dto)
    {
        try
        {
            var userId = GetUserId();
            var resultado = await _nutricionista.AtualizarPerfilProfissionalAsync(userId, dto);
            return resultado.Sucesso ? Ok(resultado) : BadRequest(resultado);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Sucesso = false, Mensagem = ex.Message });
        }
    }

    // ===================== CLÍNICAS =====================

    /// <summary>
    /// Lista clínicas ativas do nutricionista.
    /// </summary>
    [HttpGet("clinicas")]
    public async Task<IActionResult> ListarClinicas()
    {
        try
        {
            var userId = GetUserId();
            var clinicas = await _nutricionista.ListarClinicasAsync(userId);
            return Ok(clinicas);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Sucesso = false, Mensagem = ex.Message });
        }
    }

    /// <summary>
    /// Cria uma nova clínica.
    /// </summary>
    [HttpPost("clinicas")]
    public async Task<IActionResult> CriarClinica([FromBody] ClinicaDto dto)
    {
        try
        {
            var userId = GetUserId();
            var resultado = await _nutricionista.CriarClinicaAsync(userId, dto);
            return resultado.Sucesso ? Ok(resultado) : BadRequest(resultado);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Sucesso = false, Mensagem = ex.Message });
        }
    }

    /// <summary>
    /// Atualiza uma clínica existente.
    /// </summary>
    [HttpPut("clinicas/{clinicaId}")]
    public async Task<IActionResult> AtualizarClinica(int clinicaId, [FromBody] ClinicaDto dto)
    {
        try
        {
            var userId = GetUserId();
            var resultado = await _nutricionista.AtualizarClinicaAsync(userId, clinicaId, dto);
            return resultado.Sucesso ? Ok(resultado) : BadRequest(resultado);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Sucesso = false, Mensagem = ex.Message });
        }
    }

    /// <summary>
    /// Remove (soft delete) uma clínica.
    /// </summary>
    [HttpDelete("clinicas/{clinicaId}")]
    public async Task<IActionResult> RemoverClinica(int clinicaId)
    {
        try
        {
            var userId = GetUserId();
            var resultado = await _nutricionista.RemoverClinicaAsync(userId, clinicaId);
            return resultado.Sucesso ? Ok(resultado) : BadRequest(resultado);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Sucesso = false, Mensagem = ex.Message });
        }
    }

    // ===================== GESTÃO DE PACIENTES =====================

    /// <summary>
    /// Lista pacientes vinculados ao nutricionista.
    /// </summary>
    [HttpGet("pacientes")]
    public async Task<IActionResult> ListarPacientes()
    {
        try
        {
            var userId = GetUserId();
            var pacientes = await _nutricionista.ListarPacientesAsync(userId);
            return Ok(pacientes);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Sucesso = false, Mensagem = ex.Message });
        }
    }

    /// <summary>
    /// Envia convite de vínculo para um paciente (por e-mail).
    /// </summary>
    [HttpPost("pacientes/convite")]
    public async Task<IActionResult> EnviarConvite([FromBody] ConviteVinculoDto dto)
    {
        try
        {
            var userId = GetUserId();
            var resultado = await _nutricionista.EnviarConvitePacienteAsync(userId, dto);
            return resultado.Sucesso ? Ok(resultado) : BadRequest(resultado);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Sucesso = false, Mensagem = ex.Message });
        }
    }

    /// <summary>
    /// Encerra um vínculo com paciente.
    /// </summary>
    [HttpDelete("pacientes/vinculo/{vinculoId}")]
    public async Task<IActionResult> EncerrarVinculo(int vinculoId)
    {
        try
        {
            var userId = GetUserId();
            var resultado = await _nutricionista.EncerrarVinculoAsync(userId, vinculoId);
            return resultado.Sucesso ? Ok(resultado) : BadRequest(resultado);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Sucesso = false, Mensagem = ex.Message });
        }
    }

    // ===================== ASSINATURA =====================

    /// <summary>
    /// Atualiza o plano de assinatura do nutricionista.
    /// </summary>
    [HttpPut("assinatura/{novoPlano}")]
    public async Task<IActionResult> AtualizarPlano(EPlanoAssinatura novoPlano)
    {
        try
        {
            var userId = GetUserId();
            var resultado = await _nutricionista.AtualizarPlanoAsync(userId, novoPlano);
            return resultado.Sucesso ? Ok(resultado) : BadRequest(resultado);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Sucesso = false, Mensagem = ex.Message });
        }
    }
}
