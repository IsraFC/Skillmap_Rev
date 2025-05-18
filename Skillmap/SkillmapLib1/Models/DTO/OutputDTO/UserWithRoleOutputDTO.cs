using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillmapLib1.Models.DTO.OutputDTO
{
    public class UserWithRoleOutputDTO
    {
        public string UserName { get; set; } = "";
        public string Name { get; set; } = "";
        public string Father_LastName { get; set; } = "";
        public string Mother_LastName { get; set; } = "";
        public string Rol { get; set; } = "";
        public string NombreCompleto => $"{Name} {Father_LastName} {Mother_LastName}";
    }
}
