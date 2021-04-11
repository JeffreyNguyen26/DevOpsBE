using Nococid_API.Enums;
using Nococid_API.Models.Github;
using Nococid_API.Models.Nococid.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nococid_API.Services.Github
{
    public interface IGHReferenceService
    {
        GHReferenceCreationSuccess CreateReference(string gh_username, string repository_name, string access_token, string ref_name, string sha);
    }

    public class GHReferenceService : GHServiceBase, IGHReferenceService
    {
        public GHReferenceService(IErrorHandlerService errorHandler, IHttpRequestService httpRequest) : base(errorHandler, httpRequest) { }

        public GHReferenceCreationSuccess CreateReference(string gh_username, string repository_name, string access_token, string ref_name, string sha)
        {
            try
            {
                GHReferenceCreation data = new GHReferenceCreation
                {
                    Ref = "refs/heads/" + ref_name,
                    Sha = sha
                };
                return _httpRequest.Send<GHReferenceCreationSuccess, GHReferenceCreation>(
                    "https://api.github.com/repos/" + gh_username + "/" + repository_name + "/git/refs",
                    HttpRequestMethod.Post, new KeyValuePair<string, string>[]
                    {
                        Accept_V3_Json,
                        new KeyValuePair<string, string>("User-Agent", gh_username),
                        new KeyValuePair<string, string>("Authorization", "token " + access_token)
                    }, data
                );
            }
            catch (Exception e)
            {
                throw e is ServerException ? e : _errorHandler.WriteLog("An error occurred while create reference!",
                    e, DateTime.Now, "Server", "Service_GHReference_CreateReference");
            }
        }
    }
}
