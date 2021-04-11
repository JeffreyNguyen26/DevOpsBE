using Microsoft.EntityFrameworkCore;
using Nococid_API.Data;
using Nococid_API.Data.Models;
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
    public interface ISprintService
    {
        Guid Add(Guid project_id, DateTime? start_date, DateTime? end_date, IList<StageEnum> approvals);
        void EnsureExisted(Guid project_id, Guid sprint_id);
        int EnsureSprintProject(Guid sprint_id, Guid project_id);
        IList<SprintM> GetAll(Guid project_id);
        string GetConfigurationFilePath(Guid sprint_id);
        SprintDM GetDetail(Guid sprint_id);
        SprintSubmissionsM GetSubmissions(Guid sprint_id);
        void GoNextStage(Guid sprint_id);
        SprintM GoNextStage(Guid project_id, Guid sprint_id);
        SprintDM UpdateSprint(Guid sprint_id, SprintUpdateM model);
    }

    public class SprintService : ServiceBase, ISprintService
    {
        private readonly IContext<Sprint> _sprint;
        private readonly IContext<Approval> _approval;

        public SprintService(IContext<Approval> approval, IContext<Sprint> sprint, IErrorHandlerService errorHandler) : base(errorHandler)
        {
            _approval = approval;
            _sprint = sprint;
        }

        public Guid Add(Guid project_id, DateTime? start_date, DateTime? end_date, IList<StageEnum> approvals)
        {
            try
            {
                if (start_date == null || end_date == null) throw BadRequest("Must include sprint's start date and end date!");

                IList<StageEnum> stages;
                if (approvals == null)
                {
                    stages = new List<StageEnum> { StageEnum.Planning, StageEnum.Coding };
                } else
                {
                    stages = Enum.GetValues(typeof(StageEnum)).Cast<StageEnum>().OrderBy(s => s).ToList();
                    for (int i = 2; i < stages.Count; i++)
                    {
                        if (!approvals.Any(s => s.Equals(stages[i])))
                        {
                            stages.Remove(stages[i]);
                            i--;
                        }
                    }
                }

                int sprint_no = 0;
                Sprint sprint = _sprint.Where(s => s.ProjectId.Equals(project_id)).OrderByDescending(s => s.No).FirstOrDefault();
                if (sprint != null)
                {
                    sprint_no = sprint.No;
                }

                sprint = _sprint.Add(new Sprint
                {
                    No = ++sprint_no,
                    EndDate = end_date,
                    IsDelete = false,
                    ProjectId = project_id,
                    StartDate = start_date,
                    StageCode = StageEnum.Planning
                });
                foreach (var stage in stages)
                {
                    _approval.Add(new Approval
                    {
                        SprintId = sprint.Id,
                        StageCode = stage
                    });
                }

                return sprint.Id;
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred while add new sprint!",
                    e, DateTime.Now, "Server", "Service_Sprint_Add");
            }
        }

        public void EnsureExisted(Guid project_id, Guid sprint_id)
        {
            try
            {
                if (!_sprint.Any(s => s.Id.Equals(sprint_id) && s.ProjectId.Equals(project_id))) throw NotFound(sprint_id, "sprint id");
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred while ensure the sprint of project!",
                    e, DateTime.Now, "Server", "Service_Sprint_EnsureExisted");
            }
        }

        public int EnsureSprintProject(Guid sprint_id, Guid project_id)
        {
            try
            {
                Sprint sprint = _sprint.GetOne(s => s.Id.Equals(sprint_id) && s.ProjectId.Equals(project_id));
                if (sprint == null) throw NotFound(sprint_id, "sprint id");

                return sprint.No;
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred while ensure the sprint of project!",
                    e, DateTime.Now, "Server", "Service_Sprint_EnsureSprintProject");
            }
        }

        public IList<SprintM> GetAll(Guid project_id)
        {
            try
            {
                var result = _sprint.Where(s => s.ProjectId.Equals(project_id)).OrderByDescending(s => s.No)
                    .Select(s => new SprintM
                    {
                        EndDate = s.EndDate,
                        Id = s.Id,
                        No = s.No,
                        StartDate = s.StartDate,
                        Stage = new Stage
                        {
                            StageCode = s.StageCode,
                            Name = s.StageCode.ToString()
                        },
                        NextStage = new Stage
                        {
                            StageCode = StageUtils.GetNextStage(s.StageCode),
                            Name = StageUtils.GetNextStage(s.StageCode).ToString()
                        }
                    }).ToList();

                foreach (var sprint in result)
                {
                    sprint.IsRequireApproval = _approval.Any(a => a.SprintId.Equals(sprint.Id) && a.StageCode.Equals(sprint.NextStage.StageCode));
                }
                return result;
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred while get all sprint!",
                    e, DateTime.Now, "Server", "Service_Sprint_GetAll");
            }
        }

        public string GetConfigurationFilePath(Guid sprint_id)
        {
            try
            {
                var result = _sprint.Where(s => s.Id.Equals(sprint_id))
                    .Select(s => new
                    {
                        ProjectName = s.Project.Name,
                        SprintNo = s.No
                    }).FirstOrDefault();
                return ".nococid/" + result.ProjectName + "/sprint (" + result.SprintNo + ")";
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred while get all sprint!",
                    e, DateTime.Now, "Server", "Service_Sprint_GetConfigurationFilePath");
            }
        }

        public SprintDM GetDetail(Guid sprint_id)
        {
            try
            {
                return _sprint.Where(s => s.Id.Equals(sprint_id))
                    .Select(s => new SprintDM
                    {
                        Id = s.Id,
                        EndDate = s.EndDate,
                        No = s.No,
                        Stage = new Stage
                        {
                            Name = s.StageCode.ToString(),
                            StageCode = s.StageCode
                        },
                        StartDate = s.StartDate,
                        Approvals = s.Approvals.Select(a => new Stage
                        {
                            StageCode = a.StageCode,
                            Name = a.StageCode.ToString()
                        }).ToList()
                    }).FirstOrDefault();
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred while get sprint detail!",
                    e, DateTime.Now, "Server", "Service_Sprint_GetDetail");
            }
        }

        public SprintSubmissionsM GetSubmissions(Guid sprint_id)
        {
            try
            {
                return _sprint.Where(s => s.Id.Equals(sprint_id))
                    .Select(s => new SprintSubmissionsM
                    {
                        Id = s.Id,
                        EndDate = s.EndDate,
                        No = s.No,
                        Stage = new Stage
                        {
                            StageCode = s.StageCode,
                            Name = s.StageCode.ToString()
                        },
                        StartDate = s.StartDate,
                        Submissions = s.Submissions.Select(sub => new SubmissionDM
                        {
                            SubmissionTime = sub.SubmissionTime,
                            Commit = new CommitDM
                            {
                                Account = new CommitAccountM
                                {
                                    AvatarUrl = sub.Commit.Account.AvatarUrl,
                                    Email = sub.Commit.Account.Email,
                                    Id = sub.Commit.Account.Id,
                                    Name = sub.Commit.Account.Name,
                                    Repository = new CommitRepositoryM
                                    {
                                        Name = sub.Commit.Branch.Repository.Name,
                                        Branch = new CommitBranchM
                                        {
                                            Name = sub.Commit.Branch.Name,
                                            Id = sub.Commit.Branch.Id
                                        },
                                        Id = sub.Commit.Branch.Repository.Id,
                                        IsFollow = sub.Commit.Branch.Repository.HookId != null,
                                        Languages = sub.Commit.Branch.Repository.Languages
                                    }
                                },
                                CurrentCommitId = sub.Commit.CurrentCommitId,
                                Id = sub.Commit.Id,
                                IsSubmit = sub.Commit.IsSubmit,
                                Message = sub.Commit.Message,
                                MessageBody = sub.Commit.MessageBody
                            },
                            Tasks = sub.Reports.Select(r => new TaskDM
                            {
                                AssignedUser = new UserM
                                {
                                    Id = r.Task.User.Id,
                                    Username = r.Task.User.Username
                                },
                                Id = r.Task.Id,
                                Detail = r.Task.Detail,
                                Name = r.Task.Name,
                                Side = r.Task.Side,
                                Status = r.Task.Status
                            }).ToList()
                        }).ToList()
                    }).FirstOrDefault();
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred while get sprint submissions!",
                    e, DateTime.Now, "Server", "Service_Sprint_GetSubmissions");
            }
        }

        public void GoNextStage(Guid sprint_id)
        {
            try
            {
                Sprint sprint = _sprint.GetOne(s => s.Id.Equals(sprint_id));
                if (sprint == null) throw NotFound(sprint_id, "sprint id");
                StageEnum next_stage = StageUtils.GetNextStage(sprint.StageCode);
                if (!_approval.Any(a => a.SprintId.Equals(sprint_id) && a.StageCode.Equals(next_stage)))
                {
                    sprint.StageCode = next_stage;
                    SaveChanges();
                }
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred while go to next stage of a sprint!",
                    e, DateTime.Now, "Server", "Service_Sprint_GoNextStage");
            }
        }

        public SprintM GoNextStage(Guid project_id, Guid sprint_id)
        {
            try
            {
                Sprint sprint = _sprint.GetOne(s => s.Id.Equals(sprint_id) && s.ProjectId.Equals(project_id));
                if (sprint == null) throw NotFound(sprint_id, "sprint id");
                StageEnum next_stage = StageUtils.GetNextStage(sprint.StageCode);
                if (!_approval.Any(a => a.SprintId.Equals(sprint_id) && a.StageCode.Equals(next_stage))) throw BadRequest("The next stage will be automatically approved. This action should not be performed!");

                sprint.StageCode = next_stage;
                SaveChanges();
                next_stage = StageUtils.GetNextStage(next_stage);

                return new SprintM
                {
                    EndDate = sprint.EndDate,
                    Id = sprint.Id,
                    No = sprint.No,
                    StartDate = sprint.StartDate,
                    Stage = new Stage
                    {
                        StageCode = sprint.StageCode,
                        Name = sprint.StageCode.ToString()
                    },
                    NextStage = new Stage
                    {
                        StageCode = next_stage,
                        Name = next_stage.ToString()
                    },
                    IsRequireApproval = _approval.Any(a => a.SprintId.Equals(sprint.Id) && a.StageCode.Equals(next_stage))
                };
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred while go to next stage of a sprint!",
                    e, DateTime.Now, "Server", "Service_Sprint_GoNextStage");
            }
        }

        public SprintDM UpdateSprint(Guid sprint_id, SprintUpdateM model)
        {
            try
            {
                IList<StageEnum> stages;
                if (model.Approvals == null)
                {
                    stages = new List<StageEnum> { StageEnum.Planning, StageEnum.Coding };
                }
                else
                {
                    stages = Enum.GetValues(typeof(StageEnum)).Cast<StageEnum>().OrderBy(s => s).ToList();
                    for (int i = 2; i < stages.Count; i++)
                    {
                        if (!model.Approvals.Any(s => s.Equals(stages[i])))
                        {
                            stages.Remove(stages[i]);
                            i--;
                        }
                    }
                }

                Sprint sprint = _sprint.GetOne(sprint_id);
                sprint.StartDate = model.StartDate != null ? model.StartDate : sprint.StartDate;
                sprint.EndDate = model.EndDate != null ? model.EndDate : sprint.EndDate;
                IList<Approval> approvals = _approval.GetAll(a => a.SprintId.Equals(sprint_id));
                _approval.DeleteAll(approvals);
                SaveChanges();
                foreach (var stage in stages)
                {
                    _approval.Add(new Approval
                    {
                        SprintId = sprint_id,
                        StageCode = stage
                    });
                }
                SaveChanges();

                return new SprintDM
                {
                    Approvals = stages.Select(s => new Stage
                    {
                        StageCode = s,
                        Name = s.ToString()
                    }).ToList(),
                    Stage = new Stage
                    {
                        Name = sprint.StageCode.ToString(),
                        StageCode = sprint.StageCode
                    },
                    EndDate = sprint.EndDate,
                    Id = sprint.Id,
                    No = sprint.No,
                    StartDate = sprint.StartDate
                };
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred while update a sprint!",
                    e, DateTime.Now, "Server", "Service_Sprint_UpdateSprint");
            }
        }

        private int SaveChanges()
        {
            return _sprint.SaveChanges();
        }
    }
}
