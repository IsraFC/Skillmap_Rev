using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SkillmapLib1.Models;

namespace SkillmapApi.Data
{
    /// <summary>
    /// Clase encargada de poblar la base de datos con datos iniciales (usuarios, roles, materias, recursos, etc.).
    /// Esta clase es útil para entornos de desarrollo y pruebas.
    /// </summary>
    public class Seeder
    {
        /// <summary>
        /// Método principal que ejecuta el seeding si las tablas correspondientes están vacías.
        /// </summary>
        public static async Task Seed(RoleManager<IdentityRole> roleManager, UserManager<User> userManager, DataContext dataContext)
        {
            if (!dataContext.Roles.Any())
                await AddRoleAsync(roleManager);

            if (!dataContext.Users.Any())
                await AddUserAsync(userManager);

            if (!dataContext.ResourcesItems.Any())
                await AddResourcesAsync(dataContext);

            if (!dataContext.Subjects.Any())
                await AddSubjectsAsync(dataContext);

            if (!dataContext.SubjectResources.Any())
                await AddSubjectResourcesAsync(dataContext);

            if (!dataContext.ResourceFeedbacks.Any())
                await AddResourceFeedbackAsync(dataContext);
        }

        /// <summary>
        /// Agrega roles predeterminados al sistema: Admin, Teacher y Student.
        /// </summary>
        private static async Task AddRoleAsync(RoleManager<IdentityRole> roleManager)
        {
            if (!await roleManager.RoleExistsAsync("Admin"))
                await roleManager.CreateAsync(new IdentityRole("Admin"));

            if (!await roleManager.RoleExistsAsync("Teacher"))
                await roleManager.CreateAsync(new IdentityRole("Teacher"));

            if (!await roleManager.RoleExistsAsync("Student"))
                await roleManager.CreateAsync(new IdentityRole("Student"));
        }

        /// <summary>
        /// Crea usuarios predeterminados con distintos roles (Admin, Teacher, Student).
        /// </summary>
        private static async Task AddUserAsync(UserManager<User> userManager)
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
                await userManager.AddToRoleAsync(user, "Admin");

            var user2 = new User
            {
                Name = "Mauricio",
                Father_LastName = "Campos",
                Mother_LastName = "Carranza",
                Email = "teacherMau@gmail.com",
                UserName = "teacherMau@gmail.com",
                EmailConfirmed = true
            };

            if ((await userManager.CreateAsync(user2, "Tch1234#")).Succeeded)
                await userManager.AddToRoleAsync(user2, "Teacher");

            var user3 = new User
            {
                Name = "Israel",
                Father_LastName = "Fernandez",
                Mother_LastName = "Carrera",
                Email = "studentIsra@gmail.com",
                UserName = "studentIsra@gmail.com",
                EmailConfirmed = true
            };

            if ((await userManager.CreateAsync(user3, "Std1234#")).Succeeded)
                await userManager.AddToRoleAsync(user3, "Student");
        }

        /// <summary>
        /// Agrega un recurso educativo de ejemplo y su tipo (PDF).
        /// </summary>
        private static async Task AddResourcesAsync(DataContext dataContext)
        {
            var resourceType = await dataContext.ResourceTypes.FirstOrDefaultAsync();
            if (resourceType == null)
            {
                resourceType = new ResourceType { Id_Resource_Type = "PDF" };
                await dataContext.ResourceTypes.AddAsync(resourceType);
                await dataContext.SaveChangesAsync();
            }

            var item = new ResourcesItem
            {
                Title = ".NET MAUI",
                Description = "Programación en .NET MAUI para aplicaciones móviles",
                Link = "https://Microsoft.com",
                UploadDate = DateTime.Now,
                ResourceTypeId = resourceType.Id_Resource_Type
            };

            await dataContext.ResourcesItems.AddAsync(item);
            await dataContext.SaveChangesAsync();
        }

        /// <summary>
        /// Crea una materia ejemplo asignada al primer usuario registrado como docente.
        /// </summary>
        private static async Task AddSubjectsAsync(DataContext dataContext)
        {
            var teacher = await dataContext.Users.FirstOrDefaultAsync();
            if (teacher != null)
            {
                var subject = new Subject
                {
                    Name = "Aplicaciones Móviles I",
                    Semester = "6° Semestre",
                    ID_Teacher = teacher.Id
                };

                await dataContext.Subjects.AddAsync(subject);
                await dataContext.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Crea la relación entre una materia y un recurso si ambos existen.
        /// </summary>
        private static async Task AddSubjectResourcesAsync(DataContext dataContext)
        {
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
        }

        /// <summary>
        /// Agrega un comentario (feedback) de ejemplo entre un usuario y un recurso.
        /// </summary>
        private static async Task AddResourceFeedbackAsync(DataContext dataContext)
        {
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
        }
    }
}
