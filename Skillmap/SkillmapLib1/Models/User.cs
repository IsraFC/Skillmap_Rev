using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace SkillmapLib1.Models
{
    // All the code in this file is included in all platforms.
    public class User : IdentityUser
    {
        [Required, MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        public string Father_LastName { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        public string Mother_LastName { get; set; } = string.Empty;
    }
}
