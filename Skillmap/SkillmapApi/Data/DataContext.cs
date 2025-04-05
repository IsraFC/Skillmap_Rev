using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Options;
using SkillmapLib1.Models;

namespace SkillmapApi.Data
{
    public class DataContext : IdentityDbContext<User, Role, int>
    {
        public DataContext(DbContextOptions<DataContext> options)
            : base(options) 
        {
            
        }

        // Usuarios y roles
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }

        // Recursos y tipos
        public DbSet<ResourcesItem> ResourcesItems { get; set; }
        public DbSet<ResourceType> ResourceTypes { get; set; }

        // Materias y relaciones
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<SubjectResource> SubjectResources { get; set; }

        // Feedback
        public DbSet<ResourceFeedback> ResourceFeedbacks { get; set; }

        // Configuración de claves compuestas
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<SubjectResource>()
                .HasKey(sr => new { sr.ID_Materia, sr.ID_Recurso });

            modelBuilder.Entity<ResourceFeedback>()
                .HasKey(rf => new { rf.ID_Recurso, rf.ID_Usuario });
        }
    }
}
