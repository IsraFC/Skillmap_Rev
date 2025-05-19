using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkillmapApi.Data;
using SkillmapLib1.Models;

namespace SkillmapApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResourceTypeController : ControllerBase
    {
        private readonly DataContext _context;

        public ResourceTypeController(DataContext context)
        {
            _context = context;
        }


        [HttpGet]
        public async Task<List<ResourceType>> Get()
        {
            return await _context.ResourceTypes.ToListAsync();
        }

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
