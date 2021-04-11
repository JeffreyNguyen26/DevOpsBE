using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nococid_API.Models.Nococid
{
    public class AccountM
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string AvatarUrl { get; set; }
        public string VersionControl { get; set; }
    }

    public class AccountVM
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string AvatarUrl { get; set; }
        public bool IsMain { get; set; }
        public string VersionControl { get; set; }
    }

    public class AccountRepositoriesM : AccountM
    {
        public IList<RepositoryM> Repositories { get; set; }
    }
}
