using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Nococid_API.Data.Models
{
    public class Workflow : TableBase
    {
        public int No { get; set; }
        public bool IsDelete { get; set; }
        public string CICDWorkflowId { get; set; }
        public int TotalJob { get; set; }

        public Guid? SprintId { get; set; }
        public Guid? CommitId { get; set; }

        [JsonIgnore]
        [ForeignKey("CommitId")]
        public virtual Commit Commit { get; set; }
        [JsonIgnore]
        [ForeignKey("SprintId")]
        public virtual Sprint Sprint { get; set; }

        [JsonIgnore]
        public virtual ICollection<Pipeline> Pipelines { get; set; }
    }
}
