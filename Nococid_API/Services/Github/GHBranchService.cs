using Nococid_API.Enums;
using Nococid_API.Models.Github;
using Nococid_API.Models.Nococid.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nococid_API.Services.Github
{
    public interface IGHBranchService
    {
        IList<GHBranch> GetBranches(string gh_username, string repository_name, string access_token, int page);
        string GetBranchSha(string gh_username, string repository_name, string access_token, string branch_name);
    }

    public class GHBranchService : GHServiceBase, IGHBranchService
    {
        public GHBranchService(IErrorHandlerService errorHandler, IHttpRequestService httpRequest) : base(errorHandler, httpRequest) { }

        public IList<GHBranch> GetBranches(string gh_username, string repository_name, string access_token, int page)
        {
            try
            {
                return _httpRequest.Send<IList<GHBranch>>(
                    "https://api.github.com/repos/" + gh_username + "/" + repository_name + "/branches?per_page=100&page=" + page.ToString(),
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
                throw e is ServerException ? e : _errorHandler.WriteLog("An error occurred while get all repository branches from github!",
                    e, DateTime.Now, "Server", "Service_GHBranch_GetBranches");
            }
        }

        public string GetBranchSha(string gh_username, string repository_name, string access_token, string branch_name)
        {
            try
            {
                IList<GHBranch> branches;
                int page = 1;
                do
                {
                    branches = GetBranches(gh_username, repository_name, access_token, page++);
                    GHBranch branch = branches.Where(b => b.Name.Equals(branch_name)).FirstOrDefault();
                    if (branch != null)
                    {
                        return branch.Commit.Sha;
                    }
                } while (branches.Count != 0);
                return null;
            }
            catch (Exception e)
            {
                throw e is ServerException ? e : _errorHandler.WriteLog("An error occurred while get sha of a branch!",
                    e, DateTime.Now, "Server", "Service_GHBranch_GetBranches");
            }
        }
    }
}
