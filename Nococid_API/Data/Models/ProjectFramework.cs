using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Nococid_API.Data.Models
{
    public class ProjectFramework : TableBase
    {
        public Guid? ProjectId { get; set; }
        public Guid? FrameworkId { get; set; }

        [JsonIgnore]
        [ForeignKey("FrameworkId")]
        public virtual Framework Framework { get; set; }
        [JsonIgnore]
        [ForeignKey("ProjectId")]
        public virtual Project Project { get; set; }
    }
}
