using System;
using System.Linq;
using IdentityModel;
using IdentityServer4.AspNetIdentity;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;
using System.Threading.Tasks;
using Qubiz.IdentityServer.Models;
using Microsoft.Extensions.Options;

namespace Qubiz.IdentityServer
{

    public class AppClaimsPrincipalFactory : UserClaimsPrincipalFactory<ApplicationUser, IdentityRole>
    {
        private UserManager<ApplicationUser> _userManager;


        public AppClaimsPrincipalFactory(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IOptions<IdentityOptions> optionsAccessor) : base(userManager, roleManager, optionsAccessor)
        {
            optionsAccessor.Value.ClaimsIdentity.UserIdClaimType = ClaimTypes.NameIdentifier;
            _userManager = userManager;
        }

    }
}
