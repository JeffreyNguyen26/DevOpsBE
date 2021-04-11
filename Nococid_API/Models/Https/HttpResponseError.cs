using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nococid_API.Models.Https
{
    public class HttpResponseError
    {
        public int StatusCode { get; set; }
        public HttpResponseErrorDetail Detail { get; set; }
    }
}
