using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkillmapApi.Data;
using SkillmapLib1.Models;
using SkillmapLib1.Models.DTO.OutputDTO;
using SkillmapLib1.Models.DTO.InputDTO;

namespace SkillmapApi.Controllers
{
    /// <summary>
    /// Controlador que gestiona las materias (subjects) del sistema.
    /// Proporciona endpoints para crear, consultar, actualizar y eliminar materias, y relacionarlas con docentes.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class SubjectController : ControllerBase
    {
        private readonly DataContext _dataContext;

        /// <summary>
        /// Constructor que recibe el contexto de datos para el acceso a la base de datos.
        /// </summary>
        /// <param name="dataContext">Instancia del contexto de datos.</param>
        public SubjectController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        /// <summary>
        /// Obtiene todas las materias con su información de docente, si existe.
        /// Requiere que el usuario esté autenticado y tenga un rol válido.
        /// </summary>
        /// <returns>Lista de <see cref="SubjectOutputDTO"/> con datos de materia y docente.</returns>
        [HttpGet]
        public async Task<ActionResult<List<Subject>>> Get()
        {
            var user = await _dataContext.Users.FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);
            if (user == null) return Unauthorized();

            var userRoles = await _dataContext.UserRoles
                .Where(ur => ur.UserId == user.Id)
                .Join(_dataContext.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => r.Name)
                .ToListAsync();

            if (!userRoles.Contains("Admin") && !userRoles.Contains("Teacher") && !userRoles.Contains("Student"))
                return Forbid();

            var subjects = await _dataContext.Subjects
                .Include(s => s.Teacher)
                .ToListAsync();

            var result = subjects.Select(s => new SubjectOutputDTO
            {
                Id_Subject = s.ID_Subject,
                Name = s.Name,
                Semester = s.Semester,
                TeacherUserName = s.Teacher?.UserName ?? "",
                TeacherFullName = s.Teacher != null
                    ? $"{s.Teacher.Name} {s.Teacher.Father_LastName} {s.Teacher.Mother_LastName}"
                    : "Sin asignar"
            }).ToList();

            return Ok(result);
        }

        /// <summary>
        /// Obtiene una materia específica por su ID.
        /// </summary>
        /// <param name="id">ID de la materia.</param>
        /// <returns>Objeto <see cref="Subject"/> si existe; 404 si no se encuentra.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Subject>> Get(int id)
        {
            var result = await _dataContext.Subjects
                .Include(s => s.Teacher)
                .FirstOrDefaultAsync(s => s.ID_Subject == id);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        /// <summary>
        /// Crea una nueva materia asignándola a un docente.
        /// Solo puede ser ejecutado por Admins o Teachers.
        /// </summary>
        /// <param name="data">Datos de la materia a crear.</param>
        /// <returns>Materia creada o mensaje de error.</returns>
        [Authorize(Roles = "Admin,Teacher")]
        [HttpPost]
        public async Task<ActionResult<Subject>> Post([FromBody] SubjectInputDTO data)
        {
            var teacher = await _dataContext.Users.FirstOrDefaultAsync(u => u.UserName == data.TeacherUserName);
            if (teacher == null)
                return BadRequest("Docente no encontrado");

            var subject = new Subject
            {
                Name = data.Name,
                Semester = data.Semester,
                ID_Teacher = teacher.Id
            };

            await _dataContext.Subjects.AddAsync(subject);
            await _dataContext.SaveChangesAsync();

            return Ok(subject);
        }

        /// <summary>
        /// Actualiza los datos de una materia existente.
        /// Solo permitido para Admins o Teachers.
        /// </summary>
        /// <param name="id">ID de la materia a actualizar.</param>
        /// <param name="data">Datos actualizados.</param>
        /// <returns>Materia actualizada o mensaje de error.</returns>
        [Authorize(Roles = "Admin,Teacher")]
        [HttpPut("{id}")]
        public async Task<ActionResult<Subject>> Put(int id, [FromBody] SubjectInputDTO data)
        {
            var teacher = await _dataContext.Users.FirstOrDefaultAsync(u => u.UserName == data.TeacherUserName);
            if (teacher == null)
                return BadRequest("Docente no encontrado");

            var subject = await _dataContext.Subjects.FindAsync(id);
            if (subject == null)
                return NotFound();

            subject.Name = data.Name;
            subject.Semester = data.Semester;
            subject.ID_Teacher = teacher.Id;

            try
            {
                await _dataContext.SaveChangesAsync();
                return Ok(subject);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al actualizar la materia: {ex.Message}");
            }
        }

        /// <summary>
        /// Elimina una materia por su ID.
        /// Solo permitido para Admins o Teachers.
        /// </summary>
        /// <param name="id">ID de la materia a eliminar.</param>
        /// <returns>Materia eliminada o mensaje de error.</returns>
        [Authorize(Roles = "Admin,Teacher")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<Subject>> Delete(int id)
        {
            var result = await _dataContext.Subjects.FindAsync(id);

            if (result == null)
                return NotFound();

            try
            {
                _dataContext.Subjects.Remove(result);
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
