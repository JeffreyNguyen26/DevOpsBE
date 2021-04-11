using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nococid_API.Models.Nococid
{
    public class ProjectToolsM
    {
        public ProjectM Project { get; set; }
        public IList<ProjectToolM> Tools { get; set; }
    }

    public class ProjectToolM
    {
        public Guid ProjectToolId { get; set; }
        public string Name { get; set; }
        public string Stage { get; set; }
    }

    public class ProjectToolCreateM
    {
        public Guid ToolId { get; set; }
        public string Stage { get; set; }
    }
}
