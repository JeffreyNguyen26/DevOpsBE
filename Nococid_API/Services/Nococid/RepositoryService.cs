using Microsoft.EntityFrameworkCore;
using Nococid_API.Data;
using Nococid_API.Data.Models;
using Nococid_API.Data.Static;
using Nococid_API.Models.Github;
using Nococid_API.Models.Nococid;
using Nococid_API.Models.Nococid.Errors;
using Nococid_API.Services.Github;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nococid_API.Services.Nococid
{
    public interface IRepositoryService
    {
        void SetHook(Guid repository_id, string hook_id);
        IList<RepositoryM> AddManyGH(Guid account_id, IList<GHRepository> gh_repositories);
        IList<RepositoryM> GetAll(Guid account_id);
        IList<AccountRepositoriesM> GetProjectOwnerRepositories(Guid user_id);
        GHRepositoryRequirement GetGHRepositoryRequirement(Guid repository_id);
        GHRepositoryRequirement GetGHRepositoryRequirement(Guid account_id, Guid repository_id);
        bool IsCreatedWebhook(Guid id, out string repository_name, out string sha);
        void EnsureRepositoryAccount(Guid id, Guid account_id);
        void EnsureExisted(Guid account_id, Guid repository_id);
        void EnsureExisted(Guid user_id, Guid account_id, Guid repository_id);
        void EnsureRepositoryUser(Guid id, Guid user_id);
        bool HasHook(Guid repository_id);
        object GetForSetup(Guid user_id);
        object GetToken(Guid repository_id);
    }

    public class RepositoryService : ServiceBase, IRepositoryService
    {
        private readonly IContext<Account> _account;
        private readonly IContext<Repository> _repository;
        private readonly IContext<Collaborator> _collaborator;

        public RepositoryService(IContext<Account> account, IContext<Repository> repository, IContext<Collaborator> collaborator, IErrorHandlerService errorHandler) : base(errorHandler)
        {
            _account = account;
            _repository = repository;
            _collaborator = collaborator;
        }

        public IList<RepositoryM> AddManyGH(Guid account_id, IList<GHRepository> gh_repositories)
        {
            try
            {
                IList<Account> new_accounts = new List<Account>();
                Account owner;
                Repository repository;
                IList<Collaborator> new_collaborator = new List<Collaborator>();
                foreach (var gh_repository in gh_repositories)
                {
                    repository = _repository.Where(r => r.ThirdPartyRepositoryId.Equals(gh_repository.Id.ToString())).FirstOrDefault();
                    if (repository == null)
                    {
                        repository = _repository.Add(new Repository
                        {
                            Languages = gh_repository.Languages,
                            Name = gh_repository.Name,
                            ThirdPartyRepositoryId = gh_repository.Id.ToString()
                        });
                    }

                    owner = _account.Where(a => a.ThirdPartyAccountId.Equals(gh_repository.Owner.Id.ToString())).FirstOrDefault();
                    if (owner == null)
                    {
                        owner = _account.Add(new Account
                        {
                            AvatarUrl = gh_repository.Owner.Avatar_url,
                            IsMain = false,
                            ThirdPartyAccountId = gh_repository.Owner.Id.ToString(),
                            Name = gh_repository.Owner.Login,
                            ToolId = ToolID.Github
                        });
                    }

                    if (!_collaborator.Any(c => c.AccountId.Equals(account_id) && c.RepositoryId.Equals(repository.Id)))
                    {
                        _collaborator.Add(new Collaborator
                        {
                            AccountId = account_id,
                            OwnerId = owner.Id,
                            RepositoryId = repository.Id
                        });
                    }

                    if (!owner.Id.Equals(account_id))
                    {
                        if (!_collaborator.Any(c => c.AccountId.Equals(owner.Id) && c.RepositoryId.Equals(repository.Id)))
                        {
                            _collaborator.Add(new Collaborator
                            {
                                OwnerId = owner.Id,
                                AccountId = owner.Id,
                                RepositoryId = repository.Id
                            });
                        }
                    }

                    SaveChanges();
                }
                return GetAll(account_id);
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred while add repositories!",
                    e, DateTime.Now, "Server", "Service_Repository_AddMany");
            }
        }

        public IList<RepositoryM> GetAll(Guid account_id)
        {
            try
            {
                return _collaborator.Where(c => c.AccountId.Equals(account_id) && c.OwnerId.Equals(account_id))
                    .Select(c => new RepositoryM
                    {
                        Id = c.Repository.Id,
                        Name = c.Repository.Name,
                        IsFollow = c.Repository.HookId != null,
                        Languages = c.Repository.Languages
                    }).ToList();
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred while get all repository!",
                    e, DateTime.Now, "Server", "Service_Repository_GetAll");
            }
        }

        public GHRepositoryRequirement GetGHRepositoryRequirement(Guid repository_id)
        {
            try
            {
                var repository = _collaborator.Where(c => c.RepositoryId.Equals(repository_id) && c.AccountId.Equals(c.OwnerId))
                    .Select(c => new
                    {
                        c.Repository.Id,
                        c.Repository.Name,
                        Account = new
                        {
                            c.Account.Id,
                            c.Account.Name,
                            c.Account.AccessToken,
                            UserId = c.Account.UserId.Value
                        }
                    }).FirstOrDefault();

                if (repository == null) throw NotFound();

                return new GHRepositoryRequirement
                {
                    RepositoryName = repository.Name,
                    GHUser = new GHUserRequirement
                    {
                        Name = repository.Account.Name,
                        AccessToken = repository.Account.AccessToken
                    }
                };
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred while get all repository!",
                    e, DateTime.Now, "Server", "Service_Repository_GetAll");
            }
        }

        public GHRepositoryRequirement GetGHRepositoryRequirement(Guid account_id, Guid repository_id)
        {
            try
            {
                var collaborators = _collaborator.Where(c => c.RepositoryId.Equals(repository_id) && (c.AccountId.Equals(account_id) || c.AccountId.Equals(c.OwnerId)))
                    .Select(c => new
                    {
                        c.Repository.Id,
                        RepoName = c.Repository.Name,
                        Account = new
                        {
                            c.Account.Id,
                            c.Account.Name,
                            c.Account.AccessToken,
                            UserId = c.Account.UserId.Value
                        }
                    }).ToList();
                if (collaborators.Count == 0) throw NotFound();

                string account_name;
                string access_token;
                if (collaborators.Count == 1)
                {
                    account_name = collaborators[0].Account.Name;
                    access_token = collaborators[0].Account.AccessToken;
                } else
                {
                    account_name = collaborators[0].Account.Id.Equals(account_id) ? collaborators[1].Account.Name : collaborators[0].Account.Name;
                    access_token = collaborators[0].Account.Id.Equals(account_id) ? collaborators[0].Account.AccessToken : collaborators[1].Account.AccessToken;
                }


                return new GHRepositoryRequirement
                {
                    RepositoryName = collaborators[0].RepoName,
                    GHUser = new GHUserRequirement
                    {
                        Name = account_name,
                        AccessToken = access_token
                    }
                };
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred while get all repository!",
                    e, DateTime.Now, "Server", "Service_Repository_GetAll");
            }
        }

        public bool IsCreatedWebhook(Guid id, out string repository_name, out string sha)
        {
            try
            {
                Repository repository = _repository.GetOne(id);
                if (repository == null) throw NotFound(id, "repository id");

                repository_name = repository.Name;
                sha = null;
                return repository.HookId != null;
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred while create repository webhook!",
                    e, DateTime.Now, "Server", "Service_Repository_EnsureCreatedWebhook");
            }
        }

        private int SaveChanges()
        {
            return _repository.SaveChanges();
        }

        public void SetHook(Guid repository_id, string hook_id)
        {
            try
            {
                Repository repository = _repository.GetOne(repository_id);
                repository.HookId = hook_id;
                SaveChanges();
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred while add hook id to repository!",
                    e, DateTime.Now, "Server", "Service_Repository_AddHook");
            }
        }

        public void EnsureRepositoryAccount(Guid id, Guid account_id)
        {
            try
            {
                Collaborator collaborator = _collaborator.GetOne(c => c.AccountId.Equals(account_id) && c.RepositoryId.Equals(id) && c.AccountId.Equals(c.OwnerId));
                if (collaborator == null) throw NotFound(id, "repository id");
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred while ensure the repository of account!",
                    e, DateTime.Now, "Server", "Service_Repository_EnsureRepositoryAccount");
            }
        }

        public void EnsureRepositoryUser(Guid id, Guid user_id)
        {
            try
            {
                Account account = null;// _account.GetOne(a => a.EmployeeId.Equals(user_id) && a.Collaborators.Any(c => c.AccountId.Equals(c.OwnerId) && c.RepositoryId.Equals(id)));
                if (account == null) throw NotFound(id, "repository id");
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred while ensure the repository of a user!",
                    e, DateTime.Now, "Server", "Service_Repository_EnsureRepositoryUser");
            }
        }

        public void EnsureExisted(Guid account_id, Guid repository_id)
        {
            try
            {
                if (!_collaborator.Any(c => c.RepositoryId.Equals(repository_id) && c.AccountId.Equals(account_id) && c.AccountId.Equals(c.OwnerId)))
                {
                    throw NotFound(repository_id, "repository id");
                }
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred while ensure the repository is existed or not!",
                    e, DateTime.Now, "Server", "Service_Repository_EnsureExisted");
            }
        }

        public bool HasHook(Guid repository_id)
        {
            try
            {
                return _repository.Any(r => r.Id.Equals(repository_id) && r.HookId != null);
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred while ensure the repository's webhook is existed or not!",
                    e, DateTime.Now, "Server", "Service_Repository_EnsureWebhookExisted");
            }
        }

        public IList<AccountRepositoriesM> GetProjectOwnerRepositories(Guid user_id)
        {
            try
            {
                return _account.Where(a => a.UserId.Equals(user_id))
                    .Select(a => new AccountRepositoriesM
                    {
                        AvatarUrl = a.AvatarUrl,
                        Email = a.Email,
                        Id = a.Id,
                        Name = a.Name,
                        Repositories = a.Collaborators.Where(c => c.AccountId.Equals(c.OwnerId)).Select(c => new RepositoryM
                        {
                            Id = c.Repository.Id,
                            IsFollow = c.Repository.HookId != null,
                            Languages = c.Repository.Languages,
                            Name = c.Repository.Name
                        }).ToList()
                    }).ToList();
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred while get project owner repositories!",
                    e, DateTime.Now, "Server", "Service_Repository_GetProjectOwnerRepositories");
            }
        }

        public object GetForSetup(Guid user_id)
        {
            try
            {
                return _collaborator.Where(c => c.Account.UserId.Equals(user_id) && c.AccountId.Equals(c.OwnerId))
                    .Select(c => new
                    {
                        Account = new
                        {
                            c.Account.Id,
                            c.Account.Email,
                            c.Account.Name,
                            c.Account.AvatarUrl
                        },
                        c.Repository.Id,
                        c.Repository.Name,
                        IsFollow = c.Repository.HookId != null,
                        c.Repository.Languages,
                        Projects = c.Repository.ProjectRepositories.Select(pr => new
                        {
                            pr.Side,
                            pr.Project.Id,
                            pr.Project.Name,
                            ProjectType = pr.Project.ProjectType.Name
                        }).ToList()
                    }).ToList();
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred while get repository for setup!",
                    e, DateTime.Now, "Server", "Service_Repository_GetForSetup");
            }
        }

        public void EnsureExisted(Guid user_id, Guid account_id, Guid repository_id)
        {
            try
            {
                if (!_collaborator.Any(c =>
                        c.RepositoryId.Equals(repository_id) &&
                        c.AccountId.Equals(account_id) &&
                        c.Account.UserId.Equals(user_id) &&
                        c.AccountId.Equals(c.OwnerId)))
                    throw NotFound();
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred!",
                    e, DateTime.Now, "Server", "Service_Repository_EnsureExisted");
            }
        }

        public object GetToken(Guid repository_id)
        {
            try
            {
                return _collaborator.Where(c => c.RepositoryId.Equals(repository_id) && c.AccountId.Equals(c.OwnerId))
                    .Select(c => new
                    {
                        RepositoryName = c.Repository.Name,
                        Userame = c.Account.Name,
                        AccessToken = c.Account.AccessToken
                    }).FirstOrDefault();
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred!",
                    e, DateTime.Now, "Server", "Service_Repository_GetToken");
            }
        }
    }
}
