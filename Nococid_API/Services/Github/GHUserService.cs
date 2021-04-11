using Nococid_API.Enums;
using Nococid_API.Models.Github;
using Nococid_API.Models.Nococid.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nococid_API.Services.Github
{
    public interface IGHUserService
    {
        GHAuthenticatedUserInfo GetAuthenticatedUserInfo(string uri);
    }
    public class GHUserService : GHServiceBase, IGHUserService
    {
        public GHUserService(IErrorHandlerService errorHandler, IHttpRequestService httpRequest) : base(errorHandler, httpRequest) { }

        public GHAuthenticatedUserInfo GetAuthenticatedUserInfo(string uri)
        {
            try
            {
                KeyValuePair<string, string>[] headers = new KeyValuePair<string, string>[]
                {
                    Accept_V3_Json
                };
                GHToken token = _httpRequest.Send<GHToken>(uri, HttpRequestMethod.Post, headers);

                uri = "https://api.github.com/user";
                headers = new KeyValuePair<string, string>[]
                {
                    Accept_V3_Json,
                    new KeyValuePair<string, string>("User-Agent", "toanldse63050"),
                    new KeyValuePair<string, string>("Authorization", "token " + token.Access_token)
                };
                GHUser user = _httpRequest.Send<GHUser>(uri, HttpRequestMethod.Get, headers);

                uri = "https://api.github.com/user/emails";
                string email = _httpRequest.Send<IList<GHEmail>>(uri, HttpRequestMethod.Get, headers)
                    .Where(e => e.Primary).FirstOrDefault().Email;

                return new GHAuthenticatedUserInfo
                {
                    Token = token,
                    User = user,
                    Email = email
                };
            }
            catch (Exception e)
            {
                throw e is ServerException ? e : _errorHandler.WriteLog("An error occurred while get the authenticated user information!",
                    e, DateTime.Now, "Server", "Service_GHUser_GetAuthenticatedUserInfo");
            }
        }
    }
}
