using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nococid_API.Models.Github
{
    public class GHBranch
    {
        public string Name { get; set; }
        public GHBranchCommit Commit { get; set; }
        public bool Protected { get; set; }
        public bool IsDefault { get; set; }
    }

    public class GHBranchCommit
    {
        public string Sha { get; set; }
        public string Url { get; set; }
    }
}
