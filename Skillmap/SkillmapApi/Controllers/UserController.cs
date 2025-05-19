using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SkillmapLib1.Models.DTO.OutputDTO;
using SkillmapLib1.Models;
using SkillmapLib1.Models.DTO.InputDTO;

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

        [HttpGet("me")]
        public async Task<ActionResult<UserWithRoleOutputDTO>> GetCurrentUser()
        {
            var userName = User.Identity?.Name;

            if (string.IsNullOrEmpty(userName))
                return Unauthorized();

            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
                return NotFound("Usuario no encontrado");

            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault() ?? "Sin rol";

            return Ok(new UserWithRoleOutputDTO
            {
                UserName = user.UserName ?? "",
                Email = user.Email ?? "",
                Name = user.Name,
                Father_LastName = user.Father_LastName,
                Mother_LastName = user.Mother_LastName,
                Rol = role
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
                    Email = user.Email ?? "",
                    UserName = user.UserName ?? "",
                    Name = user.Name,
                    Father_LastName = user.Father_LastName,
                    Mother_LastName = user.Mother_LastName,
                    Rol = roles.FirstOrDefault() ?? "",
                });
            }

            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateUser(UserInputDTO dto)
        {
            if (await _userManager.FindByNameAsync(dto.UserName) is not null)
                return BadRequest("Ya existe un usuario con ese nombre");

            var newUser = new User
            {
                UserName = dto.UserName,
                Email = dto.UserName,
                Name = dto.Name,
                Father_LastName = dto.Father_LastName,
                Mother_LastName = dto.Mother_LastName
            };

            var result = await _userManager.CreateAsync(newUser, dto.Password);
            if (!result.Succeeded)
                return BadRequest("No se pudo crear el usuario");

            var roleResult = await _userManager.AddToRoleAsync(newUser, dto.Rol);
            if (!roleResult.Succeeded)
                return BadRequest("Usuario creado pero no se pudo asignar el rol");

            return Ok("Usuario creado correctamente");
        }

        [HttpPut]
        [Authorize (Roles = "Admin")]
        public async Task<IActionResult> UpdateUser(UpdateUserDTO dto)
        {
            var user = await _userManager.FindByNameAsync(dto.OldUserName);
            if (user == null)
                return NotFound("Usuario no encontrado");

            // Actualizar valores
            user.Name = dto.Name;
            user.Father_LastName = dto.Father_LastName;
            user.Mother_LastName = dto.Mother_LastName;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return BadRequest("No se pudo actualizar el usuario");

            var currentRoles = await _userManager.GetRolesAsync(user);
            var currentRole = currentRoles.FirstOrDefault();
            if (!string.IsNullOrEmpty(currentRole) && currentRole != dto.Rol)
            {
                await _userManager.RemoveFromRoleAsync(user, currentRole);
                await _userManager.AddToRoleAsync(user, dto.Rol);
            }

            return Ok("Usuario actualizado correctamente");
        }

        [HttpDelete("{username}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
                return NotFound("Usuario no encontrado");

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
                return BadRequest("No se pudo eliminar el usuario");

            return Ok("Usuario eliminado correctamente");
        }
    }
}
