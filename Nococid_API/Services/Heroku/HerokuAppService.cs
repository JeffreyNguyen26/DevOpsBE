using Nococid_API.Models.Herokus;
using Nococid_API.Models.Nococid.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nococid_API.Services.Heroku
{
    public interface IHerokuAppService
    {
        HerokuAppCreateSuccessM Create(string app_name, string region, string stack, string bearer_token);
        IList<HerokuAppM> Get(string bearer_token);
    }

    public class HerokuAppService : HerokuServiceBase, IHerokuAppService
    {
        public HerokuAppService(IErrorHandlerService errorHandler, IHttpRequestService httpRequest) : base(errorHandler, httpRequest) { }

        public HerokuAppCreateSuccessM Create(string app_name, string region, string stack, string bearer_token)
        {
            try
            {
                return _httpRequest.Send<HerokuAppCreateSuccessM, HerokuAppCreateM>(
                    "https://api.heroku.com/apps",
                    Enums.HttpRequestMethod.Post, new KeyValuePair<string, string>[]
                    {
                        Accept_Json_V3,
                        new KeyValuePair<string, string>("Authorization", "Bearer " + bearer_token)
                    }, new HerokuAppCreateM
                    {
                        Name = app_name,
                        Region = region,
                        Stack = stack
                    }
                );
            }
            catch (Exception e)
            {
                throw e is ServerException ? e : _errorHandler.WriteLog("An error occurred!",
                    e, DateTime.Now, "Server", "Service-HerokuApp-Create");
            }
        }

        public IList<HerokuAppM> Get(string bearer_token)
        {
            try
            {
                return _httpRequest.Send<IList<HerokuAppM>>(
                    "https://api.heroku.com/apps",
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
                    e, DateTime.Now, "Server", "Service-HerokuApp-Get");
            }
        }
    }
}
