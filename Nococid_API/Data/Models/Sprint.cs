using Newtonsoft.Json;
using Nococid_API.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Nococid_API.Data.Models
{
    public class Sprint : TableBase
    {
        public int No { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public StageEnum StageCode { get; set; }
        public bool IsDelete { get; set; }

        public Guid? ProjectId { get; set; }

        [JsonIgnore]
        [ForeignKey("ProjectId")]
        public virtual Project Project { get; set; }

        [JsonIgnore]
        public virtual ICollection<Task> Tasks { get; set; }
        [JsonIgnore]
        public virtual ICollection<Submission> Submissions { get; set; }
        [JsonIgnore]
        public virtual ICollection<Workflow> Workflows { get; set; }
        [JsonIgnore]
        public virtual ICollection<Approval> Approvals { get; set; }
    }
}
