using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nococid_API.Attributes;
using Nococid_API.Attributes.Filters;
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
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class UsersController : NococidControllerBase
    {
        private readonly IUserService _user;

        public UsersController(IUserService user, IErrorHandlerService errorHandler, IJwtAuthService jwtAuth) : base(errorHandler, jwtAuth)
        {
            _user = user;
        }

        [HttpGet("org-members")]
        public IActionResult GetMembers()
        {
            try
            {
                JwtClaimM jwt_claim = _jwtAuth.GetClaims(Request);
                return Ok(_user.GetMembers(jwt_claim.AdminUserId, jwt_claim.UserId));
            }
            catch (Exception e)
            {
                return GetError(e);
            }
        }

        [HttpPost("create-member")]
        public IActionResult CreateMember([FromBody] UserCreateM model)
        {
            try
            {
                JwtClaimM jwt_claim = _jwtAuth.GetClaims(Request);
                if (!jwt_claim.AdminUserId.Equals(Guid.Empty))
                {
                    return new ForbidResult();
                }
                return Created("", _user.Register(model, jwt_claim.UserId));
            }
            catch (Exception e)
            {
                return GetError(e);
            }
        }
    }
}