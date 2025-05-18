using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillmapLib1.Models.DTO.OutputDTO
{
    public class SubjectOutputDTO
    {
        public int ID_Subject { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Semester { get; set; } = string.Empty;
        public string TeacherName { get; set; } = string.Empty;
    }
}
