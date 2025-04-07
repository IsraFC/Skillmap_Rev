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
            if (!dataContext.Subjects.Any())
            {
                await AddSubjectsAsync(dataContext);
            }
            if (!dataContext.SubjectResources.Any())
            {
                await AddSubjectResourcesAsync(dataContext);
            }
            if (!dataContext.ResourceFeedbacks.Any())
            {
                await AddResourceFeedbackAsync(dataContext);
            }
        }

        private static async Task AddResourcesAsync(DataContext dataContext)
        {
            // Verificamos si hay tipos de recurso, si no, se agrega uno
            var resourceType = await dataContext.ResourceTypes.FirstOrDefaultAsync();
            if (resourceType == null)
            {
                resourceType = new ResourceType() { Id_Resource_Type = "PDF"};
                await dataContext.ResourceTypes.AddAsync(resourceType);
                await dataContext.SaveChangesAsync();
            }

            var item = new ResourcesItem
            {
                Title = "Item1",
                Description = "Descripción item 1",
                Link = "https://ejemplo.com",
                UploadDate = DateTime.Now,
                ResourceTypeId = resourceType.Id_Resource_Type
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

        private static async Task AddSubjectsAsync(DataContext dataContext)
        {
            var teacher = await dataContext.Users.FirstOrDefaultAsync();
            if (teacher != null)
            {
                var subject = new Subject
                {
                    Name = "Programación I",
                    Semester = "1° Semestre",
                    ID_Teacher = teacher.Id,
                };

                await dataContext.Subjects.AddAsync(subject);
                await dataContext.SaveChangesAsync();
            }
        }

        private static async Task AddSubjectResourcesAsync(DataContext dataContext)
        {
            if (await dataContext.SubjectResources.AnyAsync())
                return;

            var subject = await dataContext.Subjects.FirstOrDefaultAsync();
            var resource = await dataContext.ResourcesItems.FirstOrDefaultAsync();

            if (subject != null && resource != null)
            {
                var subjectResource = new SubjectResource
                {
                    ID_Subject = subject.ID_Subject,
                    ID_Resource = resource.Id
                };

                await dataContext.SubjectResources.AddAsync(subjectResource);
                await dataContext.SaveChangesAsync();
                Console.WriteLine("✅ SubjectResource agregado.");
            }
            else
            {
                Console.WriteLine("❌ No se encontró una materia o recurso para asociar.");
            }
        }

        private static async Task AddResourceFeedbackAsync(DataContext dataContext)
        {
            if (await dataContext.ResourceFeedbacks.AnyAsync())
                return;

            var user = await dataContext.Users.FirstOrDefaultAsync();
            var resource = await dataContext.ResourcesItems.FirstOrDefaultAsync();

            if (user != null && resource != null)
            {
                var feedback = new ResourceFeedback
                {
                    ID_User = user.Id, 
                    ID_Resource = resource.Id,
                    Feedback = "Este recurso fue muy útil para el examen final."
                };

                await dataContext.ResourceFeedbacks.AddAsync(feedback);
                await dataContext.SaveChangesAsync();
                Console.WriteLine("✅ ResourceFeedback agregado.");
            }
            else
            {
                Console.WriteLine("❌ No se encontró un usuario o recurso para asociar con feedback.");
            }
        }
    }
}
