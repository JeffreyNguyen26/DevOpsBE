using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nococid_API.Attributes;
using Nococid_API.Attributes.Filters;
using Nococid_API.Data.Static;
using Nococid_API.Enums;
using Nococid_API.Models.Examples.Workflows;
using Nococid_API.Models.Github;
using Nococid_API.Models.Nococid;
using Nococid_API.Models.Nococid.Auth;
using Nococid_API.Models.Nococid.Errors;
using Nococid_API.Services;
using Nococid_API.Services.CircleCI;
using Nococid_API.Services.Github;
using Nococid_API.Services.Nococid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Nococid_API.Controllers.Nococid
{
    [Route("api")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class WorkflowsController : NococidControllerBase
    {
        private readonly IAccountService _account;
        private readonly ISprintService _sprint;
        private readonly IPermissionService _permission;
        private readonly IWorkflowService _workflow;
        private readonly ITokenService _token;
        private readonly IRepositoryService _repository;
        private readonly IGHBranchService _gHBranch;
        private readonly IGHContentService _gHContent;
        private readonly ICircleCIProjectService _circleCIProject;
        private readonly IBranchService _branch;
        private readonly IProjectRepositoryService _projectRepository;
        private readonly ICircleCIConfigurationService _circleCIConfiguration;

        public WorkflowsController(ICircleCIConfigurationService circleCIConfiguration, IProjectRepositoryService projectRepository, IBranchService branch, IAccountService account, ITokenService token, ICircleCIProjectService circleCIProject, IGHContentService gHContent, ISprintService sprint, IPermissionService permission, IWorkflowService workflow, IRepositoryService repository, IGHBranchService gHBranch, IErrorHandlerService errorHandler, IJwtAuthService jwtAuth) : base(errorHandler, jwtAuth)
        {
            _circleCIConfiguration = circleCIConfiguration;
            _projectRepository = projectRepository;
            _branch = branch;
            _account = account;
            _token = token;
            _circleCIProject = circleCIProject;
            _gHContent = gHContent;
            _sprint = sprint;
            _permission = permission;
            _workflow = workflow;
            _repository = repository;
            _gHBranch = gHBranch;
        }

        [HttpGet("Accounts/{account_id}/Projects/{project_id}/Sprints/{sprint_id}/[controller]")]
        public IActionResult GetAll([FromRoute] Guid account_id, [FromRoute] Guid project_id, [FromRoute] Guid sprint_id)
        {
            try
            {
                _permission.EnsureProjectMember(account_id, project_id);
                _sprint.EnsureExisted(project_id, sprint_id);
                return Ok(_workflow.GetAll(sprint_id));
            }
            catch (Exception e)
            {
                return GetError(e);
            }
        }

        [HttpGet("Accounts/{account_id}/Projects/{project_id}/Sprints/{sprint_id}/[controller]/{workflow_id}")]
        public IActionResult Get([FromRoute] Guid account_id, [FromRoute] Guid project_id, [FromRoute] Guid sprint_id, [FromRoute] Guid workflow_id)
        {
            try
            {
                _permission.EnsureProjectMember(account_id, project_id);
                _workflow.EnsureExisted(project_id, sprint_id, workflow_id);
                return Ok(_workflow.GetDetail(workflow_id));
            }
            catch (Exception e)
            {
                return GetError(e);
            }
        }

        [HttpGet("[controller]/Stages")]
        public IActionResult GetWorkStages()
        {
            try
            {
                return Ok(_workflow.GetWorkStages());
            }
            catch (Exception e)
            {
                return GetError(e);
            }
        }

        [HttpPost("Projects/{project_id}/Sprint/{sprint_id}/[controller]")]
        [ExampleOperationProcessor(typeof(WorkflowCreateMExample))]
        public IActionResult Create([FromRoute] Guid project_id, [FromRoute] Guid sprint_id, [FromBody] WorkflowCreateM model)
        {
            try
            {
                JwtClaimM jwt_claim = _jwtAuth.GetClaims(Request);
                _permission.EnsureProjectMember(jwt_claim.UserId, project_id);
                _sprint.EnsureExisted(project_id, sprint_id);
                _projectRepository.EnsureExist(project_id, model.RepositoryId);
                _account.EnsureOwner(jwt_claim.UserId, model.AccountId);

                IList<BranchM> branches = _branch.GetAll(model.RepositoryId, model.BranchIds);
                string path = _sprint.GetConfigurationFilePath(sprint_id);
                GHRepositoryRequirement gh_repo = _repository.GetGHRepositoryRequirement(model.AccountId, model.RepositoryId);
                
                foreach (var config in model.Configs)
                {
                    if (config.ConfigTool.Equals(ConfigTool.CircleCI))
                    {
                        config.Content = _circleCIConfiguration.CreateConfigurationContent(config.Content);
                    }
                }

                _gHContent.CreateConfigurationFiles(gh_repo.GHUser.Name, gh_repo.RepositoryName, gh_repo.GHUser.AccessToken,
                    branches, path, model.Configs, sprint_id);

                return Ok(model.Configs);
            }
            catch (Exception e)
            {
                return GetError(e);
            }
        }

        [HttpPost("Accounts/{account_id}/Projects/{project_id}/Sprints/{sprint_id}/[controller]/{workflow_id}/run")]
        public IActionResult Run([FromRoute] Guid account_id, [FromRoute] Guid project_id, [FromRoute] Guid sprint_id, [FromRoute] Guid workflow_id)
        {
            try
            {
                //_permission.EnsureProjectMember(account_id, project_id);

                //string path = ".nococid/" + _workflow.GetPath(workflow_id);
                //GHRepositoryRequirement requirement = _repository.GetGHRepositoryRequirement(project_id);
                //_gHContent.RunWorkflow(requirement.GHUser.Name, requirement.RepositoryName, requirement.GHUser.AccessToken, path);

                return Ok("Running");
            }
            catch (Exception e)
            {
                return GetError(e);
            }
        }
    }
}