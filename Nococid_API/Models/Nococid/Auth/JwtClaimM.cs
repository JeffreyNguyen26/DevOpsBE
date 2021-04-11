using Nococid_API.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nococid_API.Models.Nococid.Auth
{
    public class JwtClaimM
    {
        public Guid AdminUserId { get; set; }
        public Guid UserId { get; set; }
        public string ApplicationRole { get; set; }
    }
}
