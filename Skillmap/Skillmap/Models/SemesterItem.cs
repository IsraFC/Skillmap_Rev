using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skillmap.Models
{
    public class SemesterItem
    {
        public string Name { get; set; }
        public List<SubjectItem> Subjects { get; set; }
        public string SubjectCountText => $"📅 {Subjects.Count} Materias";
    }
}
