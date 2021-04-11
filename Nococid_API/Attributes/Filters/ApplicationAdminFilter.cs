using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Nococid_API.Data.Static;
using Nococid_API.Models.Https;
using Nococid_API.Models.Nococid.Auth;
using Nococid_API.Models.Nococid.Errors;
using Nococid_API.Services;
using Nococid_API.Services.Nococid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nococid_API.Attributes.Filters
{
    public class ApplicationAdminFilter : IAuthorizationFilter
    {
        private readonly IErrorHandlerService _errorHandler;
        private readonly IJwtAuthService _jwtAuth;

        public ApplicationAdminFilter(IErrorHandlerService errorHandler, IJwtAuthService jwtAuth)
        {
            _errorHandler = errorHandler;
            _jwtAuth = jwtAuth;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            try
            {
                JwtClaimM jwtClaim = _jwtAuth.GetClaims(context.HttpContext.Request);
                if (!jwtClaim.ApplicationRole.Equals(ApplicationRole.Application_Admin))
                {
                    context.Result = new ObjectResult("")
                    {
                        StatusCode = 403,
                        Value = new HttpResponseError
                        {
                            StatusCode = 403,
                            Detail = new HttpResponseErrorDetail
                            {
                                Message = "Forbidden!",
                                InnerMessage = "You do not have permission to do this action"
                            }
                        }
                    };
                }
            }
            catch (Exception e)
            {
                var se = _errorHandler.WriteLog("Error while authorize user!",
                    e, DateTime.Now, "Server", "Attribute_ApplicationAdminFilter_OnAuthorization");

                context.Result = new ObjectResult("")
                {
                    StatusCode = 500,
                    Value = new ServerExceptionVM
                    {
                        Message = se.Message,
                        TraceId = se.TraceId,
                        Side = se.Side
                    }
                };
            }
        }
    }
}
