using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Nococid_API.Data.Models
{
    public class Submission : TableBase
    {
        public DateTime? SubmissionTime { get; set; }
        public string Side { get; set; }
        public bool? IsTaskPassed { get; set; }

        public Guid? SprintId { get; set; }

        [JsonIgnore]
        [ForeignKey("SprintId")]
        public virtual Sprint Sprint { get; set; }
        [JsonIgnore]
        [ForeignKey("Id")]
        public virtual Commit Commit { get; set; }

        [JsonIgnore]
        public virtual ICollection<Pipeline> Pipelines { get; set; }
        [JsonIgnore]
        public virtual ICollection<TaskSubmission> Reports { get; set; }
        [JsonIgnore]
        public virtual ICollection<Test> Tests { get; set; }
    }
}
