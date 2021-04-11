using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nococid_API.Models.Nococid.Configuration;
using Nococid_API.Services;
using Nococid_API.Services.Nococid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nococid_API.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConfigurationsController : NococidControllerBase
    {
        private readonly IConfigurationService _configuration;

        public ConfigurationsController(IConfigurationService configuration, IErrorHandlerService errorHandler, IJwtAuthService jwtAuth) : base(errorHandler, jwtAuth)
        {
            _configuration = configuration;
        }

        #region Executor
        [HttpGet("Tools/{tool_id}/Executors")]
        public IActionResult GetExecutors([FromRoute] Guid tool_id)
        {
            try
            {
                return Ok(_configuration.GetExecutors(tool_id));
            }
            catch (Exception e)
            {
                return GetError(e);
            }
        }

        [HttpPost("Tools/{tool_id}/Executors")]
        public IActionResult AddExecutor([FromRoute] Guid tool_id, [FromBody] ExecutorCreateM model)
        {
            try
            {
                return Created("", _configuration.AddExecutor(tool_id, model));
            }
            catch (Exception e)
            {
                return GetError(e);
            }
        }

        [HttpPut("Tools/{tool_id}/Executors/{executor_id}")]
        public IActionResult UpdateExecutor([FromRoute] Guid tool_id, [FromRoute] Guid executor_id, [FromBody] ExecutorUpdateM model)
        {
            try
            {
                return Ok(_configuration.UpdateExecutor(tool_id, executor_id, model));
            }
            catch (Exception e)
            {
                return GetError(e);
            }
        }

        [HttpDelete("Tools/{tool_id}/Executors/{executor_id}")]
        public IActionResult DeleteExecutor([FromRoute] Guid tool_id, [FromRoute] Guid executor_id)
        {
            try
            {
                _configuration.DeleteExecutor(tool_id, executor_id);
                return Ok();
            }
            catch (Exception e)
            {
                return GetError(e);
            }
        }
        #endregion

        #region Executor Image
        [HttpGet("Tools/{tool_id}/Executors/{executor_id}/ExecutorImages")]
        public IActionResult GetExecutorImages([FromRoute] Guid tool_id, [FromRoute] Guid executor_id)
        {
            try
            {
                return Ok(_configuration.GetExecutorImages(tool_id, executor_id));
            }
            catch (Exception e)
            {
                return GetError(e);
            }
        }

        [HttpPost("Tools/{tool_id}/Executors/{executor_id}/ExecutorImages")]
        public IActionResult AddExecutorImage([FromRoute] Guid tool_id, [FromRoute] Guid executor_id,[FromBody] ExecutorImageCreateM model)
        {
            try
            {
                return Created("", _configuration.AddExecutorImage(tool_id, executor_id, model));
            }
            catch (Exception e)
            {
                return GetError(e);
            }
        }

        [HttpPut("Tools/{tool_id}/Executors/{executor_id}/ExecutorImages/{executor_image_id}")]
        public IActionResult UpdateExecutorImage([FromRoute] Guid tool_id, [FromRoute] Guid executor_id, [FromRoute] Guid executor_image_id, [FromBody] ExecutorImageUpdateM model)
        {
            try
            {
                return Ok(_configuration.UpdateExecutorImage(tool_id, executor_id, executor_image_id, model));
            }
            catch (Exception e)
            {
                return GetError(e);
            }
        }

        [HttpDelete("Tools/{tool_id}/Executors/{executor_id}/ExecutorImages/{executor_image_id}")]
        public IActionResult DeleteExecutorImage([FromRoute] Guid tool_id, [FromRoute] Guid executor_id, [FromRoute] Guid executor_image_id)
        {
            try
            {
                _configuration.DeleteExecutorImage(tool_id, executor_id, executor_image_id);
                return Ok();
            }
            catch (Exception e)
            {
                return GetError(e);
            }
        }
        #endregion

        #region Resouce Class
        [HttpGet("Tools/{tool_id}/Executors/{executor_id}/ResouceClasses")]
        public IActionResult GetResourceClasses([FromRoute] Guid tool_id, [FromRoute] Guid executor_id)
        {
            try
            {
                return Ok(_configuration.GetResourceClasses(tool_id, executor_id));
            }
            catch (Exception e)
            {
                return GetError(e);
            }
        }

        [HttpPost("Tools/{tool_id}/Executors/{executor_id}/ResouceClasses")]
        public IActionResult AddResourceClass([FromRoute] Guid tool_id, [FromRoute] Guid executor_id, [FromBody] ResourceClassCreateM model)
        {
            try
            {
                return Created("", _configuration.AddResourceClass(tool_id, executor_id, model));
            }
            catch (Exception e)
            {
                return GetError(e);
            }
        }

        [HttpPut("Tools/{tool_id}/Executors/{executor_id}/ResouceClasses/{resouce_classes_id}")]
        public IActionResult UpdateResourceClass([FromRoute] Guid tool_id, [FromRoute] Guid executor_id, [FromRoute] Guid resouce_classes_id, [FromBody] ResourceClassUpdateM model)
        {
            try
            {
                return Ok(_configuration.UpdateResourceClass(tool_id, executor_id, resouce_classes_id, model));
            }
            catch (Exception e)
            {
                return GetError(e);
            }
        }

        [HttpDelete("Tools/{tool_id}/Executors/{executor_id}/ResouceClasses/{resouce_classes_id}")]
        public IActionResult DeleteResourceClass([FromRoute] Guid tool_id, [FromRoute] Guid executor_id, [FromRoute] Guid resouce_classes_id)
        {
            try
            {
                _configuration.DeleteResourceClass(tool_id, executor_id, resouce_classes_id);
                return Ok();
            }
            catch (Exception e)
            {
                return GetError(e);
            }
        }
        #endregion

        #region Step
        [HttpGet("Tools/{tool_id}/Executors/{executor_id}/Steps")]
        public IActionResult GetSteps([FromRoute] Guid tool_id, [FromRoute] Guid executor_id)
        {
            try
            {
                return Ok(_configuration.GetSteps(tool_id, executor_id));
            }
            catch (Exception e)
            {
                return GetError(e);
            }
        }

        [HttpPost("Tools/{tool_id}/Executors/{executor_id}/Steps")]
        public IActionResult AddStep([FromRoute] Guid tool_id, [FromRoute] Guid executor_id, [FromBody] StepCreateM model)
        {
            try
            {
                return Created("", _configuration.AddStep(tool_id, executor_id, model));
            }
            catch (Exception e)
            {
                return GetError(e);
            }
        }

        [HttpPut("Tools/{tool_id}/Executors/{executor_id}/Steps/{step_id}")]
        public IActionResult UpdateStep([FromRoute] Guid tool_id, [FromRoute] Guid executor_id, [FromRoute] Guid step_id, [FromBody] StepUpdateM model)
        {
            try
            {
                return Ok(_configuration.UpdateStep(tool_id, executor_id, step_id, model));
            }
            catch (Exception e)
            {
                return GetError(e);
            }
        }

        [HttpDelete("Tools/{tool_id}/Executors/{executor_id}/Steps/{step_id}")]
        public IActionResult DeleteStep([FromRoute] Guid tool_id, [FromRoute] Guid executor_id, [FromRoute] Guid step_id)
        {
            try
            {
                _configuration.DeleteStep(tool_id, executor_id, step_id);
                return Ok();
            }
            catch (Exception e)
            {
                return GetError(e);
            }
        }
        #endregion
    }
}
