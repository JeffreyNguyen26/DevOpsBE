using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Nococid_API.Data.Static;
using Nococid_API.Enums;
using Nococid_API.Models.CircleCIs;
using Nococid_API.Models.Github;
using Nococid_API.Models.Https;
using Nococid_API.Models.Nococid;
using Nococid_API.Models.Nococid.Errors;
using Nococid_API.Models.Theads;
using Nococid_API.Services;
using Nococid_API.Services.CircleCI;
using Nococid_API.Services.Nococid;
using Nococid_API.Services.Thread;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Nococid_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ListenersController : NococidControllerBase
    {
        private readonly ICommitService _commit;
        private readonly ITokenService _token;
        private readonly IWorkflowThreadService _workflowThread;
        private readonly IPipelineThreadService _pipelineThread;
        private readonly IConfigurationFileService _configurationFile;

        public ListenersController(IConfigurationFileService configurationFile, IPipelineThreadService pipelineThread, IWorkflowThreadService workflowThread, ITokenService token, ICommitService commit, IErrorHandlerService errorHandler, IJwtAuthService jwtAuth) : base(errorHandler, jwtAuth)
        {
            _configurationFile = configurationFile;
            _pipelineThread = pipelineThread;
            _workflowThread = workflowThread;
            _token = token;
            _commit = commit;
        }

        [HttpGet("CircleCI")]
        public IActionResult GetFileCircleCI([FromQuery] string file, [FromQuery] Guid project_id)
        {
            try
            {
                return file switch
                {
                    "Heroku" => Ok(_configurationFile.GetHerokuFile(project_id)),
                    _ => NotFound(),
                };
            }
            catch (Exception e)
            {
                return GetError(e);
            }
        }

        [HttpPost("CircleCI/job-done")]
        public IActionResult TriggerCircleCIJobDone([FromQuery] Guid circle_workflow_id, [FromQuery] int circle_job_num, [FromQuery] string stage, [FromQuery] string gh_username, [FromQuery] string gh_repository)
        {
            try
            {
                string circle_token = _token.GetToken(ToolID.CircleCI, gh_username, VSCID.Github);
                new Thread(_pipelineThread.CreateFromCircleCI).Start(new CircleCINococidPipelineCM
                {
                    CircleJobNum = circle_job_num,
                    CircleToken = circle_token,
                    CircleWorkflowId = circle_workflow_id,
                    JobName = stage,
                    Repository = gh_repository,
                    Username = gh_username
                });
                return Ok();
            }
            catch (Exception e)
            {
                return GetError(e);
            }
        }

        [HttpPost("CircleCI/start-workflow")]
        public IActionResult CreateWorkflowFromCircleCI([FromQuery] Guid sprint_id, [FromQuery] int number_of_jobs, [FromQuery] string gh_username, [FromQuery] Guid circle_workflow_id)
        {
            try
            {
                string circle_token = _token.GetToken(ToolID.CircleCI, gh_username, VSCID.Github);
                new Thread(_workflowThread.CreateFromCircleCI).Start(new CircleCINococidWorkflowCM
                {
                    TotalJob = number_of_jobs,
                    CircleToken = circle_token,
                    CircleWorkflowId = circle_workflow_id,
                    SprintId = sprint_id,
                    Username = gh_username
                });
                return Ok();
            }
            catch (Exception e)
            {
                return GetError(e);
            }
        }

        static string payload = "";

        [HttpPost("Github/webhook/payload")]
        public IActionResult ExecuteGHWebhookPayload([FromBody] GHWebhookPayload model)
        {
            try
            {
                _commit.Add(model);
                return Ok();
            }
            catch (Exception e)
            {
                return GetError(e);
            }
        }

        [HttpGet("payload")]
        [AllowAnonymous]
        public IActionResult Get([FromQuery] string datetime)
        {
            try
            {
                bool result = DateTime.TryParse(datetime, out DateTime now);
                return Ok(result + " +---+ " + now.Kind + " ==== " + DateTime.Now.ToString() + payload);
            }
            catch (Exception e)
            {
                return Ok(e.Message);
            }
            
        }
    }
}
