using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nococid_API.Models.Nococid.Errors
{
    public class ServerExceptionVM
    {
        public string Message { get; set; }
        public int TraceId { get; set; }
        public string Side { get; set; }
    }
}
