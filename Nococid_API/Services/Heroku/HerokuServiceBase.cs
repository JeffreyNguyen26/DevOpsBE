using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nococid_API.Services.Heroku
{
    public class HerokuServiceBase
    {
        protected readonly IErrorHandlerService _errorHandler;
        protected readonly IHttpRequestService _httpRequest;

        protected readonly KeyValuePair<string, string> Accept_Json_V3;
        protected readonly KeyValuePair<string, string> Accept_Json_V4;

        public HerokuServiceBase(IErrorHandlerService errorHandler, IHttpRequestService httpRequest)
        {
            _errorHandler = errorHandler;
            _httpRequest = httpRequest;

            Accept_Json_V3 = new KeyValuePair<string, string>("Accept", "application/vnd.heroku+json; version=3");
            Accept_Json_V4 = new KeyValuePair<string, string>("Accept", "application/vnd.heroku+json; version=4");
        }
    }
}