using Nococid_API.Data;
using Nococid_API.Data.Models;
using Nococid_API.Data.Static;
using Nococid_API.Enums;
using Nococid_API.Models.CircleCIs;
using Nococid_API.Models.Nococid.Errors;
using Nococid_API.Models.Theads;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nococid_API.Services.Thread
{
    public interface IPipelineThreadService
    {
        void CreateFromCircleCI(object data);
    }

    public class PipelineThreadService : ThreadServiceBase, IPipelineThreadService
    {
        public PipelineThreadService(IErrorHandlerService errorHandler, IHttpRequestService httpRequest) : base(errorHandler, httpRequest) { }

        public void CreateFromCircleCI(object data)
        {
            try
            {
                CircleCINococidPipelineCM model = (CircleCINococidPipelineCM)data;
                var workflow = ContextInitialization.NoccidContext.WorkFlow.Where(w => w.CICDWorkflowId.Equals("circleci-" + model.CircleWorkflowId.ToString()))
                    .Select(w => new
                    {
                        w.Id,
                        CommitId = w.CommitId.Value,
                        LastStep = w.Pipelines.Count()
                    }).FirstOrDefault();
                if (workflow == null) return;

                StageEnum stage_code = Enum.GetValues(typeof(StageEnum)).Cast<StageEnum>()
                    .FirstOrDefault(s => model.JobName.ToLower().Contains(s.ToString().ToLower()));
                if (stage_code.Equals(StageEnum.Planning)) stage_code = StageEnum.Custom;

                CircleCIJob circle_job;
                do
                {
                    System.Threading.Thread.Sleep(1000);
                    circle_job = _httpRequest.Send<CircleCIJob>(
                    "https://circleci.com/api/v2/project/gh/" + model.Username + "/" + model.Repository + "/job/" + model.CircleJobNum,
                    HttpRequestMethod.Get, new KeyValuePair<string, string>[]
                    {
                        Accept_Json,
                        new KeyValuePair<string, string>("Circle-Token", model.CircleToken)
                    }
                );
                } while (circle_job.Duration == null);

                Pipeline pipeline = ContextInitialization.NoccidContext.Pipeline.Add(new Pipeline
                {
                    CICDJobNum = model.CircleJobNum.ToString(),
                    CustomName = model.JobName,
                    Duration = TimeSpan.FromMilliseconds(circle_job.Duration.Value),
                    Status = circle_job.Status,
                    WorkflowId = workflow.Id,
                    Step = workflow.LastStep + 1,
                    SubmissionId = workflow.CommitId,
                    ToolId = ToolID.CircleCI,
                    Stage = stage_code
                }).Entity;

                IList<CircleCIJobTestItemM>  test_items = _httpRequest.Send<CircleCIJobTestMetaDataM>(
                    "http://circleci.com/api/v2/project/gh/" + model.Username + "/" + model.Repository + "/" + model.CircleJobNum + "/tests",
                    HttpRequestMethod.Get, new KeyValuePair<string, string>[]
                    {
                        Accept_Json,
                        new KeyValuePair<string, string>("Circle-Token", model.CircleToken)
                    }
                ).Items;

                if (test_items != null)
                {
                    if (test_items.Count > 0)
                    {
                        stage_code = StageEnum.Test;
                        int passed = 0;
                        double duration = 0;
                        foreach (var test_item in test_items)
                        {
                            if (test_item.Result.ToLower().Equals("success")) passed++;
                            duration += test_item.Run_time;
                        }

                        Test test = ContextInitialization.NoccidContext.Test.Add(new Test
                        {
                            Duration = TimeSpan.FromMilliseconds(duration * 1000),
                            Failed = test_items.Count - passed,
                            Passed = passed,
                            Type = "Auto",
                            SubmissionId = workflow.CommitId
                        }).Entity;

                        Submission submission = ContextInitialization.NoccidContext.Submission.FindAsync(workflow.CommitId).GetAwaiter().GetResult();
                        submission.IsTaskPassed = test_items.Count == passed;
                        ContextInitialization.NoccidContext.Update(submission);

                        foreach (var test_item in test_items)
                        {
                            ContextInitialization.NoccidContext.TestResult.Add(new TestResult
                            {
                                TestId = test.Id,
                                Message = test_item.Message,
                                Name = test_item.Name,
                                Passed = test_item.Result.ToLower().Equals("success"),
                                ClassName = test_item.Classname,
                                Duration = TimeSpan.FromMilliseconds(test_item.Run_time * 1000)
                            });
                        }
                    }
                }
                ContextInitialization.NoccidContext.SaveChangesAsync().GetAwaiter().GetResult();
            }
            catch (Exception e)
            {
                throw e is ServerException ? e : _errorHandler.WriteLog("An error occurred!",
                    e, DateTime.Now, "Server", "Service-PipelineThread-CreateFromCircleCI");
            }
        }
    }
}
