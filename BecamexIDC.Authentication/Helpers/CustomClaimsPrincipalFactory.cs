using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Threading.Tasks;

namespace TeduCoreApp.Helpers
{
    public class CustomClaimsPrincipalFactory : UserClaimsPrincipalFactory<IdentityUser, IdentityRole>
    {
        UserManager<IdentityUser> _userManger;

        public CustomClaimsPrincipalFactory(UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager, IOptions<IdentityOptions> options)
            : base(userManager, roleManager, options)
        {
            _userManger = userManager;
        }

        public async override Task<ClaimsPrincipal> CreateAsync(IdentityUser user)
        {
            var principal = await base.CreateAsync(user);
            var roles = await _userManger.GetRolesAsync(user);
            ((ClaimsIdentity)principal.Identity).AddClaims(new[]
            {
                new Claim(ClaimTypes.NameIdentifier,user.UserName), 
                new Claim("Username",user.UserName)
            });
            return principal;
        }
    }
}
