using Microsoft.AspNetCore.Identity;

namespace Bookify.Web.Seeds
{
    public static class DefaultRoles
    {

        public static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManger)
        {

            if (!roleManger.Roles.Any())
            {

                await roleManger.CreateAsync(new IdentityRole(AppRoles.Admin));
                await roleManger.CreateAsync(new IdentityRole(AppRoles.Archive));
                await roleManger.CreateAsync(new IdentityRole(AppRoles.Reception));

            }
        }



    }
}
