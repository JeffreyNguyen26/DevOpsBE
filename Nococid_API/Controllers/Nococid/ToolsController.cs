using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Nococid_API.Attributes;
using Nococid_API.Models.Examples.ProjectTools;
using Nococid_API.Models.Nococid;
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
    public class ToolsController : NococidControllerBase
    {
        private readonly IToolService _tool;
        private readonly IPermissionService _permission;
        private readonly IProjectToolService _projectTool;

        public ToolsController(IPermissionService permission, IProjectToolService projectTool, IToolService tool, IErrorHandlerService errorHandler, IJwtAuthService jwtAuth) : base(errorHandler, jwtAuth)
        {
            _permission = permission;
            _projectTool = projectTool;
            _tool = tool;
        }

        [HttpGet("[controller]")]
        public IActionResult GetAll()
        {
            try
            {
                return Ok(_tool.GetAll());
            }
            catch (Exception e)
            {
                return GetError(e);
            }
        }

        [HttpGet("[controller]/{tool_id}")]
        public IActionResult GetDetail([FromRoute] Guid tool_id)
        {
            try
            {
                return Ok(_tool.Get(tool_id));
            }
            catch (Exception e)
            {
                return GetError(e);
            }
        }

        [HttpGet("[controller]/{tool_id}/config")]
        public IActionResult GetConfig([FromRoute] Guid tool_id)
        {
            try
            {
                return Ok(_tool.GetConfig(tool_id));
            }
            catch (Exception e)
            {
                return GetError(e);
            }
        }

        [HttpGet("Projects/{project_id}/[controller]")]
        public IActionResult GetProjectTools([FromRoute] Guid project_id)
        {
            try
            {
                JwtClaimM jwt_claim = _jwtAuth.GetClaims(Request);
                _permission.EnsureProjectMember(jwt_claim.UserId, project_id);
                return Ok(_projectTool.GetProjectTools(project_id));
            }
            catch (Exception e)
            {
                return GetError(e);
            }
        }

        [HttpPost("Projects/{project_id}/[controller]/{tool_id}")]
        public IActionResult AddProjectTool([FromRoute] Guid project_id, [FromRoute] Guid tool_id, [FromQuery] string stage)
        {
            try
            {
                JwtClaimM jwt_claim = _jwtAuth.GetClaims(Request);
                _permission.EnsureProjectTechnical(jwt_claim.UserId, project_id);
                return Ok(_projectTool.Add(project_id, tool_id, stage));
            }
            catch (Exception e)
            {
                return GetError(e);
            }
        }

        [HttpDelete("Projects/{project_id}/[controller]/{tool_id}")]
        public IActionResult DeleteProjectTool([FromRoute] Guid project_id, [FromRoute] Guid tool_id, [FromQuery] string stage)
        {
            try
            {
                JwtClaimM jwt_claim = _jwtAuth.GetClaims(Request);
                _permission.EnsureProjectTechnical(jwt_claim.UserId, project_id);
                return Ok(_projectTool.Delete(project_id, tool_id, stage));
            }
            catch (Exception e)
            {
                return GetError(e);
            }
        }
    }
}
