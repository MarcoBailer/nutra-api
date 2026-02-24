using Microsoft.AspNetCore.Identity;
using Nutra.Interfaces;
using Nutra.Models;
using Nutra.Models.Dtos;
using Nutra.Models.Dtos.Registro;
using Nutra.Models.Usuario;

namespace Nutra.Services
{
    public class AccountsService : IAccounts
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;

        public AccountsService(UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        public async Task<RetornoPadrao> AtualizarPerfilAsync(string userId, UpdateProfileDto dto)
        {
            var retorno = new RetornoPadrao();

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                retorno.Sucesso = false;
                retorno.Mensagem = "Usuário não encontrado.";
                return retorno;
            }

            if (dto.NomeCompleto != null) user.NomeCompleto = dto.NomeCompleto;
            if (dto.Cpf != null) user.CPF = dto.Cpf;
            if (dto.DataNascimento.HasValue) user.DataNascimento = dto.DataNascimento;
            if (dto.Telefone != null) user.Telefone = dto.Telefone;
            if (dto.FotoPerfilUrl != null) user.FotoPerfilUrl = dto.FotoPerfilUrl;
            if (dto.Logradouro != null) user.Logradouro = dto.Logradouro;
            if (dto.Numero != null) user.Numero = dto.Numero;
            if (dto.Complemento != null) user.Complemento = dto.Complemento;
            if (dto.Bairro != null) user.Bairro = dto.Bairro;
            if (dto.Cidade != null) user.Cidade = dto.Cidade;
            if (dto.Estado != null) user.Estado = dto.Estado;
            if (dto.CEP != null) user.CEP = dto.CEP;

            user.AtualizadoEm = DateTime.UtcNow;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                retorno.Sucesso = false;
                retorno.Mensagem = "Erro ao atualizar: " + string.Join("; ", result.Errors.Select(e => e.Description));
                return retorno;
            }

            retorno.Sucesso = true;
            retorno.Mensagem = "Perfil atualizado com sucesso.";
            return retorno;
        }

        public async Task<RetornoPadrao> DesativarContaAsync(string userId)
        {
            var retorno = new RetornoPadrao();

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                retorno.Sucesso = false;
                retorno.Mensagem = "Usuário não encontrado.";
                return retorno;
            }

            user.Ativo = false;
            user.AtualizadoEm = DateTime.UtcNow;

            var result = await _userManager.UpdateAsync(user);

            retorno.Sucesso = result.Succeeded;
            retorno.Mensagem = result.Succeeded ? "Conta desativada com sucesso." : "Erro ao desativar conta.";
            return retorno;
        }

        public async Task<RetornoPadrao> ReativarContaAsync(string userId)
        {
            var retorno = new RetornoPadrao();

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                retorno.Sucesso = false;
                retorno.Mensagem = "Usuário não encontrado.";
                return retorno;
            }

            user.Ativo = true;
            user.AtualizadoEm = DateTime.UtcNow;

            var result = await _userManager.UpdateAsync(user);

            retorno.Sucesso = result.Succeeded;
            retorno.Mensagem = result.Succeeded ? "Conta reativada com sucesso." : "Erro ao reativar conta.";
            return retorno;
        }
    }
}
