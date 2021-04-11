using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nococid_API.Models.Nococid;
using Nococid_API.Services;
using Nococid_API.Services.Nococid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nococid_API.Controllers.Admin
{
    [Route("api")]
    [ApiController]
    public class FrameworksController : NococidControllerBase
    {
        private readonly IFrameworkService _framework;

        public FrameworksController(IFrameworkService framework, IErrorHandlerService errorHandler, IJwtAuthService jwtAuth) : base(errorHandler, jwtAuth)
        {
            _framework = framework;
        }

        [HttpPost("Languages/{language_id}/[controller]/create")]
        public IActionResult Create([FromRoute] Guid language_id, [FromBody] FrameworkCreateM model)
        {
            try
            {
                return Created("", _framework.Add(language_id, model));
            }
            catch (Exception e)
            {
                return GetError(e);
            }
        }
    }
}
