using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillmapLib1.Models
{
    public class ResourcesItem
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(100)]
        [Required]
        public string? Title { get; set; }

        [MaxLength(500)]
        [Required]
        public string? Description { get; set; }

        [MaxLength(500)]
        [Required]
        public string? Link { get; set; }

        [Required]
        public DateOnly UploadDate { get; set; }
    }
}
