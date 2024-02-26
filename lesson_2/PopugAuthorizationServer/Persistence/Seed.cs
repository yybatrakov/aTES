namespace AuthorizationServer.Persistence
{
    using AuthorizationServer.Domain;
    using AuthorizationServer.Models;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class Seed
    {
        /// <summary>
        /// This method is used for Demo/Development puprose only in order to have some data
        /// </summary>
        /// <param name="userManager">UserManager that configure IdentityUser</param>
        /// <param name="roleManager">RoleManager that configre IdentityRole</param>
        /// <returns>Completed Task</returns>
        public static async Task AddInitialUsersAsync(DataContext dataContext, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            if (!await dataContext.Users.AnyAsync())
            {

                // add some roles
                var roles = new List<IdentityRole>
                {
                    new IdentityRole
                    {
                        Name="Admin"
                    },
                    new IdentityRole
                    {
                        Name = "Manager"
                    },
                    new IdentityRole
                    {
                        Name ="Accounter"
                    }
                };
                // create roles first
                foreach (var role in roles)
                {
                    await roleManager.CreateAsync(role);
                }


                //add admin user
                var user = new IdentityUser() { UserName = "Admin" };
                await userManager.CreateAsync(user, AuthUserHelper.GetUserBeak(user.UserName));
                await userManager.AddToRoleAsync(user, "Admin");
            }

            if (!await dataContext.OAuthClients.AnyAsync())
            {
                var clients = new List<OAuthClients>
                {
                    new OAuthClients
                    {
                        ClientSecret = Guid.Parse("BF2C6EC3-338A-4EE3-9D97-F98A2A559186"),
                        ClientId = Guid.Parse("BF2C6EC3-338A-4EE3-9D97-F98A2A559186"),
                        AppName ="TaskManager",
                        FallbackUri = "/oauth/callback",
                        Website = "https://localhost:44336",
                        OAuthScopes = new List<OAuthScope>
                        {
                        },
                    }
                };

                await dataContext.OAuthClients.AddRangeAsync(clients);
                await dataContext.SaveChangesAsync();
            }
        }
    }
}
