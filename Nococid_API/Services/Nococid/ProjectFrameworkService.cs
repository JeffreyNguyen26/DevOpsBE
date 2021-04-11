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
    public interface IProjectFrameworkService
    {
        ProjectFrameworkM AddMany(Guid project_id, IList<Guid> framework_ids);
        ProjectFrameworkM GetDetail(Guid project_id);
        void Delete(Guid project_id, Guid framework_id);
    }

    public class ProjectFrameworkService : ServiceBase, IProjectFrameworkService
    {
        private readonly IContext<Project> _project;
        private readonly IContext<Framework> _framework;
        private readonly IContext<ProjectFramework> _projectFramework;
        private readonly IContext<Permission> _permission;
        private readonly IContext<Language> _language;
        private readonly IContext<Data.Models.Task> _task;

        public ProjectFrameworkService(IContext<Data.Models.Task> task, IContext<Permission> permission, IContext<Language> language, IContext<Framework> framework, IContext<Project> project, IContext<ProjectFramework> projectFramework, IErrorHandlerService errorHandler) : base(errorHandler)
        {
            _task = task;
            _permission = permission;
            _language = language;
            _framework = framework;
            _project = project;
            _projectFramework = projectFramework;
        }

        public ProjectFrameworkM AddMany(Guid project_id, IList<Guid> framework_ids)
        {
            try
            {
                foreach (var framework_id in framework_ids)
                {
                    if (!_framework.Any(f => f.Id.Equals(framework_id))) throw NotFound(framework_id, "framework id");
                }
                foreach (var framework_id in framework_ids)
                {
                    if (!_projectFramework.Any(pf => pf.ProjectId.Equals(project_id) && pf.FrameworkId.Equals(framework_id)))
                    {
                        _projectFramework.Add(new ProjectFramework
                        {
                            ProjectId = project_id,
                            FrameworkId = framework_id
                        });
                    }
                }
                SaveChanges();

                return GetDetail(project_id);
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred while add project frameworks!",
                    e, DateTime.Now, "Server", "Service_ProjectFramework_AddMany");
            }
        }

        public void Delete(Guid project_id, Guid framework_id)
        {
            try
            {
                if (!_framework.Any(f => f.Id.Equals(framework_id))) throw NotFound(framework_id, "framework id");
                ProjectFramework project_framework = _projectFramework.GetOne(pf => pf.FrameworkId.Equals(framework_id) && pf.ProjectId.Equals(project_id));
                if (project_framework == null) throw BadRequest("Do not have this framework refer to the project!");
                
                _projectFramework.Remove(project_framework);
                SaveChanges();
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred while delete a project framework!",
                    e, DateTime.Now, "Server", "Service_ProjectFramework_Delete");
            }
        }

        public ProjectFrameworkM GetDetail(Guid project_id)
        {
            try
            {
                var result = _project.Where(p => p.Id.Equals(project_id))
                    .Select(p => new
                    {
                        Project = new ProjectFrameworkM
                        {
                            Id = p.Id,
                            CreatedDate = p.CreatedDate,
                            EndDate = p.EndDate,
                            Name = p.Name,
                            StartDate = p.StartDate,
                            ProjectType = new ProjectTypeM
                            {
                                Id = p.ProjectType.Id,
                                Name = p.ProjectType.Name
                            },
                            Owner = p.Permissions.Where(permission => permission.RoleId.Equals(RoleID.Admin) && permission.ProjectId.Equals(project_id)).Select(permission => new UserM
                            {
                                Id = permission.User.Id,
                                Username = permission.User.Username
                            }).FirstOrDefault(),
                        },
                        Frameworks = p.ProjectFrameworks.Select(f => new
                        {
                            f.Framework.Id,
                            f.Framework.Name,
                            f.Framework.Language
                        }).ToList()
                    }).FirstOrDefault();

                result.Project.Languages = result.Frameworks.GroupBy(f => f.Language).Select(l => new LanguageDM
                {
                    Id = l.Key.Id,
                    Name = l.Key.Name,
                    Side = l.Key.Side,
                    Frameworks = l.Select(f => new FrameworkM
                    {
                        Id = f.Id,
                        Name = f.Name
                    }).ToList()
                }).ToList();
                return result.Project;
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred while get project framework detail!",
                    e, DateTime.Now, "Server", "Service_ProjectFramework_GetDetail");
            }
        }

        private int SaveChanges()
        {
            return _projectFramework.SaveChanges();
        }
    }
}
