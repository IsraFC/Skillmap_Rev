using System.Net.NetworkInformation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SkillmapLib1.Models;

namespace SkillmapApi.Data
{
    public class Seeder
    {
        public static async Task Seed(RoleManager<IdentityRole> roleManager, UserManager<User> userManager, DataContext dataContext)
        {
            if (!dataContext.Roles.Any())
            {
                await AddRoleAsync(roleManager);
            }
            if (!dataContext.Users.Any())
            {
                await AddUserAsync(userManager);
            }
            if (!dataContext.ResourcesItems.Any())
            {
                await AddResourcesAsync(dataContext);
            }
        }

        private static async Task AddResourcesAsync(DataContext dataContext)
        {
            // Verificamos si hay tipos de recurso, si no, se agrega uno
            var resourceType = await dataContext.ResourceTypes.FirstOrDefaultAsync();
            if (resourceType == null)
            {
                resourceType = new ResourceType() { Id_Tipo_Recurso = "PDF"};
                await dataContext.ResourceTypes.AddAsync(resourceType);
                await dataContext.SaveChangesAsync();
            }

            var item = new ResourcesItem
            {
                Title = "Item1",
                Description = "Descripción item 1",
                Link = "https://ejemplo.com",
                UploadDate = DateTime.Now,
                ResourceTypeId = resourceType.Id_Tipo_Recurso
            };

            await dataContext.ResourcesItems.AddAsync(item);
            await dataContext.SaveChangesAsync();
        }

        private static async Task AddUserAsync(UserManager<User> userManager )
        {
            var user = new User
            {
                Name = "Admin",
                Father_LastName = "1",
                Mother_LastName = "2",
                Email = "admin@mail.com",
                UserName = "admin@mail.com"
            };

            var result = await userManager.CreateAsync(user, "Az1234#");

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, "Admin");
                Console.WriteLine("✅ Usuario creado exitosamente.");
            }
            else
            {
                Console.WriteLine("❌ Error al crear el usuario:");
                foreach (var error in result.Errors)
                {
                    Console.WriteLine($"- {error.Description}");
                }
            }
        }

        private static async Task AddRoleAsync(RoleManager<IdentityRole> roleManager)
        {
            if(!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }
        }
    }
}
