using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nococid_API.Attributes;
using Nococid_API.Attributes.Filters;
using Nococid_API.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Nococid_API.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    //[AuthorizedExtensionFilter(typeof(ApplicationAdminFilter))]
    public class ErrorsController : ControllerBase
    {
        private readonly IErrorHandlerService _errorHandler;

        public ErrorsController(IErrorHandlerService errorHandler)
        {
            _errorHandler = errorHandler;
        }

        [HttpGet("{traceId}")]
        public IActionResult GetError([FromRoute] int traceId)
        {
            return Ok(_errorHandler.ReadError(traceId));
        }
    }
}
