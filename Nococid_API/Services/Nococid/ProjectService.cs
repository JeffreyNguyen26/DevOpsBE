using Microsoft.EntityFrameworkCore;
using Nococid_API.Data;
using Nococid_API.Data.Models;
using Nococid_API.Data.Static;
using Nococid_API.Models.Https;
using Nococid_API.Models.Nococid;
using Nococid_API.Models.Nococid.Errors;
using Nococid_API.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nococid_API.Services.Nococid
{
    public interface IProjectService
    {
        ProjectM Add(Guid admin_user_id, Guid user_id, ProjectCreateM model);
        IList<ProjectLastSprintM> GetAll(Guid user_id);
        ProjectDM GetDetail(Guid user_id, Guid project_id);
        ProjectTechnologyM GetTechnology(Guid project_id);
        void EnsureExisted(Guid project_id);
        void SetupTechnology(ProjectTechnologySetupM model, Guid project_id);
    }

    public class ProjectService : ServiceBase, IProjectService
    {
        private readonly IContext<Project> _project;
        private readonly IContext<Role> _role;
        private readonly IContext<ProjectType> _projectType;
        private readonly IContext<Permission> _permission;
        private readonly IContext<Repository> _repository;
        private readonly IContext<Branch> _branch;
        private readonly IContext<ProjectTool> _projectTool;
        private readonly IContext<Tool> _tool;
        private readonly IContext<Account> _account;
        private readonly IContext<User> _user;
        private readonly IContext<Approval> _approval;

        public ProjectService(IContext<Approval> approval, IContext<User> user, IContext<Account> account, IContext<Tool> tool, IContext<ProjectTool> projectTool, IContext<Repository> repository, IContext<Branch> branch, IContext<Permission> permission, IContext<Project> project, IContext<Role> role, IContext<ProjectType> projectType, IErrorHandlerService errorHandler) : base(errorHandler)
        {
            _approval = approval;
            _user = user;
            _account = account;
            _tool = tool;
            _projectTool = projectTool;
            _repository = repository;
            _branch = branch;
            _permission = permission;
            _project = project;
            _role = role;
            _projectType = projectType;
        }

        public ProjectM Add(Guid admin_user_id, Guid user_id, ProjectCreateM model)
        {
            try
            {
                if (!admin_user_id.Equals(Guid.Empty)) throw Forbidden();
                if (model.Name.Contains("/")) throw BadRequest("Project name can not contain slash(/)!");
                ProjectType project_type = _projectType.GetOne(p => p.Id.Equals(model.ProjectTypeId));
                if (project_type == null) throw NotFound(model.ProjectTypeId, "project type id");
                if (_project.Any(p => p.Name.Equals(model.Name) && p.Permissions.Any(p => p.UserId.Equals(user_id) && p.RoleId.Equals(RoleID.Admin)))) throw BadRequest("The project name is already existed!");
                
                Project project = _project.Add(new Project
                {
                    ProjectTypeId = model.ProjectTypeId,
                    IsDelete = false,
                    Name = model.Name,
                    StartDate = model.StartDate,
                    CreatedDate = DateTime.Now,
                    EndDate = model.EndDate
                });
                _permission.Add(new Permission
                {
                    UserId = user_id,
                    ProjectId = project.Id,
                    RoleId = RoleID.Admin
                });
                SaveChanges();

                return new ProjectM
                {
                    Id = project.Id,
                    CreatedDate = project.CreatedDate,
                    EndDate = project.EndDate,
                    Name = project.Name,
                    StartDate = project.StartDate,
                    ProjectType = new ProjectTypeM
                    {
                        Id = project_type.Id,
                        Name = project_type.Name
                    },
                    Owner = _user.Where(u => u.Id.Equals(user_id)).Select(u => new UserM
                    {
                        Id = u.Id,
                        Username = u.Username
                    }).FirstOrDefault()
                };
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred while add project!",
                    e, DateTime.Now, "Server", "Service_Project_Add");
            }
        }

        public void EnsureExisted(Guid project_id)
        {
            try
            {
                if (_project.Any(p => p.Id.Equals(project_id))) throw NotFound(project_id, "project id");
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred while ensure that the project is existed!",
                    e, DateTime.Now, "Server", "Service_Project_EnsureExisted");
            }
        }

        public IList<ProjectLastSprintM> GetAll(Guid user_id)
        {
            try
            {
                var result = _permission.Where(p => p.Project.Permissions.Any(p => p.UserId.Equals(user_id)) && p.RoleId.Equals(RoleID.Admin))
                    .Select(p => new ProjectLastSprintM
                    {
                        CreatedDate = p.Project.CreatedDate,
                        EndDate = p.Project.EndDate,
                        Id = p.Project.Id,
                        Name = p.Project.Name,
                        ProjectType = new ProjectTypeM
                        {
                            Id = p.Project.ProjectType.Id,
                            Name = p.Project.ProjectType.Name
                        },
                        StartDate = p.Project.StartDate,
                        Owner = new UserM
                        {
                            Id = p.User.Id,
                            Username = p.User.Username
                        },
                        TotalSprint = p.Project.Sprints.Count(),
                        LastSprint = p.Project.Sprints.OrderByDescending(s => s.No).Select(s => new SprintM
                        {
                            No = s.No,
                            EndDate = s.EndDate,
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
                            StartDate = s.StartDate
                        }).FirstOrDefault()
                    }).ToList();

                foreach (var project in result)
                {
                    if (project.LastSprint != null)
                    {
                        project.LastSprint.IsRequireApproval = _approval.Any(a => a.SprintId.Equals(project.LastSprint.Id) && a.StageCode.Equals(project.LastSprint.NextStage.StageCode));
                    }
                }
                return result;
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred while get all project!",
                    e, DateTime.Now, "Server", "Service_Project_GetAll");
            }
        }

        public ProjectDM GetDetail(Guid user_id, Guid project_id)
        {
            try
            {
                var query = _permission.Where(p => p.ProjectId.Equals(project_id))
                    .Include(p => p.Project).ThenInclude(p => p.ProjectType)
                    .Include(p => p.User)
                    .Include(p => p.Role).OrderBy(p => p.UserId);
                IList<Permission> permissions = _permission.GetAll(query);
                if (permissions.Count == 0) throw NotFound(project_id, "project id");

                IList<RoleM> roles = new List<RoleM>
                {
                    new RoleM
                    {
                        Id = permissions[0].Role.Id,
                        Name = permissions[0].Role.Name
                    }
                };
                IList<UserPermissionM> users = new List<UserPermissionM>
                {
                    new UserPermissionM
                    {
                        Id = permissions[0].User.Id,
                        IsRequestUser = permissions[0].User.Id.Equals(user_id),
                        Username = permissions[0].User.Username,
                        Roles = roles
                    }
                };

                for (int i = 1; i < permissions.Count; i++)
                {
                    if (!permissions[i].UserId.Equals(permissions[i - 1].UserId))
                    {
                        roles = new List<RoleM>();
                        users.Add(new UserPermissionM
                        {
                            Id = permissions[i].User.Id,
                            Username = permissions[i].User.Username,
                            IsRequestUser = permissions[i].User.Id.Equals(user_id),
                            Roles = roles
                        });
                    }
                    roles.Add(new RoleM
                    {
                        Id = permissions[i].Role.Id,
                        Name = permissions[i].Role.Name
                    });
                }

                return new ProjectDM
                {
                    CreatedDate = permissions[0].Project.CreatedDate,
                    EndDate = permissions[0].Project.EndDate,
                    Id = project_id,
                    Name = permissions[0].Project.Name,
                    ProjectType = new ProjectTypeM
                    {
                        Id = permissions[0].Project.ProjectType.Id,
                        Name = permissions[0].Project.ProjectType.Name
                    },
                    StartDate = permissions[0].Project.StartDate,
                    Members = users
                };
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred while get project detail!",
                    e, DateTime.Now, "Server", "Service_Project_GetDetail");
            }
        }

        public ProjectTechnologyM GetTechnology(Guid project_id)
        {
            try
            {
                return _project.Where(p => p.Id.Equals(project_id))
                    .Select(p => new ProjectTechnologyM
                    {
                        Tools = p.ProjectTools.Select(pt => new ToolM {
                            Id = pt.Tool.Id,
                            Name = pt.Tool.Name,
                            ToolType = pt.Tool.ToolType.Split(new char[] { ',' })
                        }).ToList()
                    }).FirstOrDefault();
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred while get techology of a project!",
                    e, DateTime.Now, "Server", "Service_Project_GetTechnology");
            }
        }

        public void SetupTechnology(ProjectTechnologySetupM model, Guid project_id)
        {
            try
            {
                var repository = _repository.Where(r => r.Id.Equals(model.RepositoryId))
                    .Select(r => new {
                        r.HookId
                    }).FirstOrDefault();
                if (repository == null) throw NotFound(model.RepositoryId, "repository id");
                if (repository.HookId == null) throw BadRequest("Must follow repository first!");

                Project project = _project.GetOne(project_id);
                if (project == null) throw NotFound(project_id, "project id");

                if (model.ToolIds != null)
                {
                    foreach (var tool_id in model.ToolIds)
                    {
                        IList<Tool> tools = _tool.GetAll();
                        if (!tools.Any(t => t.Id.Equals(tool_id))) throw NotFound(tool_id, "tool id");
                    }
                }

                if (model.ToolIds != null)
                {
                    foreach (var tool_id in model.ToolIds)
                    {
                        _projectTool.Add(new ProjectTool
                        {
                            ProjectId = project_id,
                            ToolId = tool_id
                        });
                    }
                }

                SaveChanges();
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred while set up techology for project!",
                    e, DateTime.Now, "Server", "Service_Project_SetupTechnology");
            }
        }

        private int SaveChanges()
        {
            return _project.SaveChanges();
        }
    }
}
