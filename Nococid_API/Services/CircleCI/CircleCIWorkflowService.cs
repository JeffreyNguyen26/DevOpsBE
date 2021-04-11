using Nococid_API.Enums;
using Nococid_API.Models.CircleCIs;
using Nococid_API.Models.Nococid.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nococid_API.Services.CircleCI
{
    public interface ICircleCIWorkflowService
    {
        Guid GetPipelineId(Guid circle_workflow_id, string circle_token);
        CircleCIWorkflowM GetWorkflow(Guid circle_workflow_id, string circle_token);
    }

    public class CircleCIWorkflowService : CircleCIServiceBase, ICircleCIWorkflowService
    {
        public CircleCIWorkflowService(IErrorHandlerService errorHandler, IHttpRequestService httpRequest) : base(errorHandler, httpRequest) { }

        public Guid GetPipelineId(Guid circle_workflow_id, string circle_token)
        {
            try
            {
                return GetWorkflow(circle_workflow_id, circle_token).Pipeline_id;
            }
            catch (Exception e)
            {
                throw e is ServerException ? e : _errorHandler.WriteLog("An error occurred while get pipeline id!",
                    e, DateTime.Now, "Server", "Service_CircleCIWorkflow_GetPipelineId");
            }
        }

        public CircleCIWorkflowM GetWorkflow(Guid circle_workflow_id, string circle_token)
        {
            try
            {
                return _httpRequest.Send<CircleCIWorkflowM>(
                    "https://circleci.com/api/v2/workflow/" + circle_workflow_id.ToString(),
                    HttpRequestMethod.Get, new KeyValuePair<string, string>[]
                    {
                        Accept_Json,
                        new KeyValuePair<string, string>("Circle-Token", circle_token)
                    }
                );
            }
            catch (Exception e)
            {
                throw e is ServerException ? e : _errorHandler.WriteLog("An error occurred while get a workflow detail!",
                    e, DateTime.Now, "Server", "Service_CircleCIWorkflow_GetWorkflow");
            }
        }
    }
}
