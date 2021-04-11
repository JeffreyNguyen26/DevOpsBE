using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nococid_API.Attributes;
using Nococid_API.Attributes.Filters;
using Nococid_API.Data.Static;
using Nococid_API.Models.Examples.Tokens;
using Nococid_API.Models.Nococid;
using Nococid_API.Models.Nococid.Auth;
using Nococid_API.Services;
using Nococid_API.Services.CircleCI;
using Nococid_API.Services.Heroku;
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
    public class TokensController : NococidControllerBase
    {
        private readonly ITokenService _token;
        private readonly IAccountService _account;
        private readonly IPermissionService _permission;
        private readonly IConfigurationFileService _configurationFile;
        private readonly IProjectToolService _projectTool;
        private readonly ICircleCIUserService _circleCIUser;
        private readonly IHerokuAccountService _herokuAccount;
        private readonly IHerokuAppService _herokuApp;
        private readonly IHerokuRegionService _herokuRegion;
        private readonly IHerokuStackService _herokuStack;

        public TokensController(ITokenService token, IAccountService account, IPermissionService permission, IConfigurationFileService configurationFile, IProjectToolService projectTool, ICircleCIUserService circleCIUser, IHerokuAccountService herokuAccount, IHerokuAppService herokuApp, IHerokuRegionService herokuRegion, IHerokuStackService herokuStack, IErrorHandlerService errorHandler, IJwtAuthService jwtAuth) : base(errorHandler, jwtAuth)
        {
            _token = token;
            _account = account;
            _permission = permission;
            _configurationFile = configurationFile;
            _projectTool = projectTool;
            _circleCIUser = circleCIUser;
            _herokuAccount = herokuAccount;
            _herokuApp = herokuApp;
            _herokuRegion = herokuRegion;
            _herokuStack = herokuStack;
        }

        [HttpGet]
        public IActionResult Get([FromQuery] Guid? project_id = null)
        {
            try
            {
                JwtClaimM jwt_claim = _jwtAuth.GetClaims(Request);
                if (project_id != null)
                {
                    if (!project_id.Equals(Guid.Empty))
                    {
                        _permission.EnsureProjectManager(jwt_claim.UserId, project_id.Value);
                        return Ok(_token.Get(jwt_claim.UserId, project_id.Value));
                    }
                }

                return Ok();
            }
            catch (Exception e)
            {
                return GetError(e);
            }
        }

        [HttpPost]
        [ExampleOperationProcessor(typeof(TokenCreateMExample))]
        public IActionResult Add([FromBody] TokenCreateM model)
        {
            try
            {
                JwtClaimM jwt_claim = _jwtAuth.GetClaims(Request);
                if (model.ToolId.Equals(ToolID.CircleCI))
                {
                    var circle_user = _circleCIUser.Get(model.Token);
                    return Ok(_token.Add(circle_user.Id, null, circle_user.Login, null, model.Token, model.ToolId, jwt_claim.UserId));
                } else if (model.ToolId.Equals(ToolID.Heroku))
                {
                    var heroku_account = _herokuAccount.Get(model.Token);
                    return Ok(_token.Add(heroku_account.Id, heroku_account.Email, heroku_account.Name, null, model.Token, ToolID.Heroku, jwt_claim.UserId));
                }
                return NotFound();
            }
            catch (Exception e)
            {
                return GetError(e);
            }
        }

        [HttpPut]
        [ExampleOperationProcessor(typeof(TokenProjectAccountMExample))]
        public IActionResult SetProjectAccount([FromBody] TokenProjectAccountM model)
        {
            try
            {
                JwtClaimM jwt_claim = _jwtAuth.GetClaims(Request);
                _permission.EnsureProjectManager(jwt_claim.UserId, model.ProjectId);
                _account.EnsureOwner(jwt_claim.UserId, model.AccountId);
                _projectTool.SetAccount(model);
                return Ok();
            }
            catch (Exception e)
            {
                return GetError(e);
            }
        }
    }
}