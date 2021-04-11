using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nococid_API.Models.Github
{
    public class GHBranchRequirement
    {
        public string Name { get; set; }
        public string Sha { get; set; }
    }

    public class GHRepositoryRequirement
    {
        public string RepositoryName { get; set; }
        public GHUserRequirement GHUser { get; set; }
    }

    public class GHUserRequirement
    {
        public string Name { get; set; }
        public string AccessToken { get; set; }
    }
}
