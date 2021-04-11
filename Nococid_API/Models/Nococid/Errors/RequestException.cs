using Nococid_API.Models.Https;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nococid_API.Models.Nococid.Errors
{
    public class RequestException : Exception
    {
        public RequestException(string message = null) : base(message) { }
        public HttpResponseError Error { get; set; }
    }
}
