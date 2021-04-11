using Nococid_API.Data;
using Nococid_API.Data.Models;
using Nococid_API.Data.Static;
using Nococid_API.Models.Github;
using Nococid_API.Models.Nococid;
using Nococid_API.Models.Nococid.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nococid_API.Services.Nococid
{
    public interface ICommitService
    {
        void Add(GHWebhookPayload model);
        IList<CommitDM> GetAll(Guid user_id);
        object GetSubmission(Guid commit_id);
        void EnsureExisted(Guid user_id, Guid commit_id);
    }

    public class CommitService : ServiceBase, ICommitService
    {
        private readonly IContext<Commit> _commit;
        private readonly IContext<Account> _account;
        private readonly IContext<Branch> _branch;
        private readonly IContext<Data.Models.Task> _task;
        private readonly IContext<TaskSubmission> _report;
        private readonly IContext<Submission> _submission;
        private readonly IContext<Sprint> _sprint;

        public CommitService(IContext<Sprint> sprint, IContext<Submission> submission, IContext<TaskSubmission> report, IContext<Data.Models.Task> task, IContext<Branch> branch, IContext<Account> account, IContext<Commit> commit, IErrorHandlerService errorHandler) : base(errorHandler)
        {
            _sprint = sprint;
            _report = report;
            _task = task;
            _branch = branch;
            _account = account;
            _commit = commit;
            _submission = submission;
        }

        public void Add(GHWebhookPayload model)
        {
            try
            {
                string branch_name = model.Ref.Substring(model.Ref.LastIndexOf("/") + 1);
                var split = model.Head_commit.Message.Split("\n\n");
                string commit_message = split[0];
                string commit_message_body = split.Length == 2 ? split[1] : "";
                
                Guid? account_id = _account.Where(a => a.Name.Equals(model.Head_commit.Author.Username)).Select(a => a.Id).FirstOrDefault();
                if (account_id == null) throw NotFound(model.Head_commit.Author.Username, "github username");
                Guid? branch_id = _branch.Where(b => b.Name.Equals(branch_name) && b.Repository.ThirdPartyRepositoryId.Equals(model.Repository.Id.ToString())).Select(b => b.Id).FirstOrDefault();
                if (branch_id == null) throw NotFound(branch_name, "branch name");

                DateTime.TryParse(model.Head_commit.Timestamp, out DateTime commit_time);
                Guid commit_id = _commit.Add(new Commit
                {
                    Message = commit_message,
                    MessageBody = commit_message_body,
                    AccountId = account_id,
                    PreviousCommitId = model.Before,
                    CurrentCommitId = model.After,
                    BranchId = branch_id,
                    IsSubmit = false,
                    CommitTime = commit_time
                }).Id;

                _submission.Add(new Submission
                {
                    Id = commit_id
                });
                SaveChanges();
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred while add commit!",
                    e, DateTime.Now, "Server", "Service_Commit_Add");
            }
        }

        public void EnsureExisted(Guid user_id, Guid commit_id)
        {
            try
            {
                if (!_commit.Any(c => c.Account.UserId.Equals(user_id) && c.Id.Equals(commit_id))) throw NotFound(commit_id, "commit id");
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred while ensure a commit existed!",
                    e, DateTime.Now, "Server", "Service_Commit_EnsureExisted");
            }
        }

        public IList<CommitDM> GetAll(Guid user_id)
        {
            try
            {
                return _commit.Where(c => c.Account.UserId.Equals(user_id)).OrderBy(c => c.Account)
                    .Select(c => new CommitDM
                    {
                        Id = c.Id,
                        Message = c.Message,
                        MessageBody = c.MessageBody,
                        Account = new CommitAccountM
                        {
                            AvatarUrl = c.Account.AvatarUrl,
                            Email = c.Account.Email,
                            Id = c.Account.Id,
                            Name = c.Account.Name,
                            Repository = new CommitRepositoryM
                            {
                                Id = c.Branch.Repository.Id,
                                Branch = new CommitBranchM
                                {
                                    Id = c.Branch.Id,
                                    Name = c.Branch.Name
                                },
                                Name = c.Branch.Repository.Name,
                                IsFollow = c.Branch.Repository.HookId != null,
                                Languages = c.Branch.Repository.Languages
                            },
                        },
                        CurrentCommitId = c.CurrentCommitId,
                        IsSubmit = c.IsSubmit
                    }).ToList();
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred while get all commit!",
                    e, DateTime.Now, "Server", "Service_Commit_GetAll");
            }
        }

        public object GetSubmission(Guid commit_id)
        {
            try
            {
                return _submission.Where(s => s.Id.Equals(commit_id))
                    .Select(s => new
                    {
                        s.SubmissionTime,
                        Project = new
                        {
                            s.Sprint.Project.Id,
                            s.Sprint.Project.Name,
                            ProjectType = s.Sprint.Project.ProjectType.Name,
                            Sprint = new
                            {
                                s.Sprint.Id,
                                s.Sprint.No,
                                Tasks = s.Reports.Select(r => new
                                {
                                    r.Task.Id,
                                    r.Task.Name,
                                    r.Task.Detail,
                                    r.Task.Side,
                                    r.Task.Status
                                }).ToList()
                            }
                        }
                    }).FirstOrDefault();
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred while get submission from commit!",
                    e, DateTime.Now, "Server", "Service_Commit_GetSubmissions");
            }
        }

        private int SaveChanges()
        {
            return _commit.SaveChanges();
        }
    }
}
