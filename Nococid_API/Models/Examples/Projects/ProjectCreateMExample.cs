using Nococid_API.Data;
using Nococid_API.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nococid_API.Models.Examples.Projects
{
    public class ProjectCreateMExample
    {
        public string Name = "Security_Helper";
        public DateTime StartDate = DateTime.Now;
        public DateTime EndDate = DateTime.Now.AddMonths(5);

        public Guid ProjectTypeId = Guid.Empty;
    }
}
