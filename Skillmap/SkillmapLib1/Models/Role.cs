using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace SkillmapLib1.Models
{
    public class Role : IdentityRole<int>
    {
        [Required, MaxLength(50)]
        public string Role_Name { get; set; } = string.Empty;

        // Relación: un rol tiene muchos usuarios
        public ICollection<User>? Usuarios { get; set; }
    }
}
