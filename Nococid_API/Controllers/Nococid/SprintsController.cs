using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nococid_API.Attributes;
using Nococid_API.Attributes.Filters;
using Nococid_API.Data.Static;
using Nococid_API.Models.Examples.Sprints;
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
    public class SprintsController : NococidControllerBase
    {
        private readonly IGHReferenceService _gHReference;
        private readonly ISprintService _sprint;
        private readonly IPermissionService _permission;
        private readonly IRepositoryService _repository;
        private readonly IGHWebhookService _gHWebhook;
        private readonly IAccountService _account;

        public SprintsController(IGHReferenceService gHReference, IAccountService account, IGHWebhookService gHWebhook, ISprintService sprint, IPermissionService permission, IRepositoryService repository, IErrorHandlerService errorHandler, IJwtAuthService jwtAuth) : base(errorHandler, jwtAuth)
        {
            _gHReference = gHReference;
            _account = account;
            _gHWebhook = gHWebhook;
            _sprint = sprint;
            _permission = permission;
            _repository = repository;
        }

        [HttpGet("[controller]/Stages")]
        public IActionResult GetStages()
        {
            try
            {
                var result = Enum.GetValues(typeof(Enums.StageEnum)).Cast<Enums.StageEnum>().OrderBy(s => s).ToList();
                result.RemoveAt(0);
                result.RemoveAt(0);
                return Ok(result.Select(r => new Models.Nococid.Stage
                {
                    StageCode = r,
                    Name = r.ToString()
                }));
            }
            catch (Exception e)
            {
                return GetError(e);
            }
        }

        [HttpGet("Projects/{project_id}/[controller]")]
        public IActionResult GetAll([FromRoute] Guid project_id)
        {
            try
            {
                JwtClaimM jwt_claim = _jwtAuth.GetClaims(Request);
                _permission.EnsureProjectMember(jwt_claim.UserId, project_id);
                return Ok(_sprint.GetAll(project_id));
            }
            catch (Exception e)
            {
                return GetError(e);
            }
        }

        [HttpGet("Projects/{project_id}/[controller]/{sprint_id}")]
        public IActionResult GetDetail([FromRoute] Guid project_id, [FromRoute] Guid sprint_id)
        {
            try
            {
                JwtClaimM jwt_claim = _jwtAuth.GetClaims(Request);
                _permission.EnsureProjectManager(jwt_claim.UserId, project_id);
                _sprint.EnsureExisted(project_id, sprint_id);
                return Ok(_sprint.GetDetail(sprint_id));
            }
            catch (Exception e)
            {
                return GetError(e);
            }
        }

        [HttpPost("Projects/{project_id}/[controller]/{sprint_id}/approve-to-next-stage")]
        public IActionResult GoNextStage([FromRoute] Guid project_id, [FromRoute] Guid sprint_id)
        {
            try
            {
                JwtClaimM jwt_claim = _jwtAuth.GetClaims(Request);
                _permission.EnsureProjectManager(jwt_claim.UserId, project_id);
                return Ok(_sprint.GoNextStage(project_id, sprint_id));
            }
            catch (Exception e)
            {
                return GetError(e);
            }
        }

        [HttpPut("Projects/{project_id}/[controller]/{sprint_id}")]
        [ExampleOperationProcessor(typeof(SprintUpdateMExample))]
        public IActionResult UpdateSprint([FromRoute] Guid project_id, [FromRoute] Guid sprint_id, [FromBody] SprintUpdateM model)
        {
            try
            {
                JwtClaimM jwt_claim = _jwtAuth.GetClaims(Request);
                _permission.EnsureProjectManager(jwt_claim.UserId, project_id);
                _sprint.EnsureExisted(project_id, sprint_id);
                return Ok(_sprint.UpdateSprint(sprint_id, model));
            }
            catch (Exception e)
            {
                return GetError(e);
            }
        }
    }
}