using Nococid_API.Models.Https;
using Nococid_API.Models.Nococid.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nococid_API.Services.Nococid
{
    public class ServiceBase
    {
        protected readonly IErrorHandlerService _errorHandler;

        public ServiceBase(IErrorHandlerService errorHandler)
        {
            _errorHandler = errorHandler;
        }

        protected RequestException NotFound(Guid id, string source)
        {
            return new RequestException
            {
                Error = new HttpResponseError
                {
                    StatusCode = 404,
                    Detail = new HttpResponseErrorDetail
                    {
                        Message = "Unable to find resource!",
                        InnerMessage = "The '" + source + "' with value '" + id.ToString() + "' is not exist!"
                    }
                }
            };
        }

        protected RequestException NotFound(string value, string source)
        {
            return new RequestException
            {
                Error = new HttpResponseError
                {
                    StatusCode = 404,
                    Detail = new HttpResponseErrorDetail
                    {
                        Message = "Unable to find resource!",
                        InnerMessage = "The '" + source + "' with value '" + value + "' is not exist!"
                    }
                }
            };
        }

        protected RequestException NotFound()
        {
            return new RequestException
            {
                Error = new HttpResponseError
                {
                    StatusCode = 404,
                    Detail = new HttpResponseErrorDetail
                    {
                        Message = "Unable to find resource!",
                        InnerMessage = "The resource you are looking for is not exist!"
                    }
                }
            };
        }

        protected RequestException BadRequest(string message)
        {
            return new RequestException
            {
                Error = new HttpResponseError
                {
                    StatusCode = 400,
                    Detail = new HttpResponseErrorDetail
                    {
                        Message = "Unable to execute the request!",
                        InnerMessage = message
                    }
                }
            };
        }

        protected RequestException Forbidden()
        {
            return new RequestException
            {
                Error = new HttpResponseError
                {
                    StatusCode = 403,
                    Detail = new HttpResponseErrorDetail
                    {
                        Message = "Forbidden",
                        InnerMessage = "You do not have permission to do this acction!"
                    }
                }
            };
        }
    }
}
