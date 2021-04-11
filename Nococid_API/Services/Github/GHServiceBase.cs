using Nococid_API.Data;
using Nococid_API.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nococid_API.Services.Github
{
    public class GHServiceBase
    {
        protected readonly IErrorHandlerService _errorHandler;
        protected readonly IHttpRequestService _httpRequest;

        protected readonly KeyValuePair<string, string> Accept_V3_Json;

        public GHServiceBase(IErrorHandlerService errorHandler, IHttpRequestService httpRequest)
        {
            _errorHandler = errorHandler;
            _httpRequest = httpRequest;

            Accept_V3_Json = new KeyValuePair<string, string>("Accept", "application/vnd.github.v3+json");
        }
    }
}
