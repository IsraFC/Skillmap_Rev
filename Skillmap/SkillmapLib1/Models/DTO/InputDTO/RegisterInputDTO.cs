using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillmapLib1.Models.DTO.InputDTO
{
    public class RegisterInputDTO
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Father_LastName { get; set; } = string.Empty;
        public string Mother_LastName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }
}
