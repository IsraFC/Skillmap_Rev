using System.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkillmapApi.Data;
using SkillmapLib1.Models;

namespace SkillmapApi.Controllers
{
    /// <summary>
    /// Controlador API para gestionar recursos educativos.
    /// Permite operaciones CRUD con restricciones de rol para docentes y administradores.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ResourceController : ControllerBase
    {
        private readonly DataContext _dataContext;

        /// <summary>
        /// Constructor del controlador que inyecta el contexto de datos.
        /// </summary>
        /// <param name="dataContext">Instancia del contexto de base de datos.</param>
        public ResourceController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        /// <summary>
        /// Obtiene la lista completa de recursos educativos.
        /// </summary>
        /// <returns>Lista de objetos <see cref="ResourcesItem"/>.</returns>
        [HttpGet]
        public async Task<List<ResourcesItem>> Get()
        {
            return await _dataContext.ResourcesItems.ToListAsync();
        }

        /// <summary>
        /// Obtiene un recurso por su ID.
        /// </summary>
        /// <param name="id">ID del recurso.</param>
        /// <returns>Recurso correspondiente o BadRequest si no existe.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<ResourcesItem>> Get(int id)
        {
            var result = await _dataContext.ResourcesItems.FindAsync(id);
            if (result == null)
            {
                return BadRequest();
            }
            return Ok(result);
        }

        /// <summary>
        /// Crea un nuevo recurso educativo.
        /// Solo accesible para usuarios con rol Admin o Teacher.
        /// </summary>
        /// <param name="resources">Objeto <see cref="ResourcesItem"/> a crear.</param>
        /// <returns>Recurso creado o error en la solicitud.</returns>
        [Authorize(Roles = "Admin,Teacher")]
        [HttpPost]
        public async Task<ActionResult<ResourcesItem>> Post([FromBody] ResourcesItem resources)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(resources);
            }

            try
            {
                await _dataContext.ResourcesItems.AddAsync(resources);
                await _dataContext.SaveChangesAsync();
                return Ok(resources);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        /// <summary>
        /// Actualiza un recurso existente.
        /// Solo accesible para usuarios con rol Admin o Teacher.
        /// </summary>
        /// <param name="id">ID del recurso a actualizar.</param>
        /// <param name="resources">Objeto <see cref="ResourcesItem"/> con los datos actualizados.</param>
        /// <returns>Recurso actualizado o mensaje de error.</returns>
        [Authorize(Roles = "Admin,Teacher")]
        [HttpPut("{id}")]
        public async Task<ActionResult<ResourcesItem>> Put(int id, [FromBody] ResourcesItem resources)
        {
            try
            {
                _dataContext.Entry(resources).State = EntityState.Modified;
                await _dataContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }

            return Ok(resources);
        }

        /// <summary>
        /// Elimina un recurso por su ID.
        /// Solo accesible para usuarios con rol Admin o Teacher.
        /// </summary>
        /// <param name="id">ID del recurso a eliminar.</param>
        /// <returns>Recurso eliminado o mensaje de error.</returns>
        [Authorize(Roles = "Admin,Teacher")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<ResourcesItem>> Delete(int id)
        {
            var result = await _dataContext.ResourcesItems.FindAsync(id);
            if (result == null)
            {
                return BadRequest();
            }

            try
            {
                _dataContext.ResourcesItems.Remove(result);
                await _dataContext.SaveChangesAsync();
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }
    }
}
