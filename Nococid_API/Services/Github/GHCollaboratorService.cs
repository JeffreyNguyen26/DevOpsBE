using Nococid_API.Models.Github;
using Nococid_API.Models.Nococid.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nococid_API.Services.Github
{
    public interface IGHCollaboratorService
    {
        GHInvitationM AddCollaborator(string gh_username, string repository_name, string access_token, string collaborator_name);
    }

    public class GHCollaboratorService : GHServiceBase, IGHCollaboratorService
    {
        public GHCollaboratorService(IErrorHandlerService errorHandler, IHttpRequestService httpRequest) : base(errorHandler, httpRequest) { }

        public GHInvitationM AddCollaborator(string gh_username, string repository_name, string access_token, string collaborator_name)
        {
            try
            {
                return _httpRequest.Send<GHInvitationM, GHCollaboratorCreateM>(
                    "https://api.github.com/repos/" + gh_username + "/" + repository_name + "/collaborators/" + collaborator_name,
                    Enums.HttpRequestMethod.Put, new KeyValuePair<string, string>[]
                    {
                        Accept_V3_Json,
                        new KeyValuePair<string, string>("User-Agent", gh_username),
                        new KeyValuePair<string, string>("Authorization", "token " + access_token)
                    }, new GHCollaboratorCreateM
                    {
                        Permissions = "write"
                    }
                );
            }
            catch (Exception e)
            {
                throw e is ServerException ? e : _errorHandler.WriteLog("An error occurred while add a repository collaborator on github!",
                    e, DateTime.Now, "Server", "Service_GHCollaborator_AddCollaborator");
            }
        }
    }
}
