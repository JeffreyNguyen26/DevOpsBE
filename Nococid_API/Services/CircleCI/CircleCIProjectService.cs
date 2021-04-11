using Nococid_API.Models.CircleCIs;
using Nococid_API.Models.Nococid.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nococid_API.Services.CircleCI
{
    public interface ICircleCIProjectService
    {
        void TriggerNewPipeline(string vsc_username, string repository_name, string circle_token, string branch, string vsc_type, IDictionary<string, string> parameters);
    }

    public class CircleCIProjectService : CircleCIServiceBase, ICircleCIProjectService
    {
        public CircleCIProjectService(IErrorHandlerService errorHandler, IHttpRequestService httpRequest) : base(errorHandler, httpRequest) { }

        public void TriggerNewPipeline(string vsc_username, string repository_name, string circle_token, string branch, string vsc_type, IDictionary<string, string> parameters)
        {
            try
            {
                CircleCITriggerPipeline data = new CircleCITriggerPipeline
                {
                    Branch = branch,
                    Parameters = parameters
                };
                _httpRequest.Send(
                    "https://circleci.com/api/v2/project/" + vsc_type + "/" + vsc_username + "/" + repository_name + "/pipeline",
                    Enums.HttpRequestMethod.Post, new KeyValuePair<string, string>[]
                    {
                        Accept_Json,
                        new KeyValuePair<string, string>("Circle-Token", circle_token)
                    }, data
                );
            }
            catch (Exception e)
            {
                throw e is ServerException ? e : _errorHandler.WriteLog("An error occurred while trigger a new pipeline!",
                    e, DateTime.Now, "Server", "Service_CircleCIProject_TriggerNewPipeline");
            }
        }
    }
}
