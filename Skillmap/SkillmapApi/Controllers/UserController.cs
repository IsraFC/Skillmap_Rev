using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SkillmapLib1.Models.DTO.OutputDTO;
using SkillmapLib1.Models;

namespace SkillmapApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<User> _userManager;

        public UserController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<ActionResult<LoggedInUserOutputDTO>> GetCurrentUser()
        {
            var userName = User.Identity?.Name;

            if (string.IsNullOrEmpty(userName))
                return Unauthorized();

            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
                return NotFound("Usuario no encontrado");

            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault() ?? "Sin rol";

            return Ok(new LoggedInUserOutputDTO
            {
                UserName = user.UserName ?? "",
                Role = role
            });
        }

        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<UserWithRoleOutputDTO>>> GetAllUsers()
        {
            var users = _userManager.Users.ToList();
            var result = new List<UserWithRoleOutputDTO>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);

                result.Add(new UserWithRoleOutputDTO
                {
                    UserName = user.UserName ?? "",
                    Name = user.Name,
                    Father_LastName = user.Father_LastName,
                    Mother_LastName = user.Mother_LastName,
                    Rol = roles.FirstOrDefault() ?? ""
                });
            }

            return Ok(result);
        }
    }
}
