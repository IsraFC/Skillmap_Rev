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
                Title = ".NET MAUI",
                Description = "Programacion en .NET MAUI para aplicaciones moviles",
                Link = "https://Microsoft.com",
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
                UserName = "admin@mail.com",
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(user, "Adm1234#");

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, "Admin");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    Console.WriteLine($"- {error.Description}");
                }
            }

            var user2 = new User
            {
                Name = "Mauricio",
                Father_LastName = "Campos",
                Mother_LastName = "Carranza",
                Email = "teacherMau@gmail.com",
                UserName = "teacherMau@gmail.com",
                EmailConfirmed = true
            };

            var result2 = await userManager.CreateAsync(user2, "Tch1234#");

            if (result2.Succeeded)
            {
                await userManager.AddToRoleAsync(user2, "Teacher");
            }
            else
            {
                foreach (var error in result2.Errors)
                {
                    Console.WriteLine($"- {error.Description}");
                }
            }

            var user3 = new User
            {
                Name = "Israel",
                Father_LastName = "Fernandez",
                Mother_LastName = "Carrera",
                Email = "studentIsra@gmail.com",
                UserName = "studentIsra@gmail.com",
                EmailConfirmed = true
            };

            var result3 = await userManager.CreateAsync(user3, "Std1234#");

            if (result3.Succeeded)
            {
                await userManager.AddToRoleAsync(user3, "Student");
            }
            else
            {
                foreach (var error in result3.Errors)
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

            if (!await roleManager.RoleExistsAsync("Teacher"))
            {
                await roleManager.CreateAsync(new IdentityRole("Teacher"));
            }

            if (!await roleManager.RoleExistsAsync("Student"))
            {
                await roleManager.CreateAsync(new IdentityRole("Student"));
            }
        }

        private static async Task AddSubjectsAsync(DataContext dataContext)
        {
            var teacher = await dataContext.Users.FirstOrDefaultAsync();
            if (teacher != null)
            {
                var subject = new Subject
                {
                    Name = "Aplicaciones Moviles I",
                    Semester = "6° Semestre",
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
            }
            else
            {
                Console.WriteLine("No se encontró una materia o recurso para asociar.");
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
            }
            else
            {
                Console.WriteLine("No se encontró un usuario o recurso para asociar con feedback.");
            }
        }
    }
}
