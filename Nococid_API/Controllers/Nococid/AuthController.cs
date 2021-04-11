using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nococid_API.Attributes;
using Nococid_API.Data.Static;
using Nococid_API.Models.Examples.Users;
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
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : NococidControllerBase
    {
        private readonly IUserService _user;
        private readonly IGHAuthService _gHAuth;
        private readonly IGHUserService _gHUser;
        private readonly IAccountService _account;

        public AuthController(IUserService user, IGHAuthService gHAuth, IGHUserService gHUser, IAccountService account, IErrorHandlerService errorHandler, IJwtAuthService jwtAuth) : base(errorHandler, jwtAuth)
        {
            _user = user;
            _gHAuth = gHAuth;
            _gHUser = gHUser;
            _account = account;
        }

        [HttpGet("login/github")]
        public IActionResult LoginGithub([FromQuery] string error, [FromQuery] string state, [FromQuery] string redirect_uri)
        {
            try
            {
                //if (!string.IsNullOrEmpty(error))
                //{
                //    if (!string.IsNullOrEmpty(state))
                //    {
                //        _gHAuth.RemoveState(state);
                //    }
                //    return Redirect(redirect_uri);
                //}
                //if (string.IsNullOrEmpty(redirect_uri))
                //{
                //    return Ok(_gHAuth.GetRedirectUri(""));
                //}
                //return Redirect(_gHAuth.GetRedirectUri(redirect_uri));;
                return Ok();
            }
            catch (Exception e)
            {
                return GetError(e);
            }
        }

        [HttpGet("login/github/code")]
        public IActionResult GithubCode([FromQuery] string code, [FromQuery] string state)
        {
            try
            {
                _gHAuth.HasState(state);
                string uri = _gHAuth.GetAccessTokenUri(code, state, out KeyValuePair<string, Guid[]> redirect_uri_user_id_pair);
                _gHAuth.RemoveState(state);
                GHAuthenticatedUserInfo user_info = _gHUser.GetAuthenticatedUserInfo(uri);
                Guid account_id = _account.Add(user_info, VSCID.Github, out bool is_connected);
                string user = "", jwt = "";

                string error = _account.Connect(redirect_uri_user_id_pair.Value[1], account_id);
                if (error != null)
                {
                    if (string.IsNullOrEmpty(redirect_uri_user_id_pair.Key))
                    {
                        return BadRequest(error);
                    }
                    return Redirect(redirect_uri_user_id_pair.Key + (redirect_uri_user_id_pair.Key.Contains("?") ? "&" : "?") + "error=" + error);
                }

                user += "nococid";
                jwt += _jwtAuth.GenerateJwt(redirect_uri_user_id_pair.Value[0], redirect_uri_user_id_pair.Value[1], ApplicationRole.Web_User);
                if (string.IsNullOrEmpty(redirect_uri_user_id_pair.Key))
                {
                    return Ok(jwt);
                }
                return Redirect(redirect_uri_user_id_pair.Key + "?user=" + user + "&jwt=" + jwt);
            }
            catch (Exception e)
            {
                return GetError(e);
            }
        }

        [HttpPost("login")]
        [ExampleOperationProcessor(typeof(UserLoginMExample), typeof(UserLoginMExamples))]
        public IActionResult Login([FromQuery] string redirect_uri, [FromBody] UserLoginM model)
        {
            try
            {
                string role = ApplicationRole.Web_User;
                UserAuthorizationM result = _user.Login(model);
                if (model.Username.Equals(ApplicationAuth.Nococid_Application_Admin))
                {
                    role = ApplicationRole.Application_Admin;
                }
                result.Jwt = _jwtAuth.GenerateJwt(result.AdminUser == null ? Guid.Empty : result.AdminUser.Id, result.User.Id, role);
                if (string.IsNullOrEmpty(redirect_uri))
                {
                    return Ok(result);
                }
                return Redirect(redirect_uri + "?user=nococid&jwt=" + result.Jwt);
            }
            catch (Exception e)
            {
                return GetError(e);
            }
        }

        [HttpPost("logout")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult Logout()
        {
            try
            {
                JwtClaimM jwt_claim = _jwtAuth.GetClaims(Request);
                _jwtAuth.RemoveAudience(jwt_claim.AdminUserId, jwt_claim.UserId);
                return Ok("Logout success");
            }
            catch (Exception e)
            {
                return GetError(e);
            }
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] UserCreateM model)
        {
            try
            {
                var result = _user.Register(model, null);
                result.Jwt = _jwtAuth.GenerateJwt(Guid.Empty, result.User.Id, ApplicationRole.Web_User);
                return Created("", result);
            }
            catch (Exception e)
            {
                return GetError(e);
            }
        }

        [HttpPost("connect-third-party-account")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult ConnectAccount([FromQuery] string redirect_uri)
        {
            try
            {
                JwtClaimM jwt_claim = _jwtAuth.GetClaims(Request);
                return Ok(_gHAuth.GetRedirectUri(redirect_uri, jwt_claim.AdminUserId, jwt_claim.UserId));
            }
            catch (Exception e)
            {
                return GetError(e);
            }
        }
    }
}
