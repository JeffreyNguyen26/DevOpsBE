using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Nococid_API.Attributes;
using Nococid_API.Attributes.Filters;
using Nococid_API.Data.Static;
using Nococid_API.Models.CircleCIs;
using Nococid_API.Models.Examples.Permissions;
using Nococid_API.Models.Examples.Projects;
using Nococid_API.Models.Nococid;
using Nococid_API.Models.Nococid.Auth;
using Nococid_API.Services;
using Nococid_API.Services.Github;
using Nococid_API.Services.Nococid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Nococid_API.Controllers.Nococid
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ProjectsController : NococidControllerBase
    {

        private readonly IProjectService _project;
        private readonly IAccountService _account;
        private readonly IPermissionService _permission;
        private readonly ICollaboratorService _collaborator;
        private readonly IProjectRepositoryService _projectRepository;
        private readonly IRepositoryService _repository;
        private readonly IGHCollaboratorService _gHCollaborator;
        private readonly IGHInvitationService _gHInvitation;
        private readonly IConfigurationFileService _configurationFile;

        public ProjectsController(IConfigurationFileService configurationFile, IGHInvitationService gHInvitation, IGHCollaboratorService gHCollaborator, IRepositoryService repository, IProjectRepositoryService projectRepository, ICollaboratorService collaborator, IPermissionService permission, IAccountService account, IProjectService project, IErrorHandlerService errorHandler, IJwtAuthService jwtAuth) : base(errorHandler, jwtAuth)
        {
            _configurationFile = configurationFile;
            _gHInvitation = gHInvitation;
            _gHCollaborator = gHCollaborator;
            _repository = repository;
            _projectRepository = projectRepository;
            _collaborator = collaborator;
            _permission = permission;
            _account = account;
            _project = project;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                JwtClaimM jwt_claim = _jwtAuth.GetClaims(Request);
                return Ok(_project.GetAll(jwt_claim.UserId));
            }
            catch (Exception e)
            {
                return GetError(e);
            }
        }

        [HttpGet("{project_id}")]
        public IActionResult GetDetail([FromRoute] Guid project_id)
        {
            try
            {
                JwtClaimM jwt_claim = _jwtAuth.GetClaims(Request);
                return Ok(_project.GetDetail(jwt_claim.UserId, project_id));
            }
            catch (Exception e)
            {
                return GetError(e);
            }
        }

        [HttpGet("{project_id}/technology")]
        public IActionResult GetTechnology([FromRoute] Guid account_id, [FromRoute] Guid project_id)
        {
            try
            {
                _permission.EnsureProjectTechnical(account_id, project_id);
                return Ok(_project.GetTechnology(project_id));
            }
            catch (Exception e)
            {
                return GetError(e);
            }
        }

        [HttpGet("{project_id}/config")]
        public IActionResult GetConfiguration([FromRoute] Guid project_id)
        {
            try
            {
                JwtClaimM jwt_claim = _jwtAuth.GetClaims(Request);
                _permission.EnsureProjectMember(jwt_claim.UserId, project_id);
                return Ok(_configurationFile.GetConfiguration(project_id));
            }
            catch (Exception e)
            {
                return GetError(e);
            }
        }

        [HttpPost("{project_id}/config")]
        public IActionResult SaveConfiguration([FromRoute] Guid project_id, [FromBody] string config_content)
        {
            try
            {
                JwtClaimM jwt_claim = _jwtAuth.GetClaims(Request);
                _permission.EnsureProjectMember(jwt_claim.UserId, project_id);
                _configurationFile.SaveConfiguration(project_id, config_content);
                return Ok();
            }
            catch (Exception e)
            {
                return GetError(e);
            }
        }

        [HttpPost("create")]
        [ExampleOperationProcessor(typeof(ProjectCreateMExample))]
        public IActionResult CreateProject([FromBody] ProjectCreateM model)
        {
            try
            {
                JwtClaimM jwt_claim = _jwtAuth.GetClaims(Request);
                return Created("", _project.Add(jwt_claim.AdminUserId, jwt_claim.UserId, model));
            }
            catch (Exception e)
            {
                return GetError(e);
            }
        }

        [HttpPost("{project_id}/assign")]
        [ExampleOperationProcessor(typeof(PermissionRoleSetupMExmaple))]
        public IActionResult Assign([FromRoute] Guid project_id, [FromBody] PermissionRoleSetupM model)
        {
            try
            {
                JwtClaimM jwt_claim = _jwtAuth.GetClaims(Request);
                if (model.RoleId.Equals(RoleID.Project_Manager))
                {
                    _permission.EnsureProjectOwner(jwt_claim.UserId, project_id);
                    _permission.AssignProjectManager(model.AssignedUserId, project_id);
                }
                else
                {
                    if (jwt_claim.AdminUserId.Equals(model.AssignedUserId)) return Forbid();
                    if (model.RoleId.Equals(RoleID.Admin) || model.RoleId.Equals(RoleID.Project_Manager)) return Forbid();
                    _permission.EnsureProjectManager(jwt_claim.UserId, project_id);
                    
                    IList<Guid> repository_ids = _projectRepository.GetRepositoryIds(project_id);
                    if (repository_ids.Count != 0)
                    {
                        if (repository_ids.Count == 2)
                        {
                            if (repository_ids[0].Equals(repository_ids[1])) repository_ids.RemoveAt(1);
                        }
                        foreach (var repository_id in repository_ids)
                        {
                            if (!_collaborator.HasCollab(model.AssignedUserId, repository_id))
                            {
                                Guid account_id = _account.GetMainAccountId(model.AssignedUserId);
                                if (!account_id.Equals(Guid.Empty))
                                {
                                    var assigned_user_account_requirement = _account.GetGHUserRequirement(account_id);
                                    var pm_repo_requirement = _repository.GetGHRepositoryRequirement(repository_id);

                                    int gh_invitation_id = _gHCollaborator.AddCollaborator(pm_repo_requirement.GHUser.Name, pm_repo_requirement.RepositoryName, pm_repo_requirement.GHUser.AccessToken, assigned_user_account_requirement.Name).Id;
                                    _gHInvitation.Accept(assigned_user_account_requirement.Name, assigned_user_account_requirement.AccessToken, gh_invitation_id);

                                    _collaborator.AddCollab(account_id, repository_id);
                                }
                            }
                        }
                    }
                    _permission.AssignProjectMember(model.AssignedUserId, project_id, model.RoleId);
                }
                return Ok("Assigned");
            }
            catch (Exception e)
            {
                return GetError(e);
            }
        }
    }
}
