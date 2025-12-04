using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

namespace SkillmapLib1.Models
{
    [PrimaryKey(nameof(ID_Resource),nameof(ID_User))]
    public class ResourceFeedback
    {
        // Clave foránea al recurso
        public int ID_Resource { get; set; }

        // Clave foránea al usuario
        public string ID_User { get; set; } = string.Empty;

        [NotMapped]
        public string? UserName { get; set; }

        // Comentario o retroalimentación
        [MaxLength(500)]
        public string Feedback { get; set; } = string.Empty;
    }
}
