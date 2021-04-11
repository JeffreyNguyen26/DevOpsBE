using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nococid_API.Models.Nococid
{
    public class SubmissionDM
    {
        public DateTime? SubmissionTime { get; set; }
        public CommitDM Commit { get; set; }
        public IList<TaskDM> Tasks { get; set; }
    }

    public class TasksSubmissionM
    {
        public Guid CommitId { get; set; }
        public IList<Guid> TaskIds { get; set; }
    }

    public class ReportErrorM
    {
        public Guid TaskId { get; set; }
        public int Passed { get; set; }
        public int Failed { get; set; }
        public string Message { get; set; }
    }
}
