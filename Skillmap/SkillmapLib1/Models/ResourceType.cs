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
        public int Id_Tipo_Recurso { get; set; }

        public ICollection<ResourcesItem> ResourcesItems { get; set; } = new List<ResourcesItem>();
    }
}
