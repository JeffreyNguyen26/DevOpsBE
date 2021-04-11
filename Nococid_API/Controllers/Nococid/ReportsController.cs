using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
    public class ReportsController : NococidControllerBase
    {
        private readonly IReportService _report;
        private readonly IPermissionService _permission;
        private readonly ISprintService _sprint;
        private readonly IWorkflowService _workflow;

        public ReportsController(IWorkflowService workflow, ISprintService sprint, IPermissionService permission, IReportService report, IErrorHandlerService errorHandler, IJwtAuthService jwtAuth) : base(errorHandler, jwtAuth)
        {
            _workflow = workflow;
            _sprint = sprint;
            _permission = permission;
            _report = report;
        }

        [HttpGet("Projects/{project_id}/Sprints/{sprint_id}/Workflows/{workflow_id}")]
        public IActionResult GetReportWorkflow([FromRoute] Guid project_id, [FromRoute] Guid sprint_id, [FromRoute] Guid workflow_id)
        {
            try
            {
                JwtClaimM jwt_claim = _jwtAuth.GetClaims(Request);
                _permission.EnsureProjectManager(jwt_claim.UserId, project_id);
                _sprint.EnsureExisted(project_id, sprint_id);
                _workflow.EnsureExisted(sprint_id, workflow_id);
                return Ok();
            }
            catch (Exception e)
            {
                return GetError(e);
            }
        }

        [HttpGet]
        public IActionResult GetInsights()
        {
            try
            {
                JwtClaimM jwt_claim = _jwtAuth.GetClaims(Request);
                if (!jwt_claim.AdminUserId.Equals(Guid.Empty)) return Forbid();
                return Ok(_report.GetInsights(jwt_claim.UserId));
            }
            catch (Exception e)
            {
                return GetError(e);
            }
        }
    }
}
