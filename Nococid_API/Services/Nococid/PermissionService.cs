using Microsoft.EntityFrameworkCore;
using Nococid_API.Data;
using Nococid_API.Data.Models;
using Nococid_API.Data.Static;
using Nococid_API.Models.Https;
using Nococid_API.Models.Nococid;
using Nococid_API.Models.Nococid.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nococid_API.Services.Nococid
{
    public interface IPermissionService
    {
        void AssignProjectManager(Guid user_id, Guid project_id);
        void AssignProjectMember(Guid user_id, Guid project_id, Guid role_id);
        void EnsureProjectOwner(Guid user_id, Guid project_id);
        void EnsureProjectManager(Guid user_id, Guid project_id);
        void EnsureProjectMember(Guid user_id, Guid project_id);
        void EnsureProjectTechnical(Guid user_id, Guid project_id);
        void EnsureTester(Guid user_id, Guid project_id);
    }

    public class PermissionService : ServiceBase, IPermissionService
    {
        private readonly IContext<Permission> _permission;

        public PermissionService(IContext<Permission> permission, IErrorHandlerService errorHandler) : base(errorHandler)
        {
            _permission = permission;
        }

        public void AssignProjectManager(Guid user_id, Guid project_id)
        {
            try
            {
                Permission permission = _permission.GetOne(p => p.ProjectId.Equals(project_id) && p.RoleId.Equals(RoleID.Project_Manager));
                if (permission == null)
                {
                    _permission.Add(new Permission
                    {
                        RoleId = RoleID.Project_Manager,
                        ProjectId = project_id,
                        UserId = user_id
                    });
                } else
                {
                    if (!permission.UserId.Equals(user_id))
                    {
                        _permission.Remove(permission);
                        _permission.Add(new Permission
                        {
                            RoleId = RoleID.Project_Manager,
                            ProjectId = project_id,
                            UserId = user_id
                        });
                    }
                    else throw BadRequest("The user has already been project manager!");
                }
                SaveChanges();
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred while assign project manager!",
                    e, DateTime.Now, "Server", "Service_Permission_AssignProjectManager");
            }
        }

        public void AssignProjectMember(Guid user_id, Guid project_id, Guid role_id)
        {
            try
            {
                Permission permission = _permission.GetOne(p => p.ProjectId.Equals(project_id) && p.RoleId.Equals(role_id) && p.UserId.Equals(user_id));
                if (permission == null)
                {
                    _permission.Add(new Permission
                    {
                        ProjectId = project_id,
                        RoleId = role_id,
                        UserId = user_id
                    });
                    SaveChanges();
                }
                else throw BadRequest("The role has been assigned to this user!");
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred while assign ensure project member!",
                    e, DateTime.Now, "Server", "Service_Permission_AssignProjectMember");
            }
        }

        public void EnsureProjectManager(Guid user_id, Guid project_id)
        {
            try
            {
                var permissions = _permission.GetAll(p => p.UserId.Equals(user_id) && p.ProjectId.Equals(project_id));
                if (permissions.Count == 0) throw NotFound(project_id, "project id");
                if (!permissions.Any(p => p.RoleId.Equals(RoleID.Project_Manager) || p.RoleId.Equals(RoleID.Admin))) throw Forbidden();
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred while ensure project manager!",
                    e, DateTime.Now, "Server", "Service_Permission_EnsureProjectManager");
            }
        }

        public void EnsureProjectMember(Guid user_id, Guid project_id)
        {
            try
            {
                if (!_permission.Any(p => p.UserId.Equals(user_id) && p.ProjectId.Equals(project_id)))
                {
                    throw NotFound(project_id, "project id");
                }
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred while ensure the project member!",
                    e, DateTime.Now, "Server", "Service_Project_EnsureProjectMember");
            }
        }

        public void EnsureProjectOwner(Guid user_id, Guid project_id)
        {
            try
            {
                if (!_permission.Any(p => p.ProjectId.Equals(project_id) && p.UserId.Equals(user_id) && p.RoleId.Equals(RoleID.Admin)))
                    throw NotFound(project_id, "project id");
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred while ensure project owner!",
                    e, DateTime.Now, "Server", "Service_Permission_EnsureProjectOwner");
            }
        }

        public void EnsureProjectTechnical(Guid user_id, Guid project_id)
        {
            try
            {
                var permissions = _permission.Where(p => p.UserId.Equals(user_id) && p.ProjectId.Equals(project_id)).ToList();
                if (permissions.Count == 0) throw NotFound(project_id, "project id");
                if (!permissions.Any(p => p.RoleId.Equals(RoleID.Project_Manager) || p.RoleId.Equals(RoleID.Technical_Manager) || p.RoleId.Equals(RoleID.Admin))) throw Forbidden();
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred while ensure project technical manager!",
                    e, DateTime.Now, "Server", "Service_Permission_EnsureProjectManager");
            }
        }

        public void EnsureTester(Guid user_id, Guid project_id)
        {
            try
            {
                var permissions = _permission.Where(p => p.UserId.Equals(user_id) && p.ProjectId.Equals(project_id)).ToList();
                if (permissions.Count == 0) throw NotFound(project_id, "project id");
                if (!permissions.Any(p => p.RoleId.Equals(RoleID.Project_Manager) || p.RoleId.Equals(RoleID.Project_Tester) || p.RoleId.Equals(RoleID.Admin))) throw Forbidden();
            }
            catch (Exception e)
            {
                throw e is ServerException ? e : _errorHandler.WriteLog("An error occurred!",
                    e, DateTime.Now, "Server", "Service-Permission-EnsureTester");
            }
        }

        private int SaveChanges()
        {
            return _permission.SaveChanges();
        }
    }
}
