using Nococid_API.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nococid_API.Models.Nococid
{
    public class PipelineM
    {
        public Guid Id { get; set; }
        public int Step { get; set; }

        public StageM Stage { get; set; }
        public ToolM Tool { get; set; }
    }

    public class PipelineJobM
    {
        public Guid Id { get; set; }
        public StageEnum StageCode { get; set; }
        public string CICDJobNum { get; set; }
    }

    public class PipelineCurJobM
    {
        public Guid CurrentJobId { get; set; }
        public Guid WorkflowId { get; set; }
        public PipelineJobM PreJob { get; set; }
    }

    public class PipelineCreateM
    {
        public int Step { get; set; }
        public StageEnum StageCode { get; set; }
    }

    public class StageM
    {
        public StageEnum Code { get; set; }
        public string Name { get; set; }
    }
}
