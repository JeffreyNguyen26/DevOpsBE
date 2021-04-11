using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nococid_API.Models.Nococid.Auth
{
    public class JwtM
    {
        public string JwtToken { get; set; }
        public string TokenType { get; set; }
        public Guid AccountId { get; set; }
    }
}
