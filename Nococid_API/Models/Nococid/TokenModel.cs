using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nococid_API.Models.Nococid
{
    public class TokenCreateM
    {
        public Guid ToolId { get; set; }
        public string Token { get; set; }
    }

    public class TokenProjectAccountM
    {
        public Guid ProjectId { get; set; }
        public Guid AccountId { get; set; }
        public Guid ToolId { get; set; }
    }
}
