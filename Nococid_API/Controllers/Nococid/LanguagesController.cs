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
    public class LanguagesController : NococidControllerBase
    {
        private readonly ILanguageService _language;

        public LanguagesController(ILanguageService language, IErrorHandlerService errorHandler, IJwtAuthService jwtAuth) : base(errorHandler, jwtAuth)
        {
            _language = language;
        }

        [HttpGet]
        public IActionResult GetAll([FromQuery] string side)
        {
            try
            {
                return Ok(_language.GetAll(side));
            }
            catch (Exception e)
            {
                return GetError(e);
            }
        }
    }
}
