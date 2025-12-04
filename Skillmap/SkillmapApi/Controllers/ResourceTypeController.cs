using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkillmapApi.Data;
using SkillmapLib1.Models;

namespace SkillmapApi.Controllers
{
    /// <summary>
    /// Controlador que gestiona las operaciones CRUD sobre los tipos de recursos educativos (PDF, video, enlace, etc.).
    /// Solo los administradores pueden modificar esta información.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ResourceTypeController : ControllerBase
    {
        private readonly DataContext _context;

        /// <summary>
        /// Constructor que recibe el contexto de datos.
        /// </summary>
        /// <param name="context">Instancia de <see cref="DataContext"/> inyectada por el sistema.</param>
        public ResourceTypeController(DataContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Obtiene todos los tipos de recurso registrados en la base de datos.
        /// </summary>
        /// <returns>Lista de <see cref="ResourceType"/>.</returns>
        [HttpGet]
        public async Task<List<ResourceType>> Get()
        {
            return await _context.ResourceTypes.ToListAsync();
        }

        /// <summary>
        /// Obtiene un tipo de recurso específico por su identificador.
        /// </summary>
        /// <param name="id">ID del tipo de recurso a consultar.</param>
        /// <returns>Objeto <see cref="ResourceType"/> si existe; 404 si no.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<ResourceType>> Get(string id)
        {
            var result = await _context.ResourceTypes.FindAsync(id);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        /// <summary>
        /// Crea un nuevo tipo de recurso.
        /// Solo permitido para usuarios con rol Admin.
        /// </summary>
        /// <param name="type">Objeto <see cref="ResourceType"/> con la información del nuevo tipo.</param>
        /// <returns>Tipo de recurso creado o mensaje de error.</returns>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<ResourceType>> Post([FromBody] ResourceType type)
        {
            if (!ModelState.IsValid)
                return BadRequest(type);

            await _context.ResourceTypes.AddAsync(type);
            await _context.SaveChangesAsync();
            return Ok(type);
        }

        /// <summary>
        /// Elimina un tipo de recurso existente.
        /// Solo permitido para usuarios con rol Admin.
        /// </summary>
        /// <param name="id">ID del tipo de recurso a eliminar.</param>
        /// <returns>Tipo de recurso eliminado o 404 si no se encuentra.</returns>
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<ResourceType>> Delete(string id)
        {
            var result = await _context.ResourceTypes.FindAsync(id);
            if (result == null)
                return NotFound();

            _context.ResourceTypes.Remove(result);
            await _context.SaveChangesAsync();
            return Ok(result);
        }
    }
}
