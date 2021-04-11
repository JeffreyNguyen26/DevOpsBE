using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nococid_API.Attributes;
using Nococid_API.Attributes.Filters;
using Nococid_API.Data.Static;
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
    public class AccountsController : NococidControllerBase
    {
        private readonly IAccountService _account;
        private readonly IPermissionService _permission;

        public AccountsController(IPermissionService permission, IAccountService account, IErrorHandlerService errorHandler, IJwtAuthService jwtAuth) : base(errorHandler, jwtAuth)
        {
            _permission = permission;
            _account = account;
        }

        [HttpGet("[controller]")]
        public IActionResult GetAll()
        {
            try
            {
                JwtClaimM jwt_claim = _jwtAuth.GetClaims(Request);
                return Ok(_account.GetAll(jwt_claim.UserId));
            }
            catch (Exception e)
            {
                return GetError(e);
            }
        }

        [HttpGet("Projects/{project_id}/Repositories/[controller]")]
        public IActionResult GetCollaAccounts([FromRoute] Guid project_id)
        {
            try
            {
                JwtClaimM jwt_claim = _jwtAuth.GetClaims(Request);
                _permission.EnsureProjectMember(jwt_claim.UserId, project_id);
                return Ok(_account.GetCollaAccounts(jwt_claim.UserId, project_id));
            }
            catch (Exception e)
            {
                return GetError(e);
            }
        }
    }
}
