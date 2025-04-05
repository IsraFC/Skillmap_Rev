using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace SkillmapLib1.Models
{
    public class ResourceFeedback
    {
        // Clave foránea al recurso
        [ForeignKey("ResourceItem")]
        public int ID_Recurso { get; set; }
        public ResourcesItem ResourceItem { get; set; } = null!;

        // Clave foránea al usuario
        [ForeignKey("User")]
        public int ID_Usuario { get; set; } 
        public User User { get; set; } = null!;

        // Comentario o retroalimentación
        [MaxLength(500)]
        public string Feedback { get; set; } = string.Empty;
    }
}
