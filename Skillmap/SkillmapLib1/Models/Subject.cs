using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillmapLib1.Models
{
    public class Subject
    {
        // Clave primaria de la materia
        [Key]
        public int ID_Materia { get; set; }

        // Nombre de la materia
        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        // Semestre correspondiente
        [Required, MaxLength(20)]
        public string Semester { get; set; } = string.Empty;

        // Clave foránea al docente (usuario)
        [ForeignKey("Teacher")]
        public int ID_Docente { get; set; }

        public User Teacher { get; set; } = null!;
    }
}
