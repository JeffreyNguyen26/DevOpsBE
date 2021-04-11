using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nococid_API.Models.Github
{
    public class GHRepository
    {
        public string Name { get; set; }
        public int Id { get; set; }//RepoId
        public GHRepositoryOwner Owner { get; set; }
        public string Sha { get; set; }
        public string Languages { get; set; }
    }

    public class GHRepositoryOwner
    {
        public string Login { get; set; }
        public int Id { get; set; }
        public string Avatar_url { get; set; }
    }

    public class GHRepositoryDetail
    {
        public string Default_branch { get; set; }
    }
}
