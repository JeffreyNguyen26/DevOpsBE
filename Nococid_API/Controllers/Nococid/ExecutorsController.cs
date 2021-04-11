using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nococid_API.Services;
using Nococid_API.Services.Nococid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nococid_API.Controllers.Nococid
{
    [Route("api")]
    [ApiController]
    public class ExecutorsController : NococidControllerBase
    {
        private readonly IExecutorService _executor;

        public ExecutorsController(IExecutorService executor, IErrorHandlerService errorHandler, IJwtAuthService jwtAuth) : base(errorHandler, jwtAuth)
        {
            _executor = executor;
        }

        [HttpGet("Tools/{tool_id}/[controller]")]
        public IActionResult GetAll([FromRoute] Guid tool_id)
        {
            try
            {
                return Ok(_executor.GetAll(tool_id));
            }
            catch (Exception e)
            {
                return GetError(e);
            }
        }

        [HttpGet("[controller]/{executor_id}")]
        public IActionResult Get([FromRoute] Guid executor_id)
        {
            try
            {
                return Ok(_executor.Get(executor_id));
            }
            catch (Exception e)
            {
                return GetError(e);
            }
        }
    }
}
