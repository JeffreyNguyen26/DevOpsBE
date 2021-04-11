using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nococid_API.Models.Nococid.Errors
{
    public class ServerException : Exception
    {
        public ServerException(string message = "", string inner_message = "") : base(message, new Exception(inner_message)) { }

        public int TraceId { get; set; }
        public string Side { get; set; }
    }
}
