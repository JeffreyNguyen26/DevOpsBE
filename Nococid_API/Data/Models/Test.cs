using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Nococid_API.Data.Models
{
    public class Test : TableBase
    {
        public string Type { get; set; }
        public int Passed { get; set; }
        public int Failed { get; set; }
        public TimeSpan? Duration { get; set; }

        public Guid? SubmissionId { get; set; }

        [JsonIgnore]
        [ForeignKey("SubmissionId")]
        public virtual Submission Submission { get; set; }
        [JsonIgnore]
        [ForeignKey("PipelineId")]
        public virtual Pipeline Pipeline { get; set; }

        [JsonIgnore]
        public virtual ICollection<TestResult> TestResults { get; set; }
    }
}
