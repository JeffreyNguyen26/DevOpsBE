using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nococid_API.Attributes;
using Nococid_API.Attributes.Filters;
using Nococid_API.Models.Examples.Tasks;
using Nococid_API.Models.Https;
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
    public class TasksController : NococidControllerBase
    {
        private readonly IPermissionService _permission;
        private readonly IProjectRepositoryService _projectRepository;
        private readonly ISprintService _sprint;
        private readonly ITaskService _task;
        private readonly IReportService _report;

        public TasksController(IProjectRepositoryService projectRepository, IReportService report, ITaskService task, ISprintService sprint, IPermissionService permission, IErrorHandlerService errorHandler, IJwtAuthService jwtAuth) : base(errorHandler, jwtAuth)
        {
            _projectRepository = projectRepository;
            _report = report;
            _task = task;
            _sprint = sprint;
            _permission = permission;
        }

        [HttpGet("Projects/{project_id}/Sprints/{sprint_id}/[controller]")]
        public IActionResult GetTask([FromRoute] Guid project_id, [FromRoute] Guid sprint_id, [FromQuery] string task = "my_task")
        {
            try
            {
                JwtClaimM jwt_claim = _jwtAuth.GetClaims(Request);
                switch (task)
                {
                    case "all_task":
                        _permission.EnsureProjectManager(jwt_claim.UserId, project_id);
                        _sprint.EnsureExisted(project_id, sprint_id);
                        return Ok(_task.GetAll(sprint_id));
                    case "my_task":
                        _permission.EnsureProjectMember(jwt_claim.UserId, project_id);
                        _sprint.EnsureExisted(project_id, sprint_id);
                        return Ok(_task.GetAllMyTask(sprint_id, jwt_claim.UserId));
                    case "submitted_task":
                        _permission.EnsureTester(jwt_claim.UserId, project_id);
                        _sprint.EnsureExisted(project_id, sprint_id);
                        return Ok(_task.GetSubmittedTask(sprint_id));
                }

                return BadRequest(new HttpResponseError { 
                    StatusCode = 400,
                    Detail = new HttpResponseErrorDetail
                    {
                        Message = "Unable to execute the request!",
                        InnerMessage = "Parameter 'task' must have value of 'all_task', 'my_task' or 'submitted_task'"
                    }
                });
            }
            catch (Exception e)
            {
                return GetError(e);
            }
        }

        [HttpGet("Projects/{project_id}/Sprints/{sprint_id}/[controller]/{task_id}")]
        public IActionResult GetTaskMoreDetail([FromRoute] Guid project_id, [FromRoute] Guid sprint_id, [FromRoute] Guid task_id)
        {
            try
            {
                JwtClaimM jwt_claim = _jwtAuth.GetClaims(Request);
                _permission.EnsureProjectMember(jwt_claim.UserId, project_id);
                _sprint.EnsureExisted(project_id, sprint_id);
                _task.EnsureExisted(sprint_id, task_id);
                return Ok(_task.GetTaskMoreDeatil(jwt_claim.UserId, project_id, task_id));
            }
            catch (Exception e)
            {
                return GetError(e);
            }
        }
        
        [HttpGet("Commits/{commit_id}/[controller]")]
        public IActionResult GetAll([FromRoute] Guid commit_id)
        {
            try
            {
                JwtClaimM jwt_claim = _jwtAuth.GetClaims(Request);
                return Ok(_task.GetForCommit(jwt_claim.UserId, commit_id));
            }
            catch (Exception e)
            {
                return GetError(e);
            }
        }

        [HttpGet("[controller]/implementing")]
        public IActionResult GetImplementingTasks()
        {
            try
            {
                JwtClaimM jwt_claim = _jwtAuth.GetClaims(Request);
                return Ok(_task.GetImplementingTasks(jwt_claim.UserId));
            }
            catch (Exception e)
            {
                return GetError(e);
            }
        }

        [HttpPost("Projects/{project_id}/[controller]/create")]
        [ExampleOperationProcessor(typeof(TaskSprintCreateMExample))]
        public IActionResult Create([FromRoute] Guid project_id, [FromBody] TaskSprintCreateM model)
        {
            try
            {
                JwtClaimM jwt_claim = _jwtAuth.GetClaims(Request);
                _permission.EnsureProjectManager(jwt_claim.UserId, project_id);
                Guid sprint_id;
                if (model.SprintId == null)
                {
                    sprint_id = _sprint.Add(project_id, model.StartDate, model.EndDate, model.Approvals);
                } else
                {
                    sprint_id = model.SprintId.Value;
                }
                return Created("", _task.AddMany(project_id, sprint_id, model));
            }
            catch (Exception e)
            {
                return GetError(e);
            }
        }
    }
}
