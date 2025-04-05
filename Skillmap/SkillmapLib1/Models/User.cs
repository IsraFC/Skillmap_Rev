using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace SkillmapLib1.Models
{
    // All the code in this file is included in all platforms.
    public class User : IdentityUser<int>
    {
        [Required, MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        public string Apellido_P { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        public string Apellido_M { get; set; } = string.Empty;

        [ForeignKey("Role")]
        public int ID_Rol { get; set; }

        public Role? Role { get; set; }
    }
}
