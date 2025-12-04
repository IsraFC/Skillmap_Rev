using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkillmapApi.Data;
using SkillmapLib1.Models;
using SkillmapLib1.Models.DTO.InputDTO;

namespace SkillmapApi.Controllers
{
    /// <summary>
    /// Controlador encargado de gestionar la retroalimentación (feedback) sobre los recursos educativos.
    /// Permite a los usuarios enviar, actualizar, obtener y eliminar comentarios.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ResourceFeedbackController : ControllerBase
    {
        private readonly DataContext _dataContext;

        public ResourceFeedbackController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        /// <summary>
        /// Obtiene todos los registros de retroalimentación.
        /// </summary>
        [HttpGet]
        public async Task<List<ResourceFeedback>> Get()
        {
            return await _dataContext.ResourceFeedbacks.ToListAsync();
        }

        /// <summary>
        /// Obtiene una retroalimentación específica por ID de recurso y nombre de usuario.
        /// </summary>
        /// <param name="idRecurso">ID del recurso.</param>
        /// <param name="username">Nombre de usuario asociado al comentario.</param>
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

        /// <summary>
        /// Registra una nueva retroalimentación para un recurso.
        /// Solo disponible para Admins y Estudiantes.
        /// </summary>
        /// <param name="data">DTO con la información del feedback.</param>
        [Authorize(Roles = "Admin,Student")]
        [HttpPost]
        public async Task<ActionResult<ResourceFeedback>> Post([FromBody] ResourceFeedbackInputDTO data)
        {
            var user = await _dataContext.Users.FirstOrDefaultAsync(u => u.UserName == data.UserName);
            if (user == null)
                return BadRequest("Usuario no válido");

            var feedback = new ResourceFeedback
            {
                ID_Resource = data.ID_Resource,
                ID_User = user.Id,
                Feedback = data.Feedback
            };

            _dataContext.ResourceFeedbacks.Add(feedback);
            await _dataContext.SaveChangesAsync();

            return Ok(feedback);
        }

        /// <summary>
        /// Actualiza una retroalimentación existente.
        /// Solo disponible para Admins y Estudiantes.
        /// </summary>
        /// <param name="idRecurso">ID del recurso.</param>
        /// <param name="username">Usuario asociado a la retroalimentación.</param>
        /// <param name="updatedData">Nuevo contenido del feedback.</param>
        [Authorize(Roles = "Admin,Student")]
        [HttpPut("{idRecurso}/{username}")]
        public async Task<ActionResult<ResourceFeedback>> Put(int idRecurso, string username, [FromBody] ResourceFeedbackUpdateInputDTO updatedData)
        {
            var user = await _dataContext.Users.FirstOrDefaultAsync(u => u.UserName == username);
            if (user == null)
                return NotFound("Usuario no encontrado");

            var feedback = await _dataContext.ResourceFeedbacks
                .FirstOrDefaultAsync(f => f.ID_Resource == idRecurso && f.ID_User == user.Id);

            if (feedback == null)
                return NotFound("Feedback no encontrado");

            feedback.Feedback = updatedData.Feedback;

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

        /// <summary>
        /// Elimina una retroalimentación existente por ID de recurso y usuario.
        /// Solo permitido para Admins y Docentes.
        /// </summary>
        /// <param name="idRecurso">ID del recurso.</param>
        /// <param name="username">Usuario del comentario a eliminar.</param>
        [Authorize(Roles = "Admin,Teacher")]
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
}
