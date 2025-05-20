using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SkillmapLib1.Models;

namespace SkillmapApi.Data
{
    /// <summary>
    /// Clase que representa el contexto de la base de datos para el proyecto SkillMap.
    /// Hereda de <see cref="IdentityDbContext"/> para incluir gestión de usuarios.
    /// Define las tablas, relaciones y claves primarias/compuestas del modelo de datos.
    /// </summary>
    public class DataContext : IdentityDbContext<User>
    {
        /// <summary>
        /// Constructor que recibe las opciones de configuración para el contexto.
        /// </summary>
        /// <param name="options">Opciones de configuración del contexto.</param>
        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// Tabla de recursos educativos (PDFs, videos, enlaces, etc.).
        /// </summary>
        public DbSet<ResourcesItem> ResourcesItems { get; set; }

        /// <summary>
        /// Tabla de tipos de recursos (PDF, video, enlace...).
        /// </summary>
        public DbSet<ResourceType> ResourceTypes { get; set; }

        /// <summary>
        /// Tabla de materias o asignaturas.
        /// </summary>
        public DbSet<Subject> Subjects { get; set; }

        /// <summary>
        /// Tabla intermedia que relaciona materias con recursos.
        /// </summary>
        public DbSet<SubjectResource> SubjectResources { get; set; }

        /// <summary>
        /// Tabla que almacena comentarios y retroalimentación sobre recursos.
        /// </summary>
        public DbSet<ResourceFeedback> ResourceFeedbacks { get; set; }

        /// <summary>
        /// Configura las claves primarias compuestas para las relaciones muchos a muchos.
        /// </summary>
        /// <param name="modelBuilder">Constructor de modelos de EF Core.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Clave compuesta para la relación Materia-Recurso
            modelBuilder.Entity<SubjectResource>()
                .HasKey(sr => new { sr.ID_Subject, sr.ID_Resource });

            // Clave compuesta para la retroalimentación (feedback)
            modelBuilder.Entity<ResourceFeedback>()
                .HasKey(rf => new { rf.ID_Resource, rf.ID_User });
        }
    }
}
