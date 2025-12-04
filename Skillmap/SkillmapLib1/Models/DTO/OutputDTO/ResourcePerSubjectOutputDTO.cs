using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillmapLib1.Models.DTO.OutputDTO
{
    public class ResourcePerSubjectOutputDTO
    {
        public int ID_Subject { get; set; }
        public string SubjectName { get; set; } = string.Empty;

        public int ID_Resource { get; set; }
        public string ResourceTitle { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Link { get; set; } = string.Empty;
        public DateTime UploadDate { get; set; }

        public string? CoverImage { get; set; }

        public string ResourceTypeId { get; set; } = string.Empty;
        public string TeacherFullName { get; set; } = string.Empty;
    }
}
