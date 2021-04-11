using Nococid_API.Enums;
using Nococid_API.Models.CircleCIs;
using Nococid_API.Models.Nococid.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nococid_API.Services.CircleCI
{
    public interface ICircleCIPipelineService
    {
        string GetCurrentCommitId(Guid circle_pipeline_id, string circle_token);
        CircleCIPipelineM GetPipeline(Guid circle_pipeline_id, string circle_token);
    }

    public class CircleCIPipelineService : CircleCIServiceBase, ICircleCIPipelineService
    {
        public CircleCIPipelineService(IErrorHandlerService errorHandler, IHttpRequestService httpRequest) : base(errorHandler, httpRequest) { }

        public string GetCurrentCommitId(Guid circle_pipeline_id, string circle_token)
        {
            try
            {
                return GetPipeline(circle_pipeline_id, circle_token).Vcs.Revision;
            }
            catch (Exception e)
            {
                throw e is ServerException ? e : _errorHandler.WriteLog("An error occurred while get current commit id!",
                    e, DateTime.Now, "Server", "Service_CircleCIPipeline_GetCurrentCommitId");
            }
        }

        public CircleCIPipelineM GetPipeline(Guid circle_pipeline_id, string circle_token)
        {
            try
            {
                return _httpRequest.Send<CircleCIPipelineM>(
                    "https://circleci.com/api/v2/pipeline/" + circle_pipeline_id.ToString(),
                    HttpRequestMethod.Get, new KeyValuePair<string, string>[]
                    {
                        Accept_Json,
                        new KeyValuePair<string, string>("Circle-Token", circle_token)
                    }
                );
            }
            catch (Exception e)
            {
                throw e is ServerException ? e : _errorHandler.WriteLog("An error occurred while get a pipeline detail!",
                    e, DateTime.Now, "Server", "Service_CircleCIPipeline_GetPipeline");
            }
        }
    }
}
