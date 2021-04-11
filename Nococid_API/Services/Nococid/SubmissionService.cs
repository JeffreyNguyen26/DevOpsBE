using Nococid_API.Data;
using Nococid_API.Data.Models;
using Nococid_API.Models.Nococid;
using Nococid_API.Models.Nococid.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nococid_API.Services.Nococid
{
    public interface ISubmissionService
    {
        dynamic SubmitTasks(Guid user_id, Guid sprint_id, Guid commit_id, IList<Guid> task_ids);
        Submission EnsureExisted(Guid project_id, Guid sprint_id, Guid submission_id);
        Guid GetSubmissionId(Guid sprint_id, string vsc_commit_id);
        dynamic GetSubmission(Guid commit_id);
        void ReportError(Guid project_id, Guid sprint_id, Guid submission_id, ReportErrorM model);
    }

    public class SubmissionService : ServiceBase, ISubmissionService
    {
        private readonly IContext<Submission> _submission;
        private readonly IContext<Commit> _commit;
        private readonly IContext<Data.Models.Task> _task;
        private readonly IContext<TaskSubmission> _taskSubmission;
        private readonly IContext<Pipeline> _pipeline;
        private readonly IContext<Test> _test;
        private readonly IContext<TestResult> _testResult;

        public SubmissionService(IContext<TestResult> testResult, IContext<Test> test, IContext<Pipeline> pipeline, IContext<TaskSubmission> taskSubmission, IContext<Data.Models.Task> task, IContext<Commit> commit, IContext<Submission> submission, IErrorHandlerService errorHandler) : base(errorHandler)
        {
            _testResult = testResult;
            _test = test;
            _pipeline = pipeline;
            _taskSubmission = taskSubmission;
            _task = task;
            _commit = commit;
            _submission = submission;
        }

        public dynamic SubmitTasks(Guid user_id, Guid sprint_id, Guid commit_id, IList<Guid> task_ids)
        {
            try
            {
                if (task_ids == null) throw BadRequest("Must have a task in oder to submit!");
                if (task_ids.Count == 0) throw BadRequest("Must have a task in oder to submit!");

                Commit commit = _commit.GetOne(c => c.Id.Equals(commit_id));
                if (commit.IsSubmit) throw BadRequest("This commit has been submitted! Kindly submit to other commit!");

                IList<Data.Models.Task> tasks = _task.GetAll(t => t.SprintId.Equals(sprint_id) && t.UserId.Equals(user_id));
                if (tasks.Count == 0) throw NotFound();

                IList<Data.Models.Task> submitted_tasks = new List<Data.Models.Task>();
                foreach (var task_id in task_ids)
                {
                    bool flag = false;
                    foreach (var task in tasks)
                    {
                        if (task.Id.Equals(task_id))
                        {
                            if (task.Status.Equals("Incomplete") || task.Status.Equals("Error"))
                            {
                                submitted_tasks.Add(task);
                            }
                            flag = true;
                            break;
                        }
                    }

                    if (!flag) throw NotFound(task_id, "task id");
                }
                string side = submitted_tasks[0].Side;
                for (int i = 1; i < submitted_tasks.Count(); i++)
                {
                    if (!side.Equals(submitted_tasks[i]))
                    {
                        side = "Both";
                        break;
                    }
                }

                DateTime now = DateTime.Now;
                commit.IsSubmit = true;
                Submission submission = _submission.Where(s => s.Id.Equals(commit_id)).FirstOrDefault();
                submission.SubmissionTime = now;
                submission.SprintId = sprint_id;
                submission.Side = side;

                foreach (var submitted_task in submitted_tasks)
                {
                    submitted_task.Status = "Submitted";
                    _taskSubmission.Add(new TaskSubmission
                    {
                        IsTested = false,
                        TaskId = submitted_task.Id,
                        IsDelete = false,
                        SubmissionId = submission.Id
                    });
                }
                SaveChanges();
                return GetSubmission(commit_id);
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred while submit tasks!",
                    e, DateTime.Now, "Server", "Service_Submission_SubmitTasks");
            }
        }

        public Guid GetSubmissionId(Guid sprint_id, string vsc_commit_id)
        {
            try
            {
                return _submission.Where(s => s.SprintId.Equals(sprint_id) && s.Commit.CurrentCommitId.Equals(vsc_commit_id))
                    .Select(s => s.Id).FirstOrDefault();
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred while update submit from CICD tool!",
                    e, DateTime.Now, "Server", "Service_Submission_UpdateFromCICDTool");
            }
        }

        private int SaveChanges()
        {
            return _submission.SaveChanges();
        }

        public dynamic GetSubmission(Guid commit_id)
        {
            try
            {
                return _submission.Where(s => s.Id.Equals(commit_id))
                    .Select(s => new
                    {
                        Commit = new
                        {
                            s.Commit.Id,
                            s.Commit.Message,
                            s.Commit.MessageBody,
                            s.Commit.CommitTime,
                            s.Commit.IsSubmit,
                            Account = new
                            {

                            },
                            Repository = new
                            {
                                s.Commit.Branch.Repository.Id,
                                s.Commit.Branch.Repository.Name,
                                Branch = new
                                {
                                    s.Commit.Branch.Id,
                                    s.Commit.Branch.Name
                                }
                            }
                        },
                        s.SubmissionTime,
                        Sprint = new
                        {
                            s.Sprint.Id,
                            s.Sprint.No
                        },
                        Tasks = s.Reports.Select(r => new
                        {
                            r.Task.Id,
                            r.Task.Name,
                            r.Task.Detail,
                            r.Task.Status,
                            r.Task.Side,
                            AssignedUser = new
                            {
                                r.Task.User.Id,
                                r.Task.User.Username
                            }
                        }).ToList()
                    }).FirstOrDefault();
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred!",
                    e, DateTime.Now, "Server", "Service-Submission-GetSubmissions");
            }
        }

        public void ReportError(Guid project_id, Guid sprint_id, Guid submission_id, ReportErrorM model)
        {
            try
            {
                Submission submission = EnsureExisted(project_id, sprint_id, submission_id);
                TaskSubmission task_submission = _taskSubmission.GetOne(ts => ts.SubmissionId.Equals(submission_id) && ts.TaskId.Equals(model.TaskId));
                if (task_submission == null) throw NotFound();

                task_submission.Passed = model.Passed;
                task_submission.Failed = model.Failed;
                task_submission.IsTested = true;
                task_submission.Message = model.Message;

                Data.Models.Task task = _task.GetOne(t => t.Id.Equals(model.TaskId));
                task.Status = model.Failed != 0 ? "Error" : "Complete";

                SaveChanges();
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred!",
                    e, DateTime.Now, "Server", "Service-Submission-ReportError");
            }
        }

        public Submission EnsureExisted(Guid project_id, Guid sprint_id, Guid submission_id)
        {
            try
            {
                Submission submission = _submission.GetOne(s => s.Sprint.ProjectId.Equals(project_id) && s.SprintId.Equals(sprint_id) && s.Id.Equals(submission_id));
                if (submission == null) throw NotFound();
                return submission;
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred!",
                    e, DateTime.Now, "Server", "Service-Submission-EnsureExisted");
            }
        }
    }
}