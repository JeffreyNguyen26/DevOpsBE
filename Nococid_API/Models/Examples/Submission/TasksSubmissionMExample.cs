using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nococid_API.Models.Examples.Submission
{
    public class TasksSubmissionMExample
    {
        public Guid CommitId = Guid.Empty;
        public IList<Guid> TaskIds = new List<Guid>
        {
            Guid.Empty,
            Guid.Empty
        };
    }
}
