using Nococid_API.Data;
using Nococid_API.Data.Models;
using Nococid_API.Data.Static;
using Nococid_API.Models.Nococid;
using Nococid_API.Models.Nococid.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nococid_API.Services.Nococid
{
    public interface IRoleService
    {
        IList<RoleM> GetAll();
        IList<RoleM> GetMyRoles(Guid user_id, Guid project_id);
        IList<RolePermissionM> GetProjectRoles(Guid project_id);
    }

    public class RoleService : ServiceBase, IRoleService
    {
        private readonly IContext<Role> _role;
        private readonly IContext<Permission> _permission;

        public RoleService(IContext<Permission> permission, IContext<Role> role, IErrorHandlerService errorHandler) : base(errorHandler)
        {
            _permission = permission;
            _role = role;
        }

        public IList<RoleM> GetAll()
        {
            try
            {
                return _role.Where(r => !r.Id.Equals(RoleID.Admin))
                    .Select(r => new RoleM
                    {
                        Id = r.Id,
                        Name = r.Name
                    }).ToList();
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred while get all role!",
                    e, DateTime.Now, "Server", "Service_Role_GetAll");
            }
        }

        public IList<RoleM> GetMyRoles(Guid user_id, Guid project_id)
        {
            try
            {
                var result = _permission.Where(p => p.ProjectId.Equals(project_id) && p.UserId.Equals(user_id))
                    .Select(p => new RoleM
                    {
                        Id = p.Role.Id,
                        Name = p.Role.Name
                    }).ToList();
                if (result.Count == 0) throw NotFound(project_id, "project id");
                return result;
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred while get all role!",
                    e, DateTime.Now, "Server", "Service_Role_GetAll");
            }
        }

        public IList<RolePermissionM> GetProjectRoles(Guid project_id)
        {
            try
            {
                return _role.Select(r => new RolePermissionM
                {
                    Role = new RoleM
                    {
                        Id = r.Id,
                        Name = r.Name
                    },
                    Users = r.Permissions.Where(p => p.ProjectId.Equals(project_id)).Select(p => new UserM
                    {
                        Id = p.User.Id,
                        Username = p.User.Username
                    }).ToList()
                }).ToList();
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred while get all role!",
                    e, DateTime.Now, "Server", "Service_Role_GetProjectRoles");
            }
        }
    }
}
