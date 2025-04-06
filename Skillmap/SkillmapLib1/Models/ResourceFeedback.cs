using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace SkillmapLib1.Models
{
    [PrimaryKey(nameof(ID_Recurso),nameof(ID_Usuario))]
    public class ResourceFeedback
    {
        // Clave foránea al recurso
        public int ID_Recurso { get; set; }
        public ResourcesItem ResourceItem { get; set; } = null!;

        // Clave foránea al usuario
        public int ID_Usuario { get; set; } 
        public User? User { get; set; }

        // Comentario o retroalimentación
        [MaxLength(500)]
        public string Feedback { get; set; } = string.Empty;
    }
}
