using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nococid_API.Models.Nococid
{
    public class UserCreateM
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class UserM
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
    }

    public class UserLoginM
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class UserAuthorizationM
    {
        public UserM User { get; set; }
        public UserM AdminUser { get; set; }
        public string Jwt { get; set; }
        public bool HasVscAccount { get; set; }
    }

    public class UserPermissionM
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public bool IsRequestUser { get; set; }
        public IList<RoleM> Roles { get; set; }
    }
}
