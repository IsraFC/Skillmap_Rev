using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkillmapApi.Data;
using SkillmapLib1.Models;

namespace SkillmapApi.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class ResourceFeedbackController : ControllerBase
    {
        private readonly DataContext _dataContext;

        public ResourceFeedbackController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        [HttpGet]
        public async Task<List<ResourceFeedback>> Get()
        {
            return await _dataContext.ResourceFeedbacks
                .Include(rf => rf.ResourceItem)
                .Include(rf => rf.User)
                .ToListAsync();
        }

        [HttpGet("{idRecurso}/{idUsuario}")]
        public async Task<ActionResult<ResourceFeedback>> Get(int idRecurso, int idUsuario)
        {
            var result = await _dataContext.ResourceFeedbacks
                .Include(rf => rf.ResourceItem)
                .Include(rf => rf.User)
                .FirstOrDefaultAsync(rf => rf.ID_Recurso == idRecurso && rf.ID_Usuario == idUsuario);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<ResourceFeedback>> Post([FromBody] ResourceFeedback feedback)
        {
            if (!ModelState.IsValid)
                return BadRequest(feedback);

            try
            {
                await _dataContext.ResourceFeedbacks.AddAsync(feedback);
                await _dataContext.SaveChangesAsync();
                return Ok(feedback);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPut("{idRecurso}/{idUsuario}")]
        public async Task<ActionResult<ResourceFeedback>> Put(int idRecurso, int idUsuario, [FromBody] ResourceFeedback feedback)
        {
            if (idRecurso != feedback.ID_Recurso || idUsuario != feedback.ID_Usuario)
                return BadRequest("IDs don't match.");

            _dataContext.Entry(feedback).State = EntityState.Modified;

            try
            {
                await _dataContext.SaveChangesAsync();
                return Ok(feedback);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("{idRecurso}/{idUsuario}")]
        public async Task<ActionResult<ResourceFeedback>> Delete(int idRecurso, int idUsuario)
        {
            var result = await _dataContext.ResourceFeedbacks
                .FirstOrDefaultAsync(rf => rf.ID_Recurso == idRecurso && rf.ID_Usuario == idUsuario);

            if (result == null)
                return NotFound();

            try
            {
                _dataContext.ResourceFeedbacks.Remove(result);
                await _dataContext.SaveChangesAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }

}
