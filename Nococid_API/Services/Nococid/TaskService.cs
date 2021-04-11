using Nococid_API.Data;
using Nococid_API.Data.Models;
using Nococid_API.Data.Static;
using Nococid_API.Enums;
using Nococid_API.Models.Nococid;
using Nococid_API.Models.Nococid.Errors;
using Nococid_API.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nococid_API.Services.Nococid
{
    public interface ITaskService
    {
        SprintTasksM AddMany(Guid project_id, Guid sprint_id, TaskSprintCreateM model);
        void EnsureExisted(Guid sprint_id, Guid task_id);
        SprintTasksM GetAll(Guid sprint_id);
        SprintTasksM GetAllMyTask(Guid sprint_id, Guid user_id);
        object GetSubmittedTask(Guid sprint_id);
        IList<TaskImplementingM> GetImplementingTasks(Guid user_id);
        object GetForCommit(Guid user_id, Guid commit_id);
        StageEnum GetNextStage(StageEnum current_stage);
        TaskMoreDM GetTaskMoreDeatil(Guid user_id, Guid project_id, Guid task_id);
    }

    public class TaskService : ServiceBase, ITaskService
    {
        private readonly IContext<Data.Models.Task> _task;
        private readonly IContext<Commit> _commit;
        private readonly IContext<User> _user;
        private readonly IContext<Permission> _permission;
        private readonly IContext<ProjectFramework> _projectFramework;
        private readonly IContext<Language> _language;
        private readonly IContext<Project> _project;
        private readonly IContext<Sprint> _sprint;
        private readonly IContext<Approval> _approval;

        public TaskService(IContext<Approval> approval, IContext<Sprint> sprint, IContext<Project> project, IContext<Language> language, IContext<ProjectFramework> projectFramework, IContext<Permission> permission, IContext<User> user, IContext<Account> account, IContext<Commit> commit, IContext<Data.Models.Task> task, IErrorHandlerService errorHandler) : base(errorHandler)
        {
            _approval = approval;
            _sprint = sprint;
            _project = project;
            _task = task;
            _language = language;
            _projectFramework = projectFramework;
            _permission = permission;
            _user = user;
            _commit = commit;
            _task = task;
        }

        public SprintTasksM AddMany(Guid project_id, Guid sprint_id, TaskSprintCreateM model)
        {
            try
            {
                foreach (var side in model.Sides)
                {
                    if (!("Server".Equals(side.Side) || "Client".Equals(side.Side) || "Database".Equals(side.Side)))
                        throw BadRequest("Side value must be 'Server', 'Client' or 'Database'!");
                    if (side.Tasks.Count == 0) throw BadRequest("No task is assigned!");
                    foreach (var task in side.Tasks)
                    {
                        if (!_permission.Any(p => p.ProjectId.Equals(project_id) && p.UserId.Equals(task.AssignedUserId))) throw NotFound(task.AssignedUserId, "assigned user id");
                    }
                }

                foreach (var side in model.Sides)
                {
                    foreach (var task in side.Tasks)
                    {
                        _task.Add(new Data.Models.Task
                        {
                            Detail = task.Detail,
                            IsDelete = false,
                            Name = task.Name,
                            SprintId = sprint_id,
                            Status = "Incomplete",
                            UserId = task.AssignedUserId,
                            Side = side.Side
                        });
                    }
                }
                SaveChanges();

                return GetAll(sprint_id);
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred while add tasks!",
                    e, DateTime.Now, "Server", "Service_Task_AddMany");
            }
        }

        public void EnsureExisted(Guid sprint_id, Guid task_id)
        {
            try
            {
                if (!_task.Any(t => t.SprintId.Equals(sprint_id) && t.Id.Equals(task_id))) throw NotFound(task_id, "task id");
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred while ensure a tas kis existed!",
                    e, DateTime.Now, "Server", "Service_Task_EnsureExisted");
            }
        }

        public SprintTasksM GetAll(Guid sprint_id)
        {
            try
            {
                var result = _sprint.Where(s => s.Id.Equals(sprint_id))
                    .Select(s => new SprintTasksM
                    {
                        Id = s.Id,
                        EndDate = s.EndDate,
                        NextStage = new Models.Nococid.Stage
                        {
                            Name = StageUtils.GetNextStage(s.StageCode).ToString(),
                            StageCode = StageUtils.GetNextStage(s.StageCode)
                        },
                        Stage = new Models.Nococid.Stage
                        {
                            StageCode = s.StageCode,
                            Name = s.StageCode.ToString()
                        },
                        No = s.No,
                        StartDate = s.StartDate,
                        Tasks = s.StageCode.Equals(StageEnum.Planning) == true ? null : s.Tasks.Select(t => new TaskDM
                        {
                            AssignedUser = new UserM
                            {
                                Id = t.User.Id,
                                Username = t.User.Username
                            },
                            Id = t.Id,
                            Detail = t.Detail,
                            Name = t.Name,
                            Side = t.Side,
                            Status = t.Status
                        }).ToList()
                    }).FirstOrDefault();
                if (result != null)
                {
                    result.IsRequireApproval = _approval.Any(a => a.SprintId.Equals(result.Id) && a.StageCode.Equals(result.NextStage.StageCode));
                }
                return result;
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred while get all tasks!",
                    e, DateTime.Now, "Server", "Service_Task_GetAll");
            }
        }

        public SprintTasksM GetAllMyTask(Guid sprint_id, Guid user_id)
        {
            try
            {
                var result = _sprint.Where(s => s.Id.Equals(sprint_id))
                    .Select(s => new SprintTasksM
                    {
                        Id = s.Id,
                        NextStage = new Models.Nococid.Stage
                        {
                            Name = StageUtils.GetNextStage(s.StageCode).ToString(),
                            StageCode = StageUtils.GetNextStage(s.StageCode)
                        },
                        Stage = new Models.Nococid.Stage
                        {
                            StageCode = s.StageCode,
                            Name = s.StageCode.ToString()
                        },
                        No = s.No,
                        StartDate = s.StartDate,
                        EndDate = s.EndDate,
                        Tasks = s.StageCode.Equals(StageEnum.Planning) == true ? null : s.Tasks.Where(t => t.UserId.Equals(user_id)).Select(t => new TaskDM
                        {
                            AssignedUser = new UserM
                            {
                                Id = t.User.Id,
                                Username = t.User.Username
                            },
                            Id = t.Id,
                            Detail = t.Detail,
                            Name = t.Name,
                            Side = t.Side,
                            Status = t.Status
                        }).ToList()
                    }).FirstOrDefault();
                if (result != null)
                {
                    result.IsRequireApproval = _approval.Any(a => a.SprintId.Equals(result.Id) && a.StageCode.Equals(result.NextStage.StageCode));
                }
                return result;
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred while get all my tasks!",
                    e, DateTime.Now, "Server", "Service_Task_GetAll");
            }
        }

        public object GetForCommit(Guid user_id, Guid commit_id)
        {
            try
            {
                if (!_commit.Any(c => c.Account.UserId.Equals(user_id) && c.Id.Equals(commit_id))) throw NotFound(commit_id, "commit id");

                return _commit.Where(c => c.Id.Equals(commit_id))
                    .Select(c => c.Branch.Repository.ProjectRepositories
                        .Select(pr => new
                        {
                            c.Id,
                            c.Message,
                            c.MessageBody,
                            c.CommitTime,
                            c.IsSubmit,
                            Project = new
                            {
                                pr.Project.Id,
                                pr.Project.Name,
                                ProjectType = pr.Project.ProjectType.Name,
                                Sprint = pr.Project.Sprints.Where(s => s.StageCode.Equals(StageEnum.Coding)).Select(s => new
                                {
                                    s.Id,
                                    s.No,
                                    Tasks = s.Tasks.Where(t => t.SprintId.Equals(s.Id) && t.UserId.Equals(user_id) && (t.Status.Equals("Incomplete") || t.Status.Equals("Error")))
                                        .Select(t => new
                                        {
                                            t.Id,
                                            t.Name,
                                            t.Detail,
                                            t.Status,
                                            t.Side
                                        }).ToList()
                                }).FirstOrDefault()
                            }
                        }).FirstOrDefault())
                    .FirstOrDefault();
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred!",
                    e, DateTime.Now, "Server", "Service_Task_GetForSubmit");
            }
        }

        public TaskMoreDM GetTaskMoreDeatil(Guid user_id, Guid project_id, Guid task_id)
        {
            try
            {
                TaskMoreDM result = _task.Where(t => t.Id.Equals(task_id))
                    .Select(t => new TaskMoreDM
                    {
                        Id = t.Id,
                        Detail = t.Detail,
                        Name = t.Name,
                        Side = t.Side,
                        Status = t.Status
                    }).FirstOrDefault();
                var query_result = _project.Where(p => p.Id.Equals(project_id))
                    .Select(p => new
                    {
                        Repository = p.ProjectRepositories.Where(pr => pr.Side.Equals(result.Side)).Select(pr => new
                        {
                            pr.Repository.Name,
                            VSCAccountNames = pr.Repository.Collaborators.Where(c => c.Account.UserId.Equals(user_id)).Select(c => c.Account.Name).ToList()
                        }).FirstOrDefault(),
                        Frameworks = p.ProjectFrameworks.Where(pf => pf.Framework.Language.Side.Equals(result.Side)).Select(pf => new
                        {
                            pf.Framework.Id,
                            pf.Framework.Name,
                            pf.Framework.Language
                        }).ToList(),
                        Tools= p.ProjectTools.Select(pt => new ProjectToolM
                        {
                            Name = pt.Tool.Name,
                            Stage = pt.Stages,
                            ProjectToolId = pt.Id
                        }).ToList()
                    }).FirstOrDefault();

                result.Tools = query_result.Tools;
                result.Languages = query_result.Frameworks.GroupBy(f => f.Language).Select(l => new TaskLanguagesM
                {
                    Name = l.Key.Name,
                    Frameworks = l.Select(f => new TaskFrameworkM
                    {
                        Name = f.Name
                    }).ToList()
                }).ToList();
                result.VSCAccountNames = query_result.Repository != null ? query_result.Repository.VSCAccountNames : new List<string>();
                result.RepositoryName = result.VSCAccountNames.Count != 0 ? query_result.Repository.Name : null;

                return result;
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred while get more detail of a task!",
                    e, DateTime.Now, "Server", "Service_Task_GetTaskMoreDeatil");
            }
        }

        public IList<TaskImplementingM> GetImplementingTasks(Guid user_id)
        {
            try
            {
                DateTime now = DateTime.Now;
                var query_result = _task.Where(t => t.UserId.Equals(user_id) && t.Sprint.StartDate.Value <= now && t.Sprint.EndDate.Value >= now && t.Sprint.StageCode.Equals(StageEnum.Coding))
                    .Select(t => new
                    {
                        Task = new TaskDM
                        {
                            Id = t.Id,
                            AssignedUser = null,
                            Detail = t.Detail,
                            Name = t.Name,
                            Side = t.Side,
                            Status = t.Status
                        },
                        t.Sprint,
                        t.Sprint.Project,
                        t.Sprint.Project.ProjectType
                    }).ToList();
                var group_result = query_result.GroupBy(r => r.Sprint).GroupBy(s => s.Key.Project);
                return group_result.Select(p => new TaskImplementingM
                {
                    Project = new ProjectM
                    {
                        Name = p.Key.Name,
                        CreatedDate = p.Key.CreatedDate,
                        EndDate = p.Key.EndDate,
                        Id = p.Key.Id,
                        StartDate = p.Key.StartDate,
                        ProjectType = new ProjectTypeM
                        {
                            Id = p.Key.ProjectType.Id,
                            Name = p.Key.ProjectType.Name
                        }
                    },
                    Sprint = p.Select(s => new SprintTasksM
                    {
                        No = s.Key.No,
                        Id = s.Key.Id,
                        EndDate = s.Key.EndDate,
                        StartDate = s.Key.StartDate,
                        Tasks = s.Select(s => s.Task).ToList(),
                        Stage = new Models.Nococid.Stage
                        {
                            StageCode = s.Key.StageCode,
                            Name = s.Key.StageCode.ToString()
                        },
                        NextStage = new Models.Nococid.Stage
                        {
                            StageCode = GetNextStage(s.Key.StageCode),
                            Name = GetNextStage(s.Key.StageCode).ToString()
                        }
                    }).FirstOrDefault()
                }).ToList();
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred while get all today tasks!",
                    e, DateTime.Now, "Server", "Service_Task_GetTodayTasks");
            }
        }

        public StageEnum GetNextStage(StageEnum current_stage)
        {
            try
            {
                return current_stage switch
                {
                    StageEnum.Planning => StageEnum.Coding,
                    StageEnum.Coding => StageEnum.Build,
                    StageEnum.Build => StageEnum.Test,
                    StageEnum.Test => StageEnum.Release,
                    StageEnum.Release => StageEnum.Deploy,
                    StageEnum.Deploy => StageEnum.Operate,
                    StageEnum.Operate => StageEnum.Monitor,
                    _ => StageEnum.Planning,
                };
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred while get next stage!",
                    e, DateTime.Now, "Server", "Service_Sprint_GetNextStage");
            }
        }

        private int SaveChanges()
        {
            return _task.SaveChanges();
        }

        public object GetSubmittedTask(Guid sprint_id)
        {
            try
            {
                return _sprint.Where(s => s.Id.Equals(sprint_id)).Select(s => new
                {
                    Project = new
                    {
                        s.Project.Id,
                        s.Project.Name
                    },
                    Sprint = new
                    {
                        s.Id,
                        s.No
                    },
                    Tasks = s.Tasks.Where(t => t.Status.Equals("Submitted")).Select(t => new
                    {
                        t.Id,
                        t.Name,
                        t.Detail,
                        t.Status,
                        t.Side,
                        SubmissionId = t.TaskSubmissions.FirstOrDefault().SubmissionId.Value
                    }).ToList()
                }).FirstOrDefault();
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred!",
                    e, DateTime.Now, "Server", "Service-Task-GetSubmittedTask");
            }
        }
    }
}
