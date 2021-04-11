using Nococid_API.Data;
using Nococid_API.Data.Models;
using Nococid_API.Enums;
using Nococid_API.Models.Github;
using Nococid_API.Models.Https;
using Nococid_API.Models.Nococid.Errors;
using Nococid_API.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nococid_API.Services.Github
{
    public interface IGHAuthService
    {
        string GetAccessTokenUri(string code, string state, out KeyValuePair<string, Guid[]> redirect_uri_user_id_pair);
        string GetRedirectUri(string nococid_redirect_uri, Guid admin_user_id, Guid user_id);
        void HasState(string state);
        void RemoveState(string state);
    }
    public class GHAuthService : GHServiceBase, IGHAuthService
    {
        private readonly object lockObj = new object();
        private int count = 0;
        private readonly IDictionary<string, KeyValuePair<string, Guid[]>> Mapper = new Dictionary<string, KeyValuePair<string, Guid[]>>();

        public GHAuthService(IErrorHandlerService errorHandler, IHttpRequestService httpRequest) : base(errorHandler, httpRequest) { }

        public string GetAccessTokenUri(string code, string state, out KeyValuePair<string, Guid[]> redirect_uri_user_id_pair)
        {
            try
            {
                if (string.IsNullOrEmpty(code) || string.IsNullOrEmpty(state))
                {
                    throw new RequestException
                    {
                        Error = new HttpResponseError
                        {
                            StatusCode = 400,
                            Detail = new HttpResponseErrorDetail
                            {
                                Message = "Could not execute!",
                                InnerMessage = "'Code' and 'state' parameter must not empty"
                            }
                        }
                    };
                }

                bool result = Mapper.TryGetValue(state, out KeyValuePair<string, Guid[]> value);
                if (!result)
                {
                    throw new RequestException
                    {
                        Error = new HttpResponseError
                        {
                            StatusCode = 400,
                            Detail = new HttpResponseErrorDetail
                            {
                                Message = "Could not execute!",
                                InnerMessage = "'State' with value '" + state + "' is invalid!"
                            }
                        }
                    };
                }

                redirect_uri_user_id_pair = value;
                return "https://github.com/login/oauth/access_token?" +
                    "client_id=da019b140cc63e523227&" +
                    "client_secret=379c26b6583fc2c1b61833311203decf6ef3616b&" +
                    "redirect_uri=http://toan0701.ddns.net:9080/Nococid/api/Auth/login/github/code&" +
                    "state=" + state + "&" +
                    "code=" + code;
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred while get the redirect uri after login success!",
                    e, DateTime.Now, "Server", "Service_GHAuth_GetRedirectUri");
            }
        }

        public string GetRedirectUri(string nococid_redirect_uri, Guid admin_user_id, Guid user_id)
        {
            try
            {
                lock (lockObj)
                {
                    string state = "NoCoCid" + count++ + StringUtils.GenerateRandomString(40);
                    Mapper.Add(state, new KeyValuePair<string, Guid[]>(nococid_redirect_uri, new Guid[] { admin_user_id, user_id }));

                    return "https://github.com/login/oauth/authorize?" +
                    "client_id=da019b140cc63e523227&" +
                    "scope=repo,user,delete_repo&" +
                    "redirect_uri=http://toan0701.ddns.net:9080/Nococid/api/Auth/login/github/code&" +
                    "state=" + state;
                }
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred while get the redirect uri!",
                    e, DateTime.Now, "Server", "Service_GHAuth_GetRedirectUri");
            }
        }

        public void HasState(string state)
        {
            try
            {
                bool result = Mapper.ContainsKey(state);
                if (!result)
                {
                    throw new RequestException
                    {
                        Error = new HttpResponseError
                        {
                            StatusCode = 400,
                            Detail = new HttpResponseErrorDetail
                            {
                                Message = "Could not execute!",
                                InnerMessage = "No state '" + state + "' found"
                            }
                        }
                    };
                }
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred while check state!",
                    e, DateTime.Now, "Server", "Service_GHAuth_HasState");
            }
        }

        public void RemoveState(string state)
        {
            try
            {
                Mapper.Remove(state);
            }
            catch (Exception e)
            {
                throw e is RequestException ? e : _errorHandler.WriteLog("An error occurred while remove state!",
                    e, DateTime.Now, "Server", "Service_GHAuth_RemoveState");
            }
        }
    }
}
