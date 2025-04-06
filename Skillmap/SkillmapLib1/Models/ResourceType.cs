using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillmapLib1.Models
{
    public class ResourceType
    {
        [Key]
        public string Id_Tipo_Recurso { get; set; } = string.Empty;

        public ICollection<ResourcesItem> ResourcesItems { get; set; } = new List<ResourcesItem>();
    }
}
