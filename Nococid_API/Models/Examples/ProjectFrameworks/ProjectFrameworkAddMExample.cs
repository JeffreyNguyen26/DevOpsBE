using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nococid_API.Models.Examples.ProjectFrameworks
{
    public class ProjectFrameworkAddMExample : List<Guid>
    {
        public ProjectFrameworkAddMExample()
        {
            Add(Guid.Empty);
            Add(Guid.Empty);
        }
    }
}
