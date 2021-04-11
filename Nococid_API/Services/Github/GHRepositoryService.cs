using Nococid_API.Enums;
using Nococid_API.Models.Github;
using Nococid_API.Models.Nococid.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nococid_API.Services.Github
{
    public interface IGHRepositoryService
    {
        IList<GHRepository> Get(string gh_username, string access_token, int page);
        GHRepositoryDetail Get(string gh_username, string repository_name, string access_token);
        IDictionary<string, int> GetLanguages(string gh_username, string repository_name, string access_token);
    }

    public class GHRepositoryService : GHServiceBase, IGHRepositoryService
    {
        public GHRepositoryService(IErrorHandlerService errorHandler, IHttpRequestService httpRequest) : base(errorHandler, httpRequest) { }

        public IList<GHRepository> Get(string gh_username, string access_token, int page)
        {
            try
            {
                return _httpRequest.Send<IList<GHRepository>>(
                    "https://api.github.com/user/repos?visibility=all&affiliation=owner,collaborator&per_page=20&page=" + page,
                    HttpRequestMethod.Get, new KeyValuePair<string, string>[]
                    {
                        Accept_V3_Json,
                        new KeyValuePair<string, string>("User-Agent", gh_username),
                        new KeyValuePair<string, string>("Authorization", "token " + access_token)
                    }
                );
            }
            catch (Exception e)
            {
                throw e is ServerException ? e : _errorHandler.WriteLog("An error occurred while search account!",
                    e, DateTime.Now, "Server", "Service_GHRepository_Get");
            }
        }

        public GHRepositoryDetail Get(string gh_username, string repository_name, string access_token)
        {
            try
            {
                return _httpRequest.Send<GHRepositoryDetail>(
                    "https://api.github.com/repos/" + gh_username + "/" + repository_name,
                    HttpRequestMethod.Get, new KeyValuePair<string, string>[]
                    {
                        Accept_V3_Json,
                        new KeyValuePair<string, string>("User-Agent", gh_username),
                        new KeyValuePair<string, string>("Authorization", "token " + access_token)
                    }
                );
            }
            catch (Exception e)
            {
                throw e is ServerException ? e : _errorHandler.WriteLog("An error occurred while get a repository!",
                    e, DateTime.Now, "Server", "Service_GHRepository_Get");
            }
        }

        public IDictionary<string, int> GetLanguages(string gh_username, string repository_name, string access_token)
        {
            try
            {
                return _httpRequest.Send<IDictionary<string, int>>(
                    "https://api.github.com/repos/" + gh_username + "/" + repository_name + "/languages",
                    HttpRequestMethod.Get, new KeyValuePair<string, string>[]
                    {
                        Accept_V3_Json,
                        new KeyValuePair<string, string>("User-Agent", gh_username),
                        new KeyValuePair<string, string>("Authorization", "token " + access_token)
                    }
                );
            }
            catch (Exception e)
            {
                throw e is ServerException ? e : _errorHandler.WriteLog("An error occurred while get a repository's languages!",
                    e, DateTime.Now, "Server", "Service_GHRepository_GetLanguages");
            }
        }
    }
}
