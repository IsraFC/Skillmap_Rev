using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkillmapApi.Data;
using SkillmapLib1.Models;

namespace SkillmapApi.Controllers
{
    [Authorize(Roles = "Admin,Teacher")]
    [Route("api/[controller]")]
    [ApiController]
    public class SubjectController : ControllerBase
    {
        private readonly DataContext _dataContext;

        public SubjectController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        [HttpGet]
        public async Task<List<Subject>> Get()
        {
            return await _dataContext.Subjects
                .Include(s => s.Teacher)
                .ToListAsync();
        }

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

        [HttpPost]
        public async Task<ActionResult<Subject>> Post([FromBody] SubjectInput data)
        {
            var teacher = await _dataContext.Users.FirstOrDefaultAsync(u => u.UserName == data.teacherUserName);
            if (teacher == null)
                return BadRequest("Docente no encontrado");

            var subject = new Subject
            {
                Name = data.name,
                Semester = data.semester,
                ID_Teacher = teacher.Id
            };

            await _dataContext.Subjects.AddAsync(subject);
            await _dataContext.SaveChangesAsync();

            return Ok(subject);
        }


        [HttpPut("{id}")]
        public async Task<ActionResult<Subject>> Put(int id, [FromBody] SubjectInput data)
        {
            var teacher = await _dataContext.Users.FirstOrDefaultAsync(u => u.UserName == data.teacherUserName);
            if (teacher == null)
                return BadRequest("Docente no encontrado");

            var subject = await _dataContext.Subjects.FindAsync(id);
            if (subject == null)
                return NotFound();

            subject.Name = data.name;
            subject.Semester = data.semester;
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

    public class SubjectInput
    {
        public string name { get; set; } = string.Empty;
        public string semester { get; set; } = string.Empty;
        public string teacherUserName { get; set; } = string.Empty;
    }
}
