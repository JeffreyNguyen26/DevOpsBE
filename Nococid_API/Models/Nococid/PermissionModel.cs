using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nococid_API.Models.Nococid
{
    public class PermissionM
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }

    public class PermissionRoleSetupM
    {
        public Guid RoleId { get; set; }
        public Guid AssignedUserId { get; set; }
    }
}
