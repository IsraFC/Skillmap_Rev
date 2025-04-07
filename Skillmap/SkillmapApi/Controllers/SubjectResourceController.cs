using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using SkillmapApi.Data;
using SkillmapLib1.Models;

namespace SkillmapApi.Controllers
{
    [Authorize(Roles = "Admin")]
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

        [HttpPost]
        public async Task<ActionResult<SubjectResource>> Post([FromBody] SubjectResource subjectResource)
        {
            try
            {
                await _dataContext.SubjectResources.AddAsync(subjectResource);
                await _dataContext.SaveChangesAsync();
                return Ok(subjectResource);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al agregar la relación: {ex.Message}");
            }
        }

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
    }
}
