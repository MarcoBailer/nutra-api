using Nutra.Interfaces;
using Nutra.Models;
using Nutra.Models.Dtos;

namespace Nutra.Services
{
    public class AccountsService : IAccounts
    {
        private readonly IApplicationUserService _applicationUserService;

        public AccountsService(IApplicationUserService applicationUserService)
        {
            _applicationUserService = applicationUserService;
        }

        public async Task<RetornoPadrao> AtualizarPerfilAsync(string userId, UpdateProfileDto dto)
        {
            var user = await _applicationUserService.FindByIdAsync(userId);
            if (user == null)
                return RetornoPadrao.NaoEncontrado("Usuário não encontrado.");

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

            var updated = await _applicationUserService.UpdateAsync(user);

            if (!updated)
                return RetornoPadrao.NaoEncontrado("Erro ao atualizar: usuário não encontrado.");

            return RetornoPadrao.Ok("Perfil atualizado com sucesso.");
        }

        public async Task<RetornoPadrao> DesativarContaAsync(string userId)
        {
            var user = await _applicationUserService.FindByIdAsync(userId);
            if (user == null)
                return RetornoPadrao.NaoEncontrado("Usuário não encontrado.");

            user.Ativo = false;
            user.AtualizadoEm = DateTime.UtcNow;

            var updated = await _applicationUserService.UpdateAsync(user);

            return updated
                ? RetornoPadrao.Ok("Conta desativada com sucesso.")
                : RetornoPadrao.NaoEncontrado("Erro ao desativar conta: usuário não encontrado.");
        }

        public async Task<RetornoPadrao> ReativarContaAsync(string userId)
        {
            var user = await _applicationUserService.FindByIdAsync(userId);
            if (user == null)
                return RetornoPadrao.NaoEncontrado("Usuário não encontrado.");

            user.Ativo = true;
            user.AtualizadoEm = DateTime.UtcNow;

            var updated = await _applicationUserService.UpdateAsync(user);

            return updated
                ? RetornoPadrao.Ok("Conta reativada com sucesso.")
                : RetornoPadrao.NaoEncontrado("Erro ao reativar conta: usuário não encontrado.");
        }
    }
}
