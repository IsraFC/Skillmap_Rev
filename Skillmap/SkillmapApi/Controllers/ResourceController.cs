using System.Resources;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkillmapApi.Data;
using SkillmapLib1.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SkillmapApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResourceController : ControllerBase
    {
        private readonly DataContext _dataContext;
        public ResourceController (DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        // GET: api/<ResourceController>
        [HttpGet]
        public async Task<List<ResourcesItem>> Get()
        {
            return await _dataContext.Resources.ToListAsync();
        }

        // GET api/<ResourceController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ResourcesItem>> Get(int id)
        {
            var result = await _dataContext.Resources.FindAsync(id);
            if (result == null)
            {
                return BadRequest();
            }
            return Ok(result);
        }

        // POST api/<ResourceController>
        [HttpPost]
        public async Task<ActionResult<ResourcesItem>> Post([FromBody] ResourcesItem resources)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(resources);
            }
            try
            {
                await _dataContext.Resources.AddAsync(resources);
                await _dataContext.SaveChangesAsync();
                return Ok(resources);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // PUT api/<ResourceController>/5
        [HttpPut("{id}")]
        public async Task<ActionResult<ResourcesItem>> Put(int id, [FromBody] ResourcesItem resources)
        {
            var result = await _dataContext.Resources.FindAsync(id);
            if (result == null)
            {
                return BadRequest();
            }
            try
            {
                _dataContext.Resources.Update(result);
                await _dataContext.SaveChangesAsync();
                return Ok(result);
            }
            catch (Exception e) 
            {
                return BadRequest(e);
            }
        }

        // DELETE api/<ResourceController>/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ResourcesItem>> Delete(int id)
        {
            var result = await _dataContext.Resources.FindAsync(id);
            if (result == null)
            {
                return BadRequest();
            }
            try
            {
                _dataContext.Resources.Remove(result);
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
