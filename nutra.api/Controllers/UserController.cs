using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nutra.Enum;
using Nutra.Interfaces;
using Nutra.Models.Dtos;
using System.Security.Claims;

namespace Nutra.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UserController : ControllerBase
{
    private readonly IUserProfile _userProfile;

    public UserController(IUserProfile userProfile)
    {
        _userProfile = userProfile;
    }

    private string GetUserId() =>
        User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? throw new UnauthorizedAccessException("Usuario nao autenticado.");

    // ===================== PERFIL NUTRICIONAL =====================

    [HttpPost("perfil-nutricional")]
    public async Task<IActionResult> PostPerfilNutricional([FromBody] PerfilNutricionalDto perfilNutricional)
    {
        try
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email)
                          ?? User.FindFirstValue("email")
                          ?? throw new UnauthorizedAccessException("Usu�rio n�o autenticado.");
            perfilNutricional.UserEmail = userEmail;
            var perfil = await _userProfile.PostPerfilNutricional(perfilNutricional);
            return Ok(perfil);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Sucesso = false, Mensagem = ex.Message });
        }
    }

    [HttpPut("perfil-nutricional")]
    public async Task<IActionResult> AtualizarPerfilNutricional([FromBody] PerfilNutricionalDto perfilNutricional)
    {
        try
        {
            var userId = GetUserId();
            var resultado = await _userProfile.AtualizarPerfilNutricional(userId, perfilNutricional);
            return resultado.Sucesso ? Ok(resultado) : BadRequest(resultado);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Sucesso = false, Mensagem = ex.Message });
        }
    }

    [HttpGet("buscar-perfil-nutricional")]
    public async Task<IActionResult> GetPerfilNutricional()
    {
        try
        {
            var userId = GetUserId();
            var perfil = await _userProfile.GetPerfilNutricional(userId);
            return Ok(perfil);
        }
        catch (Exception ex)
        {
            return NotFound(new { Sucesso = false, Mensagem = ex.Message });
        }
    }

    // ===================== PREFERENCIAS ALIMENTARES =====================

    [HttpPost("preferencia-alimentar/{id}/{tabela}/{afinidade}")]
    public async Task<IActionResult> PostPreferenciaAlimentar(int id, ETipoTabela tabela, ETipoPreferencia afinidade)
    {
        try
        {
            var userId = GetUserId();
            var resultado = await _userProfile.PostPreferenciaAlimentar(userId, id, tabela, afinidade);
            return resultado.Sucesso ? Ok(resultado) : BadRequest(resultado);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Sucesso = false, Mensagem = ex.Message });
        }
    }

    [HttpDelete("preferencia-alimentar/{preferenciaId}")]
    public async Task<IActionResult> RemoverPreferenciaAlimentar(int preferenciaId)
    {
        try
        {
            var userId = GetUserId();
            var resultado = await _userProfile.RemoverPreferenciaAlimentar(userId, preferenciaId);
            return resultado.Sucesso ? Ok(resultado) : BadRequest(resultado);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Sucesso = false, Mensagem = ex.Message });
        }
    }

    // ===================== REGISTRO BIOM�TRICO =====================

    [HttpPost("registro-biometrico")]
    public async Task<IActionResult> PostRegistroBiometrico([FromBody] RegistroBiometricoDto registroBiometricoDto)
    {
        try
        {
            var userId = GetUserId();
            var resultado = await _userProfile.PostRegistroBiometrico(userId, registroBiometricoDto);
            return Ok(resultado);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Sucesso = false, Mensagem = ex.Message });
        }
    }

    [HttpGet("historico-biometrico")]
    public async Task<IActionResult> ListarHistoricoBiometrico()
    {
        try
        {
            var userId = GetUserId();
            var historico = await _userProfile.ListarHistoricoBiometrico(userId);
            return Ok(historico);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Sucesso = false, Mensagem = ex.Message });
        }
    }

    // ===================== HIST�RICO CL�NICO =====================

    [HttpGet("historico-clinico")]
    public async Task<IActionResult> ListarHistoricoClinico()
    {
        try
        {
            var userId = GetUserId();
            var historico = await _userProfile.ListarHistoricoClinico(userId);
            return Ok(historico);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Sucesso = false, Mensagem = ex.Message });
        }
    }

    [HttpPost("historico-clinico")]
    public async Task<IActionResult> AdicionarHistoricoClinico([FromBody] HistoricoClinicoDto dto)
    {
        try
        {
            var userId = GetUserId();
            var resultado = await _userProfile.AdicionarHistoricoClinico(userId, dto);
            return resultado.Sucesso ? Ok(resultado) : BadRequest(resultado);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Sucesso = false, Mensagem = ex.Message });
        }
    }

    [HttpPut("historico-clinico/{id}")]
    public async Task<IActionResult> AtualizarHistoricoClinico(int id, [FromBody] HistoricoClinicoDto dto)
    {
        try
        {
            var userId = GetUserId();
            var resultado = await _userProfile.AtualizarHistoricoClinico(userId, id, dto);
            return resultado.Sucesso ? Ok(resultado) : BadRequest(resultado);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Sucesso = false, Mensagem = ex.Message });
        }
    }

    [HttpDelete("historico-clinico/{id}")]
    public async Task<IActionResult> RemoverHistoricoClinico(int id)
    {
        try
        {
            var userId = GetUserId();
            var resultado = await _userProfile.RemoverHistoricoClinico(userId, id);
            return resultado.Sucesso ? Ok(resultado) : BadRequest(resultado);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Sucesso = false, Mensagem = ex.Message });
        }
    }

    // ===================== ANAMNESE ALIMENTAR =====================

    [HttpPost("anamnese-alimentar")]
    public async Task<IActionResult> SalvarAnamnese([FromBody] AnamneseAlimentarDto dto)
    {
        try
        {
            var userId = GetUserId();
            var resultado = await _userProfile.SalvarAnamneseAlimentar(userId, dto);
            return resultado.Sucesso ? Ok(resultado) : BadRequest(resultado);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Sucesso = false, Mensagem = ex.Message });
        }
    }

    [HttpGet("anamnese-alimentar/ultima")]
    public async Task<IActionResult> ObterUltimaAnamnese()
    {
        try
        {
            var userId = GetUserId();
            var anamnese = await _userProfile.ObterUltimaAnamnese(userId);
            if (anamnese == null)
                return NotFound(new { Sucesso = false, Mensagem = "Nenhuma anamnese encontrada." });
            return Ok(anamnese);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Sucesso = false, Mensagem = ex.Message });
        }
    }

    [HttpGet("anamnese-alimentar/historico")]
    public async Task<IActionResult> ListarAnamneses()
    {
        try
        {
            var userId = GetUserId();
            var anamneses = await _userProfile.ListarAnamneses(userId);
            return Ok(anamneses);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Sucesso = false, Mensagem = ex.Message });
        }
    }
}
