using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nococid_API.Data.Static;
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
    public class RolesController : NococidControllerBase
    {
        private readonly IRoleService _role;
        private readonly IPermissionService _permission;

        public RolesController(IPermissionService permission, IRoleService role, IErrorHandlerService errorHandler, IJwtAuthService jwtAuth) : base(errorHandler, jwtAuth)
        {
            _permission = permission;
            _role = role;
        }

        [HttpGet("[controller]")]
        public IActionResult GetAll()
        {
            try
            {
                return Ok(_role.GetAll());
            }
            catch (Exception e)
            {
                return GetError(e);
            }
        }

        [HttpGet("Projects/{project_id}/[controller]")]
        public IActionResult GetProjectRoles([FromRoute] Guid project_id)
        {
            try
            {
                JwtClaimM jwt_claim = _jwtAuth.GetClaims(Request);
                _permission.EnsureProjectMember(jwt_claim.UserId, project_id);
                return Ok(_role.GetProjectRoles(project_id));
            }
            catch (Exception e)
            {
                return GetError(e);
            }
        }

        [HttpGet("Projects/{project_id}/[controller]/me")]
        public IActionResult GetMyRoles([FromRoute] Guid project_id)
        {
            try
            {
                JwtClaimM jwt_claim = _jwtAuth.GetClaims(Request);
                return Ok(_role.GetMyRoles(jwt_claim.UserId, project_id));
            }
            catch (Exception e)
            {
                return GetError(e);
            }
        }
    }
}
