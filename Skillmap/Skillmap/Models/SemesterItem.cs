using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkillmapLib1.Models.DTO.OutputDTO;

namespace Skillmap.Models
{
    public class SemesterItem
    {
        public string Name { get; set; }
        public List<SubjectOutputDTO> Subjects { get; set; }
        public string SubjectCountText => $"📅 {Subjects.Count} Materias";
    }
}
