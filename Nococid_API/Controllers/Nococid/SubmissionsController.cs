using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nococid_API.Attributes;
using Nococid_API.Models.Examples.Submission;
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
    public class SubmissionsController : NococidControllerBase
    {
        private readonly ISubmissionService _submission;
        private readonly ICommitService _commit;
        private readonly IPermissionService _permission;
        private readonly ISprintService _sprint;

        public SubmissionsController(ISprintService sprint, IPermissionService permission, ICommitService commit, ISubmissionService submission, IErrorHandlerService errorHandler, IJwtAuthService jwtAuth) : base(errorHandler, jwtAuth)
        {
            _sprint = sprint;
            _permission = permission;
            _commit = commit;
            _submission = submission;
        }

        [HttpGet("Commits/{commit_id}/Submission")]
        public IActionResult GetSubmission([FromRoute] Guid commit_id)
        {
            try
            {
                JwtClaimM jwt_claim = _jwtAuth.GetClaims(Request);
                _commit.EnsureExisted(jwt_claim.UserId, commit_id);
                return Ok(_submission.GetSubmission(commit_id));
            }
            catch (Exception e)
            {
                return GetError(e);
            }
        }

        [HttpPost("Projects/{project_id}/Sprints/{sprint_id}/[controller]")]
        [ExampleOperationProcessor(typeof(TasksSubmissionMExample))]
        public IActionResult SubmitTasks([FromRoute] Guid project_id, [FromRoute] Guid sprint_id, [FromBody] TasksSubmissionM model)
        {
            try
            {
                JwtClaimM jwt_claim = _jwtAuth.GetClaims(Request);
                _permission.EnsureProjectMember(jwt_claim.UserId, project_id);
                _sprint.EnsureExisted(project_id, sprint_id);
                _commit.EnsureExisted(jwt_claim.UserId, model.CommitId);
                return Ok(_submission.SubmitTasks(jwt_claim.UserId, sprint_id, model.CommitId, model.TaskIds));
            }
            catch (Exception e)
            {
                return GetError(e);
            }
        }

        [HttpPost("Projects/{project_id}/Sprints/{sprint_id}/[controller]/{submission_id}")]
        [ExampleOperationProcessor(typeof(ReportErrorMExample))]
        public IActionResult ReportError([FromRoute] Guid project_id, [FromRoute] Guid sprint_id, [FromRoute] Guid submission_id, [FromBody] ReportErrorM model)
        {
            try
            {
                JwtClaimM jwt_claim = _jwtAuth.GetClaims(Request);
                _permission.EnsureTester(jwt_claim.UserId, project_id);
                _submission.ReportError(project_id, sprint_id, submission_id, model);
                return Ok();
            }
            catch (Exception e)
            {
                return GetError(e);
            }
        }
    }
}