using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Nococid_API.Data.Models
{
    public class ProjectRepository : TableBase
    {
        public string Side { get; set; }

        public Guid? ProjectId { get; set; }
        public Guid? RepositoryId { get; set; }

        [JsonIgnore]
        [ForeignKey("RepositoryId")]
        public virtual Repository Repository { get; set; }
        [JsonIgnore]
        [ForeignKey("ProjectId")]
        public virtual Project Project { get; set; }
    }
}
