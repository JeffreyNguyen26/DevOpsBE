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
    public interface IProjectRepositoryService
    {
        IList<ProjectRepositoryM> GetProjectRepositories(Guid project_id);
        IList<ProjectRepositoryM> SetProjectRepository(Guid user_id, Guid project_id, ProjectRepositorySetupM model);
        void EnsureExist(Guid project_id, Guid repository_id);
        void Delete(Guid project_id, Guid repository_id, string side);
        IList<Guid> GetRepositoryIds(Guid project_id);
    }

    public class ProjectRepositoryService : ServiceBase, IProjectRepositoryService
    {
        private readonly IContext<Repository> _repository;
        private readonly IContext<Collaborator> _collaborator;
        private readonly IContext<Project> _project;
        private readonly IContext<ProjectRepository> _projectRepository;

        public ProjectRepositoryService(IContext<Project> project, IContext<Collaborator> collaborator, IContext<Repository> repository, IContext<ProjectRepository> projectRepository, IErrorHandlerService errorHandler) : base(errorHandler)
        {
            _project = project;
            _collaborator = collaborator;
            _repository = repository;
            _projectRepository = projectRepository;
        }

        public void Delete(Guid project_id, Guid repository_id, string side)
        {
            try
            {
                if (!("Server".Equals(side) || "Client".Equals(side) || "Database".Equals(side)))
                    throw BadRequest("Side value must be 'Server' or 'Client'!");

                ProjectRepository project_repository = _projectRepository.GetOne(pr => pr.RepositoryId.Equals(repository_id) && pr.ProjectId.Equals(project_id) && pr.Side.Equals(side));
                if (project_repository == null) throw NotFound();
                _projectRepository.Remove(project_repository);
                SaveChanges();
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred while delete a repo setup!",
                    e, DateTime.Now, "Server", "Service_ProjectRepository_Delete");
            }
        }

        public void EnsureExist(Guid project_id, Guid repository_id)
        {
            try
            {
                if (!_projectRepository.Any(pr => pr.ProjectId.Equals(project_id) && pr.RepositoryId.Equals(repository_id))) throw NotFound();
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred while ensure project repository existed!",
                    e, DateTime.Now, "Server", "Service_ProjectRepository_EnsureExist");
            }
        }

        public IList<ProjectRepositoryM> GetProjectRepositories(Guid project_id)
        {
            try
            {
                return _projectRepository.Where(pr => pr.ProjectId.Equals(project_id))
                    .Select(pr => new ProjectRepositoryM
                    {
                        Side = pr.Side,
                        Repository = new RepositoryM
                        {
                            Id = pr.Repository.Id,
                            IsFollow = pr.Repository.HookId != null,
                            Languages = pr.Repository.Languages,
                            Name = pr.Repository.Name
                        }
                    }).ToList();
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred while get project repositories!",
                    e, DateTime.Now, "Server", "Service_ProjectRepository_GetProjectRepositories");
            }
        }

        public IList<Guid> GetRepositoryIds(Guid project_id)
        {
            try
            {
                return _projectRepository.Where(pr => pr.ProjectId.Equals(project_id))
                    .Select(pr => pr.RepositoryId.Value).ToList();
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred while get project repositories!",
                    e, DateTime.Now, "Server", "Service_ProjectRepository_GetProjectRepositories");
            }
        }

        public IList<ProjectRepositoryM> SetProjectRepository(Guid user_id, Guid project_id, ProjectRepositorySetupM model)
        {
            try
            {
                if (!("Server".Equals(model.Side) || "Client".Equals(model.Side) || "Database".Equals(model.Side)))
                    throw BadRequest("Side value must be 'Server' or 'Client'!");

                if (!_collaborator.Any(c => c.Account.UserId.Equals(user_id) && c.RepositoryId.Equals(model.RepositoryId) && c.AccountId.Equals(c.OwnerId))) throw NotFound(model.RepositoryId, "repository id");
                
                ProjectRepository project_repository = _projectRepository.GetOne(pr => pr.ProjectId.Equals(project_id) && pr.Side.Equals(model.Side));
                var project = _projectRepository.Where(pr => pr.RepositoryId.Equals(model.RepositoryId) && pr.Side.Equals(model.Side))
                    .Select(pr => new
                    {
                        pr.Project.Id,
                        pr.Project.Name
                    }).FirstOrDefault();
                if (project_repository == null)
                {
                    if (project != null) throw BadRequest("This repository has been set to project " + project.Name + "!");
                    else
                    {
                        _projectRepository.Add(new ProjectRepository
                        {
                            ProjectId = project_id,
                            RepositoryId = model.RepositoryId,
                            Side = model.Side
                        });
                        SaveChanges();
                    }
                } else
                {
                    if (project != null)
                    {
                        if (!project.Id.Equals(project_id))
                            throw BadRequest("This repository has been set to project " + project.Name + "!");
                    }
                    if (!project_repository.RepositoryId.Equals(model.RepositoryId))
                    {
                        _projectRepository.Remove(project_repository);
                        _projectRepository.Add(new ProjectRepository
                        {
                            ProjectId = project_id,
                            RepositoryId = model.RepositoryId,
                            Side = model.Side
                        });
                        SaveChanges();
                    }
                }

                return GetProjectRepositories(project_id);
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred while set project repository!",
                    e, DateTime.Now, "Server", "Service_ProjectRepository_SetProjectRepository");
            }
        }

        private int SaveChanges()
        {
            return _projectRepository.SaveChanges();
        }
    }
}
