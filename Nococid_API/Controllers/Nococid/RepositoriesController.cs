using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nococid_API.Attributes;
using Nococid_API.Attributes.Filters;
using Nococid_API.Data.Static;
using Nococid_API.Models.Examples.ProjectRepositories;
using Nococid_API.Models.Github;
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
    [Route("api")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class RepositoriesController : NococidControllerBase
    {
        private readonly IGHRepositoryService _gHRepository;
        private readonly IGHReferenceService _gHReference;
        private readonly IGHWebhookService _gHWebhook;
        private readonly IGHBranchService _gHBranch;
        private readonly IRepositoryService _repository;
        private readonly IPermissionService _permission;
        private readonly IAccountService _account;
        private readonly IBranchService _branch;
        private readonly IProjectService _project;
        private readonly IProjectRepositoryService _projectRepository;

        public RepositoriesController(IProjectRepositoryService projectRepository, IProjectService project, IPermissionService permission, IBranchService branch, IGHReferenceService gHReference, IGHWebhookService gHWebhook, IGHBranchService gHBranch, IGHRepositoryService gHRepository, IRepositoryService repository, IAccountService account, IErrorHandlerService errorHandler, IJwtAuthService jwtAuth) : base(errorHandler, jwtAuth)
        {
            _projectRepository = projectRepository;
            _project = project;
            _permission = permission;
            _branch = branch;
            _gHReference = gHReference;
            _gHWebhook = gHWebhook;
            _gHBranch = gHBranch;
            _gHRepository = gHRepository;
            _repository = repository;
            _account = account;
        }

        [HttpGet("[controller]")]
        public IActionResult GetRepositories()
        {
            try
            {
                JwtClaimM jwt_claim = _jwtAuth.GetClaims(Request);
                return Ok(_repository.GetForSetup(jwt_claim.UserId));
            }
            catch (Exception e)
            {
                return GetError(e);
            }
        }

        [HttpGet("Projects/{project_id}/[controller]")]
        public IActionResult GetProjectRepositories([FromRoute] Guid project_id)
        {
            try
            {
                JwtClaimM jwt_claim = _jwtAuth.GetClaims(Request);
                _permission.EnsureProjectMember(jwt_claim.UserId, project_id);
                return Ok(_projectRepository.GetProjectRepositories(project_id));
            }
            catch (Exception e)
            {
                return GetError(e);
            }
        }

        [HttpPost("Projects/{project_id}/setup-repo")]
        [ExampleOperationProcessor(typeof(ProjectRepositorySetupMExample))]
        public IActionResult SetProjectRepository([FromRoute] Guid project_id, [FromBody] ProjectRepositorySetupM model)
        {
            try
            {
                JwtClaimM jwt_claim = _jwtAuth.GetClaims(Request);
                _permission.EnsureProjectManager(jwt_claim.UserId, project_id);
                return Created("", _projectRepository.SetProjectRepository(jwt_claim.UserId, project_id, model));
            }
            catch (Exception e)
            {
                return GetError(e);
            }
        }

        [HttpPost("Accounts/{account_id}/[controller]/update-from-github")]
        public IActionResult UpdateFromGH([FromRoute] Guid account_id)
        {
            try
            {
                JwtClaimM jwt_claim = _jwtAuth.GetClaims(Request);
                GHUserRequirement gh_user = _account.GetGHUserRequirement(jwt_claim.UserId, account_id);

                IList<RepositoryM> result = new List<RepositoryM>();
                IList<GHRepository> gh_repositories;
                int page = 1;
                do
                {
                    gh_repositories = _gHRepository.Get(gh_user.Name, gh_user.AccessToken, page++);
                    foreach (var gh_repository in gh_repositories)
                    {
                        gh_repository.Languages = "";
                        if (gh_repository.Owner.Login.Equals(gh_user.Name))
                        {
                            var languages = _gHRepository.GetLanguages(gh_user.Name, gh_repository.Name, gh_user.AccessToken).Keys;
                            if (languages.Count != 0)
                            {
                                foreach (var language in languages)
                                {
                                    gh_repository.Languages += "," + language;
                                }
                                gh_repository.Languages = gh_repository.Languages.Substring(1);
                            }
                        }
                    }
                    IList<RepositoryM> repositories = _repository.AddManyGH(account_id, gh_repositories);
                    foreach (var repository in repositories)
                    {
                        result.Add(repository);
                    }
                } while (gh_repositories.Count >= 20);

                return Ok(result);
            }
            catch (Exception e)
            {
                return GetError(e);
            }
        }

        [HttpPut("Accounts/{account_id}/[controller]/{repository_id}/hook")]
        public IActionResult Follow([FromRoute] Guid account_id, [FromRoute] Guid repository_id)
        {
            try
            {
                JwtClaimM jwt_claim = _jwtAuth.GetClaims(Request);
                _repository.EnsureExisted(jwt_claim.UserId, account_id, repository_id);
                if (_repository.HasHook(repository_id)) return Ok();

                dynamic token = _repository.GetToken(repository_id);
                int hook_id = _gHWebhook.Create(token.Userame, token.RepositoryName, token.AccessToken);
                _repository.SetHook(repository_id, hook_id.ToString());

                string default_branch_name = _gHRepository.Get(token.Userame, token.RepositoryName, token.AccessToken).Default_branch;
                IList<GHBranch> branches;
                int page = 1;

                do
                {
                    branches = _gHBranch.GetBranches(token.Userame, token.RepositoryName, token.AccessToken, page++);
                    _branch.AddMany(branches, repository_id, default_branch_name);
                } while (branches.Count >= 100);

                return Ok();
            }
            catch (Exception e)
            {
                return GetError(e);
            }
        }

        [HttpDelete("Projects/{project_id}/[controller]/{repository_id}")]
        public IActionResult DeleteProjectRepo([FromRoute] Guid project_id, [FromRoute] Guid repository_id, [FromQuery] string side)
        {
            try
            {
                JwtClaimM jwt_claim = _jwtAuth.GetClaims(Request);
                _permission.EnsureProjectManager(jwt_claim.UserId, project_id);
                _projectRepository.Delete(project_id, repository_id, side);
                return Ok();
            }
            catch (Exception e)
            {
                return GetError(e);
            }
        }
    }
}