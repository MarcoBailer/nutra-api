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
        ?? throw new UnauthorizedAccessException("Usuário não autenticado.");

    private string GetUserEmail() =>
        User.FindFirstValue(ClaimTypes.Email)
        ?? User.FindFirstValue("email")
        ?? throw new UnauthorizedAccessException("Usuário não autenticado.");

    // ===================== PERFIL NUTRICIONAL =====================

    [HttpPost("perfil-nutricional")]
    public async Task<IActionResult> PostPerfilNutricional([FromBody] PerfilNutricionalDto perfilNutricional)
    {
        perfilNutricional.UserEmail = GetUserEmail();
        var retorno = await _userProfile.PostPerfilNutricional(perfilNutricional);
        return StatusCode(retorno.StatusCode, retorno);
    }

    [HttpPut("perfil-nutricional")]
    public async Task<IActionResult> AtualizarPerfilNutricional([FromBody] PerfilNutricionalDto perfilNutricional)
    {
        var retorno = await _userProfile.AtualizarPerfilNutricional(GetUserId(), perfilNutricional);
        return StatusCode(retorno.StatusCode, retorno);
    }

    [HttpGet("buscar-perfil-nutricional")]
    public async Task<IActionResult> GetPerfilNutricional()
    {
        var retorno = await _userProfile.GetPerfilNutricional(GetUserId());
        return StatusCode(retorno.StatusCode, retorno);
    }

    // ===================== PREFERENCIAS ALIMENTARES =====================

    [HttpPost("preferencia-alimentar/{id}/{tabela}/{afinidade}")]
    public async Task<IActionResult> PostPreferenciaAlimentar(int id, ETipoTabela tabela, ETipoPreferencia afinidade)
    {
        var retorno = await _userProfile.PostPreferenciaAlimentar(GetUserId(), id, tabela, afinidade);
        return StatusCode(retorno.StatusCode, retorno);
    }

    [HttpDelete("preferencia-alimentar/{preferenciaId}")]
    public async Task<IActionResult> RemoverPreferenciaAlimentar(int preferenciaId)
    {
        var retorno = await _userProfile.RemoverPreferenciaAlimentar(GetUserId(), preferenciaId);
        return StatusCode(retorno.StatusCode, retorno);
    }

    // ===================== REGISTRO BIOMÉTRICO =====================

    [HttpPost("registro-biometrico")]
    public async Task<IActionResult> PostRegistroBiometrico([FromBody] RegistroBiometricoDto registroBiometricoDto)
    {
        var retorno = await _userProfile.PostRegistroBiometrico(GetUserId(), registroBiometricoDto);
        return StatusCode(retorno.StatusCode, retorno);
    }

    [HttpGet("historico-biometrico")]
    public async Task<IActionResult> ListarHistoricoBiometrico()
    {
        var retorno = await _userProfile.ListarHistoricoBiometrico(GetUserId());
        return StatusCode(retorno.StatusCode, retorno);
    }

    // ===================== HISTÓRICO CLÍNICO =====================

    [HttpGet("historico-clinico")]
    public async Task<IActionResult> ListarHistoricoClinico()
    {
        var retorno = await _userProfile.ListarHistoricoClinico(GetUserId());
        return StatusCode(retorno.StatusCode, retorno);
    }

    [HttpPost("historico-clinico")]
    public async Task<IActionResult> AdicionarHistoricoClinico([FromBody] HistoricoClinicoDto dto)
    {
        var retorno = await _userProfile.AdicionarHistoricoClinico(GetUserId(), dto);
        return StatusCode(retorno.StatusCode, retorno);
    }

    [HttpPut("historico-clinico/{id}")]
    public async Task<IActionResult> AtualizarHistoricoClinico(int id, [FromBody] HistoricoClinicoDto dto)
    {
        var retorno = await _userProfile.AtualizarHistoricoClinico(GetUserId(), id, dto);
        return StatusCode(retorno.StatusCode, retorno);
    }

    [HttpDelete("historico-clinico/{id}")]
    public async Task<IActionResult> RemoverHistoricoClinico(int id)
    {
        var retorno = await _userProfile.RemoverHistoricoClinico(GetUserId(), id);
        return StatusCode(retorno.StatusCode, retorno);
    }

    // ===================== ANAMNESE ALIMENTAR =====================

    [HttpPost("anamnese-alimentar")]
    public async Task<IActionResult> SalvarAnamnese([FromBody] AnamneseAlimentarDto dto)
    {
        var retorno = await _userProfile.SalvarAnamneseAlimentar(GetUserId(), dto);
        return StatusCode(retorno.StatusCode, retorno);
    }

    [HttpGet("anamnese-alimentar/ultima")]
    public async Task<IActionResult> ObterUltimaAnamnese()
    {
        var retorno = await _userProfile.ObterUltimaAnamnese(GetUserId());
        return StatusCode(retorno.StatusCode, retorno);
    }

    [HttpGet("anamnese-alimentar/historico")]
    public async Task<IActionResult> ListarAnamneses()
    {
        var retorno = await _userProfile.ListarAnamneses(GetUserId());
        return StatusCode(retorno.StatusCode, retorno);
    }
}
