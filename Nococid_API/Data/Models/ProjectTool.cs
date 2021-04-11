using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Nococid_API.Data.Models
{
    public class ProjectTool : TableBase
    {
        public string Stages { get; set; }
        public string WebUrl { get; set; }

        public Guid? ProjectId { get; set; }
        public Guid? ToolId { get; set; }
        public Guid? AccountId { get; set; }

        [JsonIgnore]
        [ForeignKey("AccountId")]
        public virtual Account Account { get; set; }
        [JsonIgnore]
        [ForeignKey("ToolId")]
        public virtual Tool Tool { get; set; }
        [JsonIgnore]
        [ForeignKey("ProjectId")]
        public virtual Project Project { get; set; }
    }
}
