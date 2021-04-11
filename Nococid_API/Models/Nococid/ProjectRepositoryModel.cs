using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nococid_API.Models.Nococid
{
    public class ProjectRepositorySetupM
    {
        public Guid RepositoryId { get; set; }
        public string Side { get; set; }
    }

    public class ProjectRepositoryM
    {
        public string Side { get; set; }
        public RepositoryM Repository { get; set; }
    }
}
