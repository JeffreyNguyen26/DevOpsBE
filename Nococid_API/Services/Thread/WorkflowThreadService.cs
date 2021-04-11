using Nococid_API.Data;
using Nococid_API.Data.Models;
using Nococid_API.Enums;
using Nococid_API.Models.CircleCIs;
using Nococid_API.Models.Nococid.Errors;
using Nococid_API.Models.Theads;
using Nococid_API.Services.CircleCI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nococid_API.Services.Thread
{
    public interface IWorkflowThreadService
    {
        void CreateFromCircleCI(object data);
    }

    public class WorkflowThreadService : ThreadServiceBase, IWorkflowThreadService
    {
        public WorkflowThreadService(IErrorHandlerService errorHandler, IHttpRequestService httpRequest) : base(errorHandler, httpRequest) { }

        public void CreateFromCircleCI(object data)
        {
            try
            {
                CircleCINococidWorkflowCM model = (CircleCINococidWorkflowCM)data;
                Guid circle_pipeline_id = _httpRequest.Send<CircleCIWorkflowM>(
                    "https://circleci.com/api/v2/workflow/" + model.CircleWorkflowId.ToString(),
                    HttpRequestMethod.Get, new KeyValuePair<string, string>[]
                    {
                        Accept_Json,
                        new KeyValuePair<string, string>("Circle-Token", model.CircleToken)
                    }
                ).Pipeline_id;
                string current_commit_id =_httpRequest.Send<CircleCIPipelineM>(
                    "https://circleci.com/api/v2/pipeline/" + circle_pipeline_id.ToString(),
                    HttpRequestMethod.Get, new KeyValuePair<string, string>[]
                    {
                        Accept_Json,
                        new KeyValuePair<string, string>("Circle-Token", model.CircleToken)
                    }
                ).Vcs.Revision;

                Guid commit_id = Guid.Empty;
                do
                {
                    commit_id = ContextInitialization.NoccidContext.Commit.Where(c => c.CurrentCommitId.Equals(current_commit_id))
                        .Select(c => c.Id).FirstOrDefault();
                } while (commit_id.Equals(Guid.Empty));

                Workflow workflow = ContextInitialization.NoccidContext.WorkFlow.Where(w => w.SprintId.Equals(model.SprintId)).OrderByDescending(w => w.No)
                    .Take(1).FirstOrDefault();

                ContextInitialization.NoccidContext.WorkFlow.Add(new Workflow
                {
                    CICDWorkflowId = "circleci-" + model.CircleWorkflowId.ToString(),
                    CommitId = commit_id,
                    IsDelete = false,
                    SprintId = model.SprintId,
                    No = workflow == null ? 1 : workflow.No + 1,
                    TotalJob = model.TotalJob
                });

                ContextInitialization.NoccidContext.SaveChangesAsync().GetAwaiter().GetResult();
            }
            catch (Exception e)
            {
                throw e is ServerException ? e : _errorHandler.WriteLog("An error occurred!",
                    e, DateTime.Now, "Server", "Service-WorkflowThead-CreateFromCircleCI");
            }
        }
    }
}
