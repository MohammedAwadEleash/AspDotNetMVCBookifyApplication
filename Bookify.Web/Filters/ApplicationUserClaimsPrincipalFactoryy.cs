using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Bookify.Web.Filters
{
    public class ApplicationUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<ApplicationUser, IdentityRole>

    {
        public ApplicationUserClaimsPrincipalFactory(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IOptions<IdentityOptions> options)
            : base(userManager, roleManager, options)
        {

        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
        {
            var identity = await base.GenerateClaimsAsync(user); //It Gets the  claim taht is  added with user when  he logged in

            identity.AddClaim(new Claim(ClaimTypes.GivenName, user.FullName));

            return identity;
        }

    }
}
