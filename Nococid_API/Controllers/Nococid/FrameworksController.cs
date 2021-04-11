using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nococid_API.Attributes;
using Nococid_API.Models.Examples.ProjectFrameworks;
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
    public class FrameworksController : NococidControllerBase
    {
        private readonly IFrameworkService _framework;
        private readonly ILanguageService _language;
        private readonly IPermissionService _permission;
        private readonly IProjectFrameworkService _projectFramework;

        public FrameworksController(IProjectFrameworkService projectFramework, IPermissionService permission, ILanguageService language, IFrameworkService framework, IErrorHandlerService errorHandler, IJwtAuthService jwtAuth) : base(errorHandler, jwtAuth)
        {
            _projectFramework = projectFramework;
            _permission = permission;
            _language = language;
            _framework = framework;
        }

        [HttpGet("Languages/{language_id}/[controller]")]
        public IActionResult GetLanguageFrameworks([FromRoute] Guid language_id)
        {
            try
            {
                return Ok(_language.GetDetail(language_id));
            }
            catch (Exception e)
            {
                return GetError(e);
            }
        }

        [HttpGet("Projects/{project_id}/[controller]")]
        public IActionResult GetProjectFramework([FromRoute] Guid project_id)
        {
            try
            {
                JwtClaimM jwt_claim = _jwtAuth.GetClaims(Request);
                _permission.EnsureProjectTechnical(jwt_claim.UserId, project_id);
                return Ok(_projectFramework.GetDetail(project_id));
            }
            catch (Exception e)
            {
                return GetError(e);
            }
        }

        [HttpPost("Projects/{project_id}/[controller]/set-framework")]
        [ExampleOperationProcessor(typeof(ProjectFrameworkAddMExample))]
        public IActionResult SetupProjectFramework([FromRoute] Guid project_id, [FromBody] IList<Guid> framework_ids)
        {
            try
            {
                JwtClaimM jwt_claim = _jwtAuth.GetClaims(Request);
                _permission.EnsureProjectTechnical(jwt_claim.UserId, project_id);
                return Created("", _projectFramework.AddMany(project_id, framework_ids));
            }
            catch (Exception e)
            {
                return GetError(e);
            }
        }

        [HttpDelete("Projects/{project_id}/[controller]/{framework_id}/delete")]
        public IActionResult DeleteProjectFramework([FromRoute] Guid project_id, [FromRoute] Guid framework_id)
        {
            try
            {
                JwtClaimM jwt_claim = _jwtAuth.GetClaims(Request);
                _permission.EnsureProjectTechnical(jwt_claim.UserId, project_id);
                _projectFramework.Delete(project_id, framework_id);
                return Ok();
            }
            catch (Exception e)
            {
                return GetError(e);
            }
        }
    }
}
