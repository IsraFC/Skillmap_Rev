using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using SkillmapApi.Data;
using SkillmapLib1.Models;
using SkillmapLib1.Models.DTO.OutputDTO;
using SkillmapLib1.Models.DTO.InputDTO;

namespace SkillmapApi.Controllers
{
    /// <summary>
    /// Controlador encargado de manejar la relación entre materias y recursos.
    /// Permite crear, consultar y eliminar asignaciones de recursos a materias.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class SubjectResourceController : ControllerBase
    {
        private readonly DataContext _dataContext;

        /// <summary>
        /// Constructor que recibe el contexto de base de datos.
        /// </summary>
        /// <param name="dataContext">Instancia del contexto inyectado.</param>
        public SubjectResourceController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        /// <summary>
        /// Obtiene todas las relaciones entre materias y recursos.
        /// </summary>
        /// <returns>Lista completa de <see cref="SubjectResource"/>.</returns>
        [HttpGet]
        public async Task<List<SubjectResource>> Get()
        {
            return await _dataContext.SubjectResources
                .Include(sr => sr.Subject)
                .Include(sr => sr.ResourceItem)
                .ToListAsync();
        }

        /// <summary>
        /// Obtiene una relación específica entre materia y recurso por sus IDs.
        /// </summary>
        /// <param name="idMateria">ID de la materia.</param>
        /// <param name="idRecurso">ID del recurso.</param>
        /// <returns>Objeto <see cref="SubjectResource"/> si existe.</returns>
        [HttpGet("{idMateria}/{idRecurso}")]
        public async Task<ActionResult<SubjectResource>> Get(int idMateria, int idRecurso)
        {
            var result = await _dataContext.SubjectResources
                .Include(sr => sr.Subject)
                .Include(sr => sr.ResourceItem)
                .FirstOrDefaultAsync(sr => sr.ID_Subject == idMateria && sr.ID_Resource == idRecurso);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        /// <summary>
        /// Crea una nueva relación entre materia y recurso.
        /// Solo permitido para Admins o Docentes.
        /// </summary>
        /// <param name="dto">Datos de entrada con IDs de materia y recurso.</param>
        /// <returns>Relación creada o error si alguno no existe.</returns>
        [Authorize(Roles = "Admin,Teacher")]
        [HttpPost]
        public async Task<ActionResult<SubjectResource>> Post([FromBody] SubjectResourceInputDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(dto);

            var subjectExists = await _dataContext.Subjects.AnyAsync(s => s.ID_Subject == dto.ID_Subject);
            var resourceExists = await _dataContext.ResourcesItems.AnyAsync(r => r.Id == dto.ID_Resource);

            if (!subjectExists || !resourceExists)
                return NotFound("La materia o el recurso no existen.");

            var relation = new SubjectResource
            {
                ID_Subject = dto.ID_Subject,
                ID_Resource = dto.ID_Resource
            };

            await _dataContext.SubjectResources.AddAsync(relation);
            await _dataContext.SaveChangesAsync();

            return Ok(relation);
        }

        /// <summary>
        /// Elimina una relación entre materia y recurso por sus IDs.
        /// Solo permitido para Admins o Docentes.
        /// </summary>
        /// <param name="idMateria">ID de la materia.</param>
        /// <param name="idRecurso">ID del recurso.</param>
        /// <returns>Relación eliminada o mensaje de error.</returns>
        [Authorize(Roles = "Admin,Teacher")]
        [HttpDelete("{idMateria}/{idRecurso}")]
        public async Task<ActionResult<SubjectResource>> Delete(int idMateria, int idRecurso)
        {
            var result = await _dataContext.SubjectResources
                .FirstOrDefaultAsync(sr => sr.ID_Subject == idMateria && sr.ID_Resource == idRecurso);

            if (result == null)
                return NotFound();

            try
            {
                _dataContext.SubjectResources.Remove(result);
                await _dataContext.SaveChangesAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al eliminar la relación: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtiene todos los recursos relacionados con una materia específica.
        /// </summary>
        /// <param name="id">ID de la materia.</param>
        /// <returns>Lista de <see cref="ResourcePerSubjectOutputDTO"/> con datos combinados del recurso y su docente.</returns>
        [HttpGet("subject/{id}")]
        public async Task<ActionResult<List<ResourcePerSubjectOutputDTO>>> GetResourcesBySubject(int id)
        {
            var result = await _dataContext.SubjectResources
                .Where(sr => sr.ID_Subject == id)
                .Include(sr => sr.ResourceItem)
                .Include(sr => sr.Subject)
                    .ThenInclude(s => s.Teacher)
                .Select(sr => new ResourcePerSubjectOutputDTO
                {
                    ID_Subject = sr.Subject.ID_Subject,
                    SubjectName = sr.Subject.Name,
                    ID_Resource = sr.ResourceItem.Id,
                    ResourceTitle = sr.ResourceItem.Title,
                    Description = sr.ResourceItem.Description,
                    Link = sr.ResourceItem.Link,
                    UploadDate = sr.ResourceItem.UploadDate,

                    CoverImage = sr.ResourceItem.CoverImage,

                    TeacherFullName = sr.Subject.Teacher.Name + " " + sr.Subject.Teacher.Father_LastName
                })
                .ToListAsync();

            return Ok(result);
        }
    }
}
