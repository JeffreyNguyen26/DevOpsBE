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
    public class UserFilter : IAuthorizationFilter
    {
        private readonly IErrorHandlerService _errorHandler;
        private readonly IJwtAuthService _jwtAuth;
        private readonly IUserService _user;

        public UserFilter(IUserService user, IErrorHandlerService errorHandler, IJwtAuthService jwtAuth)
        {
            _user = user;
            _errorHandler = errorHandler;
            _jwtAuth = jwtAuth;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            try
            {
                JwtClaimM jwtClaim = _jwtAuth.GetClaims(context.HttpContext.Request);
                if (context.HttpContext.Request.RouteValues.TryGetValue("user_id", out object value))
                {
                    if (Guid.TryParse(value.ToString(), out Guid user_id))
                    {
                        if (!jwtClaim.UserId.Equals(user_id))
                        {
                            context.Result = new UnauthorizedObjectResult(new HttpResponseError
                            {
                                StatusCode = 404,
                                Detail = new HttpResponseErrorDetail
                                {
                                    Message = "Unable to find resource!",
                                    InnerMessage = "The 'user id' with value '" + user_id.ToString() + "' is not exist!"
                                }
                            });
                        }
                    } else
                    {
                        context.Result = new UnauthorizedResult();
                    }
                } else
                {
                    context.Result = new UnauthorizedResult();
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
