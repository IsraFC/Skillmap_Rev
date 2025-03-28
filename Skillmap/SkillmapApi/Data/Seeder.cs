using System.Net.NetworkInformation;
using Microsoft.AspNetCore.Identity;
using SkillmapLib1.Models;

namespace SkillmapApi.Data
{
    public class Seeder
    {
        public static async Task Seed(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager, DataContext dataContext)
        {
            if (!dataContext.Roles.Any())
            {
                await AddRoleAsync(roleManager, dataContext);
            }
            if (!dataContext.Users.Any())
            {
                await AddUserAsync(userManager, dataContext);
            }
            //if (dataContext.ResourcesItems.Any()) 
            //{
            //    await AddResourcesAsync(dataContext);
            //}
        }

        //private static async Task AddResourcesAsync(DataContext dataContext)
        //{
        //    ResourcesItem item = new ResourcesItem
        //    {
        //        Title = "Item1",
        //        Description = "Descrpcion item 1",
        //    };
        //}

        private static async Task AddUserAsync(UserManager<IdentityUser> userManager, DataContext dataContext)
        {
            User user = new User
            {
                Name = "Admin",
                Ap = "1",
                Am = "2",
                Email = "admin@mail.com",
                UserName = "admin@mail.com",
            };

            var result = await userManager.CreateAsync(user, "123456");

            if (result.Succeeded) 
            {
                await userManager.AddToRoleAsync(user, "Admin");
            }
        }

        private static async Task AddRoleAsync(RoleManager<IdentityRole> roleManager, DataContext dataContext)
        {
            if(!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }
        }
    }
}
