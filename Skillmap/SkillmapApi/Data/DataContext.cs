using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Options;
using SkillmapLib1.Models;

namespace SkillmapApi.Data
{
    public class DataContext : IdentityDbContext<User>
    {
        public DataContext(DbContextOptions<DataContext> options)
            : base(options) 
        {
            
        }

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
                .HasKey(sr => new { sr.ID_Subject, sr.ID_Resource });

            modelBuilder.Entity<ResourceFeedback>()
                .HasKey(rf => new { rf.ID_Resource, rf.ID_User });
        }
    }
}
