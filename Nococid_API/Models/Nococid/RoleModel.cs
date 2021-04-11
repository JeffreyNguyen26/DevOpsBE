using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nococid_API.Models.Nococid
{
    public class RoleM
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }

    public class RolePermissionM
    {
        public RoleM Role { get; set; }
        public IList<UserM> Users { get; set; }
    }
}
