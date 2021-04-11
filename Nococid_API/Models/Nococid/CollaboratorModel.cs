using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nococid_API.Models.Nococid
{
    public class ThirdPartyCollaboratorM
    {
        public AccountM Account { get; set; }
        public bool IsOwner { get; set; }
        public IList<RepositoryM> Repositories { get; set; }
    }
}
