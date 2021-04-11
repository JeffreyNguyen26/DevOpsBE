using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Nococid_API.Data.Models
{
    public class Executor : TableBase
    {
        public string Name { get; set; }
        public string Language { get; set; }
        public string Version { get; set; }

        public Guid? ToolId { get; set; }

        [JsonIgnore]
        [ForeignKey("ToolId")]
        public virtual Tool Tool { get; set; }

        [JsonIgnore]
        public virtual ICollection<ExecutorImage> ExecutorImages { get; set; }
        [JsonIgnore]
        public virtual ICollection<ResourceClass> ResourceClasses { get; set; }
        [JsonIgnore]
        public virtual ICollection<Step> Steps { get; set; }
    }
}
