using Microsoft.EntityFrameworkCore;
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
    public interface ICollaboratorService
    {
        bool HasCollab(Guid user_id, Guid repository_id);
        void AddCollab(Guid account_id, Guid repository_id);
    }

    public class CollaboratorService : ServiceBase, ICollaboratorService
    {
        private readonly IContext<Account> _account;
        private readonly IContext<Collaborator> _collaborator;

        public CollaboratorService(IContext<Account> account, IContext<Collaborator> collaborator, IErrorHandlerService errorHandler) : base(errorHandler)
        {
            _account = account;
            _collaborator = collaborator;
        }

        public void AddCollab(Guid account_id, Guid repository_id)
        {
            try
            {
                Guid owner_id = _collaborator.Where(c => c.RepositoryId.Equals(repository_id) && c.AccountId.Equals(c.OwnerId))
                    .Select(c => c.OwnerId.Value).FirstOrDefault();
                _collaborator.Add(new Collaborator
                {
                    AccountId = account_id,
                    RepositoryId = repository_id,
                    OwnerId = owner_id
                });
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred while invite a member!",
                    e, DateTime.Now, "Server", "Service_Collaborator_Add");
            }
        }

        public bool HasCollab(Guid user_id, Guid repository_id)
        {
            try
            {
                return _collaborator.Any(c => c.Account.UserId.Equals(user_id) && c.RepositoryId.Equals(repository_id)) ;
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred while invite a member!",
                    e, DateTime.Now, "Server", "Service_Collaborator_Add");
            }
        }

        private int SaveChanges()
        {
            return _collaborator.SaveChanges();
        }
    }
}
