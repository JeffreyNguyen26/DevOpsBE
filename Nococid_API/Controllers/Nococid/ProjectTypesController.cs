using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
    public class ProjectTypesController : NococidControllerBase
    {
        private readonly IProjectTypeService _projectType;

        public ProjectTypesController(IProjectTypeService projectType, IErrorHandlerService errorHandler, IJwtAuthService jwtAuth) : base(errorHandler, jwtAuth)
        {
            _projectType = projectType;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                return Ok(_projectType.GetAll());
            }
            catch (Exception e)
            {
                return GetError(e);
            }
        }

        [HttpGet("{project_type_id}")]
        public IActionResult GetDetail([FromRoute] Guid project_type_id)
        {
            try
            {
                return Ok(_projectType.GetDetail(project_type_id));
            }
            catch (Exception e)
            {
                return GetError(e);
            }
        }
    }
}
