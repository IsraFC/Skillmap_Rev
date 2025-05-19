using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillmapLib1.Models.DTO.InputDTO
{
    public class UserInputDTO
    {
        public string UserName { get; set; } = "";
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
        public string Name { get; set; } = "";
        public string Father_LastName { get; set; } = "";
        public string Mother_LastName { get; set; } = "";
        public string Rol { get; set; } = "";
    }
}
