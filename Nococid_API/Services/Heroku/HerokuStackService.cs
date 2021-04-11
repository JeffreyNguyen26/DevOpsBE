using Nococid_API.Models.Herokus;
using Nococid_API.Models.Nococid.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nococid_API.Services.Heroku
{
    public interface IHerokuStackService
    {
        IList<HerokuStackM> Get(string bearer_token);
    }

    public class HerokuStackService : HerokuServiceBase, IHerokuStackService
    {
        public HerokuStackService(IErrorHandlerService errorHandler, IHttpRequestService httpRequest) : base(errorHandler, httpRequest) { }

        public IList<HerokuStackM> Get(string bearer_token)
        {
            try
            {
                return _httpRequest.Send<IList<HerokuStackM>>(
                    "https://api.heroku.com/stacks",
                    Enums.HttpRequestMethod.Get, new KeyValuePair<string, string>[]
                    {
                        Accept_Json_V3,
                        new KeyValuePair<string, string>("Authorization", "Bearer " + bearer_token)
                    }
                );
            }
            catch (Exception e)
            {
                throw e is ServerException ? e : _errorHandler.WriteLog("An error occurred!",
                    e, DateTime.Now, "Server", "Service-HerokuRegion-Get");
            }
        }
    }
}
