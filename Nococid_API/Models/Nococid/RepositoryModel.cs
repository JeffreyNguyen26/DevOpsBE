using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nococid_API.Models.Nococid
{
    public class RepositoryM
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool IsFollow { get; set; }
        public string Languages { get; set; }
    }

    public class RepositoryVM
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool IsFollow { get; set; }
        public string Languages { get; set; }
    }

    public class RepositoryDVM
    {
        public object MyProperty { get; set; }
    }

    public class RepositoryAccountsM
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public IList<AccountM> Accounts { get; set; }
    }
}
