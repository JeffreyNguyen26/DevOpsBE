using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Nococid_API.Data.Models
{
    public class TaskSubmission : TableBase
    {
        public bool IsDelete { get; set; }
        public int Passed { get; set; }
        public bool IsTested { get; set; }
        public int Failed { get; set; }
        public string Message { get; set; }

        public Guid? SubmissionId { get; set; }
        public Guid? TaskId { get; set; }

        [JsonIgnore]
        [ForeignKey("TaskId")]
        public virtual Task Task { get; set; }
        [JsonIgnore]
        [ForeignKey("SubmissionId")]
        public virtual Submission Submission { get; set; }
    }
}
