using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Nococid_API.Data;
using Nococid_API.Data.Models;
using Nococid_API.Data.Static;
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
    public interface IWorkflowService
    {
        Guid AddFromCircleCI(Guid sprint_id, Guid branch_id, Guid circle_workflow_id);
        void EnsureExisted(Guid sprint_id, Guid workflow_id);
        void EnsureExisted(Guid project_id, Guid sprint_id, Guid workflow_id);
        int EnsureWorkflowSprint(Guid workflow_id, Guid sprint_id);
        IList<WorkflowM> GetAll(Guid sprint_id);
        WorkflowDM GetDetail(Guid workflow_id);
        IList<StageM> GetWorkStages();
    }

    public class WorkflowService : ServiceBase, IWorkflowService
    {
        private readonly IContext<Workflow> _workflow;
        private readonly IContext<Pipeline> _pipeline;
        private readonly IContext<Sprint> _sprint;
        private readonly IContext<Tool> _tool;

        public WorkflowService(IContext<Tool> tool, IContext<Workflow> workflow, IContext<Pipeline> pipeline, IContext<Sprint> sprint, IErrorHandlerService errorHandler) : base(errorHandler)
        {
            _tool = tool;
            _workflow = workflow;
            _pipeline = pipeline;
            _sprint = sprint;
        }

        public Guid AddFromCircleCI(Guid sprint_id, Guid branch_id, Guid circle_workflow_id)
        {
            try
            {
                int workflow_no = _workflow.Where(w => w.SprintId.Equals(sprint_id)).OrderByDescending(w => w.No)
                    .Select(w => w.No).FirstOrDefault();
                
                Workflow workflow = _workflow.Add(new Workflow
                {
                    IsDelete = false,
                    No = ++workflow_no,
                    SprintId = sprint_id,
                    CICDWorkflowId = "circleci-" + circle_workflow_id.ToString()
                });
                SaveChanges();
                return workflow.Id;
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred while add workflow!",
                    e, DateTime.Now, "Server", "Service_WorkFlow_Add");
            }
        }

        public void EnsureExisted(Guid project_id, Guid sprint_id, Guid workflow_id)
        {
            try
            {
                var query = _sprint.Where(s => s.Id.Equals(sprint_id) && s.Project.Id.Equals(project_id))
                    .Include(s => s.Workflows);
                Sprint sprint = _sprint.GetOne(query);
                if (sprint == null) throw NotFound(sprint_id, "sprint id");
                if (!sprint.Workflows.Any(w => w.Id.Equals(workflow_id))) throw NotFound(workflow_id, "workflow id");
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred while ensure the workflow!",
                    e, DateTime.Now, "Server", "Service_WorkFlow_EnsureExisted");
            }
        }

        public void EnsureExisted(Guid sprint_id, Guid workflow_id)
        {
            try
            {
                if (!_workflow.Any(w => w.SprintId.Equals(sprint_id) && w.Id.Equals(workflow_id))) throw NotFound(workflow_id, "workflow id");
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred while ensure the workflow!",
                   e, DateTime.Now, "Server", "Service_WorkFlow_EnsureExisted");
            }
        }

        public int EnsureWorkflowSprint(Guid workflow_id, Guid sprint_id)
        {
            try
            {
                Workflow workflow = _workflow.GetOne(w => w.Id.Equals(workflow_id) && w.SprintId.Equals(sprint_id));
                if (workflow == null) throw NotFound(workflow_id, "workflow id");

                return workflow.No;
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred while ensure the sprint of workflow!",
                    e, DateTime.Now, "Server", "Service_WorkFlow_Add");
            }
        }

        public IList<WorkflowM> GetAll(Guid sprint_id)
        {
            try
            {
                return _workflow.Where(w => w.SprintId.Equals(sprint_id))
                    .Select(w => new WorkflowM
                    {
                        Id = w.Id,
                        No = w.No
                    }).ToList();
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred while get all workflow!",
                       e, DateTime.Now, "Server", "Service_WorkFlow_GetAll");
            }
        }

        public WorkflowDM GetDetail(Guid workflow_id)
        {
            try
            {
                return _workflow.Where(w => w.Id.Equals(workflow_id))
                    .Select(w => new WorkflowDM
                    {
                        Id = w.Id,
                        No = w.No,
                        Sprint = new SprintM
                        {
                            No = w.Sprint.No,
                            Id = w.Sprint.Id,
                            EndDate = w.Sprint.EndDate,
                            StartDate = w.Sprint.StartDate,
                        },
                        Pipelines = w.Pipelines.Select(p => new PipelineM
                        {
                            Id = p.Id,
                            Stage = new StageM
                            {
                                Code = p.Stage,
                                Name = p.Stage.ToString()
                            },
                            Step = p.Step
                        }).ToList()
                    }).FirstOrDefault();
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred while get workflow detail!",
                       e, DateTime.Now, "Server", "Service_WorkFlow_GetDetail");
            }
        }

        public IList<StageM> GetWorkStages()
        {
            try
            {
                return new List<StageM>
                {
                    new StageM
                    {
                        Code = StageEnum.Build,
                        Name = StageEnum.Build.ToString()
                    },
                    new StageM
                    {
                        Code = StageEnum.Test,
                        Name = StageEnum.Test.ToString()
                    },
                    new StageM
                    {
                        Code = StageEnum.Deploy,
                        Name = StageEnum.Deploy.ToString()
                    }
                };
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred while get workflow stages!",
                    e, DateTime.Now, "Server", "Service_WorkFlow_GetWorkStages");
            }
        }

        private void EnsureStageCode(IList<PipelineCreateM> pipelines)
        {
            bool flag;
            foreach (var pipeline in pipelines)
            {
                flag = true;
                foreach (var stage in Enum.GetValues(typeof(StageEnum)).Cast<StageEnum>())
                {
                    if (pipeline.StageCode.Equals(stage))
                    {
                        flag = false;
                        break;
                    }
                }
                if (flag) throw BadRequest("One of stage code is incorrect!");
            }
        }

        private int SaveChanges()
        {
            return _workflow.SaveChanges();
        }
    }
}
