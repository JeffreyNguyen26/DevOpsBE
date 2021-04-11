using Newtonsoft.Json;
using Nococid_API.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Nococid_API.Data.Models
{
    public class Pipeline : TableBase
    {
        public TimeSpan Duration { get; set; }
        public StageEnum Stage { get; set; }
        public string CustomName { get; set; }
        public int Step { get; set; }
        public string CICDJobNum { get; set; }
        public string Status { get; set; }

        public Guid? WorkflowId { get; set; }
        public Guid? SubmissionId { get; set; }
        public Guid? ToolId { get; set; }

        [JsonIgnore]
        [ForeignKey("ToolId")]
        public virtual Tool Tool { get; set; }
        [JsonIgnore]
        [ForeignKey("SubmissionId")]
        public virtual Submission Submission { get; set; }
        [JsonIgnore]
        [ForeignKey("WorkflowId")]
        public virtual Workflow Workflow { get; set; }

        [JsonIgnore]
        public virtual ICollection<Test> Tests { get; set; }
    }
}
