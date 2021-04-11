using Nococid_API.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nococid_API.Models.Nococid
{
    public class WorkflowM
    {
        public Guid Id { get; set; }
        public int No { get; set; }
    }

    public class WorkflowDM : WorkflowM
    {
        public SprintM Sprint { get; set; }

        public IList<PipelineM> Pipelines { get; set; }
    }

    public class WorkflowCreateM
    {
        public Guid AccountId { get; set; }
        public Guid RepositoryId { get; set; }
        public IList<Guid> BranchIds { get; set; }
        public IList<WorkflowCreateConfigM> Configs { get; set; }
    }

    public class WorkflowCreateConfigM
    {
        public string ConfigTool { get; set; }
        public string Content { get; set; }
    }
}
