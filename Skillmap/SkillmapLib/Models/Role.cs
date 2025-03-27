using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillmapLib.Models
{
    public class Role
    {
        [Key]
        public int ID_Role { get; set; }

        [Required]
        public string? Role_Name { get; set; }

        public ICollection<Role>? Roles { get; set; }
    }
}
