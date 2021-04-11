using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nococid_API.Models.Nococid.Errors;
using Nococid_API.Services;
using Nococid_API.Services.Nococid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Nococid_API.Controllers
{
    public class NococidControllerBase : ControllerBase
    {
        protected readonly IErrorHandlerService _errorHandler;
        protected readonly IJwtAuthService _jwtAuth;

        public NococidControllerBase(IErrorHandlerService errorHandler, IJwtAuthService jwtAuth)
        {
            _errorHandler = errorHandler;
            _jwtAuth = jwtAuth;
        }

        protected IActionResult GetError(Exception e, [CallerMemberName] string callerName = "")
        {
            if (e is RequestException re)
            {
                return re.Error.StatusCode switch
                {
                    400 => BadRequest(re.Error),
                    401 => Unauthorized(re.Error),
                    403 => StatusCode(403, re.Error),
                    404 => NotFound(re.Error),
                    _ => null,
                };
            }
            else
            {
                if (!(e is ServerException se))
                {
                    se = _errorHandler.WriteLog("An error has occured!", e, DateTime.Now, "Server", "Controller_Method_" + callerName);
                }
                return StatusCode(500, new ServerExceptionVM
                {
                    Message = se.Message,
                    Side = se.Side,
                    TraceId = se.TraceId
                });
            }
        }
    }
}
