using Nococid_API.Models.Nococid.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nococid_API.Services.Github
{
    public interface IGHInvitationService
    {
        void Accept(string gh_username, string access_token, int gh_invitation_id);
    }

    public class GHInvitationService : GHServiceBase, IGHInvitationService
    {
        public GHInvitationService(IErrorHandlerService errorHandler, IHttpRequestService httpRequest) : base(errorHandler, httpRequest) { }

        public void Accept(string gh_username, string access_token, int gh_invitation_id)
        {
            try
            {
                _httpRequest.Send<object>(
                    "https://api.github.com/user/repository_invitations/" + gh_invitation_id.ToString(),
                    Enums.HttpRequestMethod.Patch, new KeyValuePair<string, string>[]
                    {
                        Accept_V3_Json,
                        new KeyValuePair<string, string>("User-Agent", gh_username),
                        new KeyValuePair<string, string>("Authorization", "token " + access_token)
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
