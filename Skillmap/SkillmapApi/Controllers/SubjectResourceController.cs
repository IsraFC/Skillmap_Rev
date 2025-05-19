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
    [Route("api/[controller]")]
    [ApiController]
    public class SubjectResourceController : ControllerBase
    {
        private readonly DataContext _dataContext;

        public SubjectResourceController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        [HttpGet]
        public async Task<List<SubjectResource>> Get()
        {
            return await _dataContext.SubjectResources
                .Include(sr => sr.Subject)
                .Include(sr => sr.ResourceItem)
                .ToListAsync();
        }

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

        [Authorize(Roles = "Admin,Teacher")]
        [HttpPost]
        public async Task<ActionResult<SubjectResource>> Post([FromBody] SubjectResourceInputDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(dto);

            // Validación opcional: asegurarse que existen
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
                    TeacherFullName = sr.Subject.Teacher.Name + " " + sr.Subject.Teacher.Father_LastName
                })
                .ToListAsync();

            return Ok(result);
        }
    }
}
