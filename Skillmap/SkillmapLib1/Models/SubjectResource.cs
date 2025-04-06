using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillmapLib1.Models
{
    public class SubjectResource
    {
        // Clave foránea a la materia
        [ForeignKey("Subject")]
        public int ID_Materia { get; set; }
        public Subject Subject { get; set; } = null!;

        // Clave foránea al recurso
        [ForeignKey("ResourceItem")]
        public int ID_Recurso { get; set; }
        public ResourcesItem ResourceItem { get; set; } = null!;
    }
}
