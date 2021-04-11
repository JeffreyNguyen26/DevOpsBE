using Nococid_API.Data;
using Nococid_API.Data.Models;
using Nococid_API.Enums;
using Nococid_API.Models.CircleCIs;
using Nococid_API.Models.Nococid;
using Nococid_API.Models.Nococid.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nococid_API.Services.Nococid
{
    public interface IPipelineService
    {
        Guid Add(string ci_cd_workflow_id, string job_num, string stage);
        void AddBuild(string ci_cd_workflow_id, StageEnum stage, int step, Guid report_id, TimeSpan duration);
        IList<PipelineJobM> GetJobs(string ci_cd_workflow_id);
        void Update(Guid pipeline_id, string status, TimeSpan duration);
    }

    public class PipelineService : ServiceBase, IPipelineService
    {
        private readonly IContext<Pipeline> _pipeline;
        private readonly IContext<Workflow> _workflow;

        public PipelineService(IContext<Workflow> workflow, IContext<Pipeline> pipeline, IErrorHandlerService errorHandler) : base(errorHandler)
        {
            _workflow = workflow;
            _pipeline = pipeline;
        }

        public Guid Add(string ci_cd_workflow_id, string job_num, string stage)
        {
            try
            {
                Guid workflow_id = _workflow.Where(w => w.CICDWorkflowId.Equals(ci_cd_workflow_id))
                    .Select(w => w.Id).FirstOrDefault();
                if (workflow_id.Equals(Guid.Empty)) throw NotFound();

                StageEnum stage_code = GetStageCode(stage);
                int number_of_jobs_done = _pipeline.Count(p => p.WorkflowId.Equals(workflow_id));
                Pipeline pipeline = _pipeline.Add(new Pipeline
                {
                    CICDJobNum = job_num,
                    Stage = stage_code,
                    WorkflowId = workflow_id,
                    CustomName = stage_code.Equals(StageEnum.Custom) ? stage : null,
                    Step = number_of_jobs_done + 1
                });
                SaveChanges();
                return pipeline.Id;
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred while add a pipeline!",
                    e, DateTime.Now, "Server", "Service_Pipeline_Add");
            }
        }

        public void AddBuild(string ci_cd_workflow_id, StageEnum stage, int step, Guid report_id, TimeSpan duration)
        {
            try
            {
                Guid workflow_id = _workflow.Where(w => w.CICDWorkflowId.Equals(ci_cd_workflow_id))
                    .Select(w => w.Id).FirstOrDefault();

                Pipeline pipeline = _pipeline.Add(new Pipeline
                {
                    Stage = stage,
                    Step = step,
                    WorkflowId = workflow_id
                });
                SaveChanges();
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred while add a build pipeline!",
                    e, DateTime.Now, "Server", "Service_Pipeline_AddBuild");
            }
        }

        public IList<PipelineJobM> GetJobs(string ci_cd_workflow_id)
        {
            try
            {
                return _pipeline.Where(p => p.Workflow.CICDWorkflowId.Equals(ci_cd_workflow_id))
                    .Select(p => new PipelineJobM
                    {
                        CICDJobNum = p.CICDJobNum,
                        Id = p.Id,
                        StageCode = p.Stage
                    }).ToList();
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred while get all !",
                    e, DateTime.Now, "Server", "Service_Pipeline_GetJobs");
            }
        }

        public void Update(Guid pipeline_id, string status, TimeSpan duration)
        {
            try
            {
                Pipeline pipeline = _pipeline.GetOne(pipeline_id);
                pipeline.Status = status;
                pipeline.Duration = duration;
                SaveChanges();
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred while update pipeline!",
                    e, DateTime.Now, "Server", "Service_Pipeline_Update");
            }
        }

        private StageEnum GetStageCode(string stage)
        {
            int index = stage.IndexOf("-");
            if (index >= 0) stage = stage.Substring(0, index);
            StageEnum stage_code = Enum.GetValues(typeof(StageEnum)).Cast<StageEnum>().FirstOrDefault(s => s.ToString().ToLower().Equals(stage.ToLower()));
            if (stage_code.Equals(StageEnum.Planning)) stage_code = StageEnum.Custom;
            return stage_code;
        }

        private int SaveChanges()
        {
            return _pipeline.SaveChanges();
        }
    }
}
