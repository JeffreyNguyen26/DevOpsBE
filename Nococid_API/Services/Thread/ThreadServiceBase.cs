using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nococid_API.Services.Thread
{
    public class ThreadServiceBase
    {
        protected readonly IErrorHandlerService _errorHandler;
        protected readonly IHttpRequestService _httpRequest;

        protected readonly KeyValuePair<string, string> Accept_Json;

        public ThreadServiceBase(IErrorHandlerService errorHandler, IHttpRequestService httpRequest)
        {
            _errorHandler = errorHandler;
            _httpRequest = httpRequest;

            Accept_Json = new KeyValuePair<string, string>("Accept", "application/json");
        }
    }
}
