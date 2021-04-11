using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nococid_API.Attributes;
using Nococid_API.Attributes.Filters;
using Nococid_API.Models.Nococid.Auth;
using Nococid_API.Services;
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
    public class BranchesController : NococidControllerBase
    {
        private readonly IBranchService _branch;
        private readonly IPermissionService _permission;
        private readonly IProjectRepositoryService _projectRepository;

        public BranchesController(IProjectRepositoryService projectRepository, IPermissionService permission, IBranchService branch, IErrorHandlerService errorHandler, IJwtAuthService jwtAuth) : base(errorHandler, jwtAuth)
        {
            _projectRepository = projectRepository;
            _permission = permission;
            _branch = branch;
        }

        [HttpGet("Projects/{project_id}/Repositories/{repository_id}/[controller]")]
        public IActionResult GetProjectBranches([FromRoute] Guid project_id, [FromRoute] Guid repository_id)
        {
            try
            {
                JwtClaimM jwt_claim = _jwtAuth.GetClaims(Request);
                _permission.EnsureProjectMember(jwt_claim.UserId, project_id);
                _projectRepository.EnsureExist(project_id, repository_id);
                return Ok(_branch.GetAll(repository_id));
            }
            catch (Exception e)
            {
                return GetError(e);
            }
        }
    }
}
