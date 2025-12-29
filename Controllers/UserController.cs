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

    [HttpPost("perfil-nutricional")]
    public async Task<IActionResult> PostPerfilNutricional([FromBody] PerfilNutricionalDto perfilNutricional)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Token inválido: ID do usuário não encontrado.");
            }
            perfilNutricional.UserId = userId;
            var perfil = await _userProfile.PostPerfilNutricional(perfilNutricional);
            return Ok(perfil);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("preferencia-alimentar/{id}/{tabela}/{afinidade}")]
    public async Task<IActionResult> PostPreferenciaAlimentar(int id, ETipoTabela tabela, ETipoPreferencia afinidade)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Token inválido: ID do usuário não encontrado.");
            }
            var resultado = await _userProfile.PostPreferenciaAlimentar(userId ,id, tabela, afinidade);
            return Ok(resultado);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("registro-biometrico")]
    public async Task<IActionResult> PostRegistroBiometrico([FromBody] RegistroBiometricoDto registroBiometricoDto)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Token inválido: ID do usuário não encontrado.");
            }
            var resultado = await _userProfile.PostRegistroBiometrico(userId, registroBiometricoDto);
            return Ok(resultado);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("buscar-perfil-nutricional")]
    public async Task<IActionResult> GetPerfilNutricional()
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Token inválido: ID do usuário não encontrado.");
            }
            var perfil = await _userProfile.GetPerfilNutricional(userId);
            return Ok(perfil);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
