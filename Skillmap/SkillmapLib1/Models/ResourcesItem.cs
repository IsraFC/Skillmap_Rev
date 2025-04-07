using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace SkillmapLib1.Models
{
    public class ResourcesItem
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Title { get; set; } = string.Empty;

        [Required, MaxLength(500)]
        public string Description { get; set; } = string.Empty;

        [Required, MaxLength(500)]
        public string Link { get; set; } = string.Empty;

        [Required]
        public DateTime UploadDate { get; set; }

        // Relación con el tipo de recurso
        [ForeignKey("ResourceType")]
        public string ResourceTypeId { get; set; } = string.Empty;
    }
}
