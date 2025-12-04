using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillmapLib1.Models.DTO.InputDTO
{
    public class ResourceFeedbackInputDTO
    {
        public int ID_Resource { get; set; }
        public string Feedback { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
    }
}
