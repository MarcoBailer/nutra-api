using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Nutra.Interfaces;
using Nutra.Models;
using Nutra.Models.Dtos.Registro;
using Nutra.Models.Usuario;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

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
    }
}
