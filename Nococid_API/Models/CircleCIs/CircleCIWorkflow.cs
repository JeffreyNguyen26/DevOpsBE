using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nococid_API.Models.CircleCIs
{
    public class CircleCIWorkflowM
    {
        public Guid Pipeline_id { get; set; }
    }

    public class CircleCIWorkflowStart
    {
        public Guid SprintId { get; set; }
        public Guid BranchId { get; set; }
    }

    public class CircleCIWorkflowStartResult
    {
        public CircleCIWorkflowStart CircleCIWorkflowStart { get; set; }
        public Guid CircleWorkflowId { get; set; }
        public Guid NococidWorkflowId { get; set; }
    }
}
