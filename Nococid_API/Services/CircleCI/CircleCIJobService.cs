using Nococid_API.Enums;
using Nococid_API.Models.CircleCIs;
using Nococid_API.Models.Nococid.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nococid_API.Services.CircleCI
{
    public interface ICircleCIJobService
    {
        CircleCIJob GetDetail(string gh_username, string repository_name, string circle_token, int job_number);
        CircleCIJobTestMetaDataM GetTestMetaData(string gh_username, string repository_name, string circle_token, int job_number);
    }

    public class CircleCIJobService : CircleCIServiceBase, ICircleCIJobService
    {
        public CircleCIJobService(IErrorHandlerService errorHandler, IHttpRequestService httpRequest) : base(errorHandler, httpRequest) { }

        public CircleCIJob GetDetail(string gh_username, string repository_name, string circle_token, int job_number)
        {
            try
            {
                return _httpRequest.Send<CircleCIJob>(
                    "https://circleci.com/api/v2/project/gh/" + gh_username + "/" + repository_name + "/job/" + job_number,
                    HttpRequestMethod.Get, new KeyValuePair<string, string>[]
                    {
                        Accept_Json,
                        new KeyValuePair<string, string>("Circle-Token", circle_token)
                    }
                );
            }
            catch (Exception e)
            {
                throw e is ServerException ? e : _errorHandler.WriteLog("An error occurred while get a job detail!",
                    e, DateTime.Now, "Server", "Service_CircleCIJob_GetAJobDetail");
            }
        }

        public CircleCIJobTestMetaDataM GetTestMetaData(string gh_username, string repository_name, string circle_token, int job_number)
        {
            try
            {
                return _httpRequest.Send<CircleCIJobTestMetaDataM>(
                    "http://circleci.com/api/v2/project/gh/" + gh_username + "/" + repository_name + "/" + job_number + "/tests",
                    HttpRequestMethod.Get, new KeyValuePair<string, string>[]
                    {
                        Accept_Json,
                        new KeyValuePair<string, string>("Circle-Token", circle_token)
                    }
                );
            }
            catch (Exception e)
            {
                throw e is ServerException ? e : _errorHandler.WriteLog("An error occurred while get a job's test meta data!",
                    e, DateTime.Now, "Server", "Service_CircleCIJob_GetTestMetaData");
            }
        }
    }
}
