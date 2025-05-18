using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkillmapApi.Data;
using SkillmapLib1.Models;

namespace SkillmapApi.Controllers
{
    [Authorize(Roles = "Admin,Student")]
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
                .ToListAsync();
        }

        [HttpGet("{idRecurso}/{username}")]
        public async Task<ActionResult<ResourceFeedback>> Get(int idRecurso, string username)
        {
            var user = await _dataContext.Users.FirstOrDefaultAsync(u => u.UserName == username);
            if (user == null)
                return NotFound("Usuario no encontrado");

            var result = await _dataContext.ResourceFeedbacks
                .FirstOrDefaultAsync(rf => rf.ID_Resource == idRecurso && rf.ID_User == user.Id);

            if (result == null)
                return NotFound("Feedback no encontrado");

            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<ResourceFeedback>> Post([FromBody] ResourceFeedbackInput data)
        {
            var user = await _dataContext.Users.FirstOrDefaultAsync(u => u.UserName == data.userName);
            if (user == null)
                return BadRequest("Usuario no válido");

            var feedback = new ResourceFeedback
            {
                ID_Resource = data.id_Resource,
                ID_User = user.Id,
                Feedback = data.feedback
            };

            _dataContext.ResourceFeedbacks.Add(feedback);
            await _dataContext.SaveChangesAsync();

            return Ok(feedback);
        }

        [HttpPut("{idRecurso}/{username}")]
        public async Task<ActionResult<ResourceFeedback>> Put(int idRecurso, string username, [FromBody] ResourceFeedbackUpdate updatedData)
        {
            var user = await _dataContext.Users.FirstOrDefaultAsync(u => u.UserName == username);
            if (user == null)
                return NotFound("Usuario no encontrado");

            var feedback = await _dataContext.ResourceFeedbacks
                .FirstOrDefaultAsync(f => f.ID_Resource == idRecurso && f.ID_User == user.Id);

            if (feedback == null)
                return NotFound("Feedback no encontrado");

            feedback.Feedback = updatedData.feedback;

            try
            {
                await _dataContext.SaveChangesAsync();
                return Ok(feedback);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al actualizar: {ex.Message}");
            }
        }

        [HttpDelete("{idRecurso}/{username}")]
        public async Task<ActionResult<ResourceFeedback>> Delete(int idRecurso, string username)
        {
            var user = await _dataContext.Users.FirstOrDefaultAsync(u => u.UserName == username);
            if (user == null)
                return NotFound("Usuario no encontrado");

            var result = await _dataContext.ResourceFeedbacks
                .FirstOrDefaultAsync(rf => rf.ID_Resource == idRecurso && rf.ID_User == user.Id);

            if (result == null)
                return NotFound("Feedback no encontrado");

            try
            {
                _dataContext.ResourceFeedbacks.Remove(result);
                await _dataContext.SaveChangesAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al eliminar: {ex.Message}");
            }
        }
    }

    public class ResourceFeedbackInput
    {
        public int id_Resource { get; set; }
        public string userName { get; set; } = string.Empty;
        public string feedback { get; set; } = string.Empty;
    }

    public class ResourceFeedbackUpdate
    {
        public string feedback { get; set; } = string.Empty;
    }
}
