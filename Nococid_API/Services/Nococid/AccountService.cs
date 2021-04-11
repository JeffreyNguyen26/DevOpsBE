using Microsoft.EntityFrameworkCore;
using Nococid_API.Data;
using Nococid_API.Data.Models;
using Nococid_API.Data.Static;
using Nococid_API.Models.Github;
using Nococid_API.Models.Https;
using Nococid_API.Models.Nococid;
using Nococid_API.Models.Nococid.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nococid_API.Services.Nococid
{
    public interface IAccountService
    {
        Guid Add(GHAuthenticatedUserInfo model, Guid vsc_id, out bool is_connected);
        string Connect(Guid user_id, Guid account_id);
        void EnsureOwner(Guid user_id, Guid account_id);
        IList<AccountM> GetAll(Guid user_id);
        Guid GetMainAccountId(Guid user_id);
        IList<RepositoryAccountsM> GetCollaAccounts(Guid user_id, Guid project_id);
        GHUserRequirement GetGHUserRequirement(Guid account_id);
        GHUserRequirement GetGHUserRequirement(Guid user_id, Guid account_id);
    }

    public class AccountService : ServiceBase, IAccountService
    {
        private readonly IContext<Account> _account;
        private readonly IContext<Repository> _repository;
        private readonly IContext<Collaborator> _collaborator;

        public AccountService(IContext<Collaborator> collaborator, IContext<Repository> repository, IContext<Account> account, IErrorHandlerService errorHandler) : base(errorHandler)
        {
            _collaborator = collaborator;
            _repository = repository;
            _account = account;
        }

        public Guid Add(GHAuthenticatedUserInfo model, Guid vsc_id, out bool is_connected)
        {
            try
            {
                Account account = _account.GetOne(a => a.ThirdPartyAccountId.Trim().Equals(model.User.Id.ToString().Trim()));
                if (account == null)
                {
                    account = _account.Add(new Account
                    {
                        Name = model.User.Login,
                        ThirdPartyAccountId = model.User.Id.ToString(),
                        IsMain = false,
                        ToolId = ToolID.Github
                    });
                }
                account.AccessToken = model.Token.Access_token;
                account.AvatarUrl = model.User.Avatar_url;
                account.Email = model.Email;
                SaveChanges();

                is_connected = account.UserId != null;
                return account.Id;
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred while add third party account!",
                    e, DateTime.Now, "Server", "Service_Account_Add");
            }
        }

        public string Connect(Guid user_id, Guid account_id)
        {
            try
            {
                Account account = _account.GetOne(account_id);
                if (account == null) return "AccountId_With_Value_" + account_id.ToString() + "_Not_Found";
                if (account.UserId != null) return "This third party account has connected to a nococid account";

                if (_account.Count(a => a.UserId.Equals(user_id)) == 1)
                {
                    account.IsMain = true;
                } else
                {
                    account.IsMain = false;
                }

                account.UserId = user_id;
                SaveChanges();
                return null;
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred while connect third party account to nococid account!",
                    e, DateTime.Now, "Server", "Service_Account_Connect");
            }
        }

        public void EnsureOwner(Guid user_id, Guid account_id)
        {
            try
            {
                if (!_account.Any(a => a.Id.Equals(account_id) && a.UserId.Equals(user_id))) throw NotFound(account_id, "account id");
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred while ensure the third party owner!",
                   e, DateTime.Now, "Server", "Service_Account_EnsureOwner");
            }
        }

        public IList<AccountM> GetAll(Guid user_id)
        {
            try
            {
                return _account.Where(a => a.UserId.Equals(user_id))
                    .Select(a => new AccountM
                    {
                        AvatarUrl = a.AvatarUrl,
                        Email = a.Email,
                        Id = a.Id,
                        Name = a.Name
                    }).ToList();
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred while get all third party account!",
                    e, DateTime.Now, "Server", "Service_Account_GetAll");
            }
        }

        public IList<RepositoryAccountsM> GetCollaAccounts(Guid user_id, Guid project_id)
        {
            try
            {
                IList<RepositoryAccountsM> repositories = _repository.Where(r => r.ProjectRepositories.Any(pr => pr.ProjectId.Equals(project_id)))
                    .Select(r => new RepositoryAccountsM
                    {
                        Id = r.Id,
                        Name = r.Name
                    }).ToList();
                foreach (var repository in repositories)
                {
                    repository.Accounts = _collaborator.Where(c => c.Account.UserId.Equals(user_id) && c.RepositoryId.Equals(repository.Id))
                        .Select(c => new AccountM
                        {
                            Id = c.Account.Id,
                            AvatarUrl = c.Account.AvatarUrl,
                            Email = c.Account.Email,
                            Name = c.Account.Name
                        }).ToList();
                }
                return repositories;
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred while get colla accounts!",
                    e, DateTime.Now, "Server", "Service_Account_GetCollaAccounts");
            }
        }

        public GHUserRequirement GetGHUserRequirement(Guid user_id, Guid account_id)
        {
            try
            {
                GHUserRequirement result = _account.Where(a => a.Id.Equals(account_id) && a.UserId.Equals(user_id))
                    .Select(a => new GHUserRequirement
                    {
                        Name = a.Name,
                        AccessToken = a.AccessToken
                    }).FirstOrDefault();
                if (result == null) throw NotFound(account_id, "account id");
                return result;
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred while get github user requirement for calling API!",
                    e, DateTime.Now, "Server", "Service_Account_GetGHUserRequirement");
            }
        }

        public GHUserRequirement GetGHUserRequirement(Guid account_id)
        {
            try
            {
                return _account.Where(a => a.Id.Equals(account_id))
                    .Select(a => new GHUserRequirement
                    {
                        Name = a.Name,
                        AccessToken = a.AccessToken
                    }).FirstOrDefault();
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred while get github account requirement for calling API!",
                    e, DateTime.Now, "Server", "Service_Account_GetGHUserRequirement");
            }
        }

        public Guid GetMainAccountId(Guid user_id)
        {
            try
            {
                return _account.Where(a => a.UserId.Equals(user_id) && a.IsMain).Select(a => a.Id).FirstOrDefault();
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred while get account id of main id!",
                    e, DateTime.Now, "Server", "Service_Account_GetMainAccountId");
            }
        }

        private int SaveChanges()
        {
            return _account.SaveChanges();
        }
    }
}