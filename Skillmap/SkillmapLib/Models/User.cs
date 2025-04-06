using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace SkillmapLib.Models
{
    // All the code in this file is included in all platforms.
    public class User : IdentityUser
    {
        [MaxLength(100)]
        [Required]
        public override string? Email { get; set; }

        [MaxLength(20)]
        [Required]
        public string? Password { get; set; }

        [MaxLength(50)]
        [Required]
        public string? Name { get; set; }

        [MaxLength(50)]
        [Required]
        public string? Ap { get; set; }

        [MaxLength(50)]
        [Required]
        public string? Am { get; set; }

        public Role? Role { get; set; }
    }
}
