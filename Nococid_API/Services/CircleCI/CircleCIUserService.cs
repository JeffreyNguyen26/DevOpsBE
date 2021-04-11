using Nococid_API.Enums;
using Nococid_API.Models.CircleCIs;
using Nococid_API.Models.Nococid.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nococid_API.Services.CircleCI
{
    public interface ICircleCIUserService
    {
        CircleCIUser Get(string circle_token);
    }

    public class CircleCIUserService : CircleCIServiceBase, ICircleCIUserService
    {
        public CircleCIUserService(IErrorHandlerService errorHandler, IHttpRequestService httpRequest) : base(errorHandler, httpRequest) { }

        public CircleCIUser Get(string circle_token)
        {
            try
            {
                return _httpRequest.Send<CircleCIUser>(
                    "https://circleci.com/api/v2/me?circle-token=" + circle_token,
                    HttpRequestMethod.Get, new KeyValuePair<string, string>[] { }
                );
            }
            catch (Exception e)
            {
                throw e is ServerException ? e : _errorHandler.WriteLog("An error occurred!",
                    e, DateTime.Now, "Server", "Service-CircleCIUser-Get");
            }
        }
    }
}
