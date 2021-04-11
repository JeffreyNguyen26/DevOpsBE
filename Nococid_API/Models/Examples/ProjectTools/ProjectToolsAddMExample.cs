using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nococid_API.Models.Examples.ProjectTools
{
    public class ProjectToolsAddMExample : List<ProjectToolAddMExample>
    {
        public ProjectToolsAddMExample()
        {
            Add(new ProjectToolAddMExample());
        }
    }

    public class ProjectToolAddMExample
    {
        public Guid ToolId { get; set; } = Guid.Empty;
        public string Stage { get; set; } = "Deploy";
    }
}
