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
                .FirstOrDefaultAsync(s => s.ID_Materia == id);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<Subject>> Post([FromBody] Subject subject)
        {
            if (!ModelState.IsValid)
                return BadRequest(subject);

            try
            {
                await _dataContext.Subjects.AddAsync(subject);
                await _dataContext.SaveChangesAsync();
                return Ok(subject);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Subject>> Put(int id, [FromBody] Subject subject)
        {
            if (id != subject.ID_Materia)
                return BadRequest("ID mismatch.");

            _dataContext.Entry(subject).State = EntityState.Modified;

            try
            {
                await _dataContext.SaveChangesAsync();
                return Ok(subject);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
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
}
