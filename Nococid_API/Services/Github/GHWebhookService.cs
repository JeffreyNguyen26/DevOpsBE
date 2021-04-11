using Nococid_API.Enums;
using Nococid_API.Models.Github;
using Nococid_API.Models.Nococid.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nococid_API.Services.Github
{
    public interface IGHWebhookService
    {
        int Create(string gh_username, string repository_name, string access_token);
        IList<GHWebhook> GetAll(string gh_username, string repository_name, string access_token);
    }

    public class GHWebhookService : GHServiceBase, IGHWebhookService
    {
        private readonly string hook_url = "http://toan0701.ddns.net:9080/Nococid/api/Listeners/Github/webhook/payload";

        public GHWebhookService(IErrorHandlerService errorHandler, IHttpRequestService httpRequest) : base(errorHandler, httpRequest) { }

        public int Create(string gh_username, string repository_name, string access_token)
        {
            try
            {
                GHWebhookCreation data = new GHWebhookCreation
                {
                    Name = "web",
                    Events = new string[] { "push", "pull_request" },
                    Active = true,
                    Config = new GHWebhookCreationConfig
                    {
                        Url = "http://toan0701.ddns.net:9080/Nococid/api/Listeners/Github/webhook/payload",
                        Content_type = "json",
                        Insecure_ssl = "1"//SSL verification is not performed
                    }
                };
                return _httpRequest.Send<GHWebhookCreationSuccess, GHWebhookCreation>(
                    "https://api.github.com/repos/" + gh_username + "/" + repository_name + "/hooks",
                    HttpRequestMethod.Post, new KeyValuePair<string, string>[]
                    {
                        Accept_V3_Json,
                        new KeyValuePair<string, string>("User-Agent", gh_username),
                        new KeyValuePair<string, string>("Authorization", "token " + access_token)
                    }, data
                ).Id;
            }
            catch (Exception e)
            {
                if (e is ServerException se)
                {
                    if (se.InnerException.Message.Contains("Hook already exists on this repository"))
                    {
                        IList<GHWebhook> hooks = GetAll(gh_username, repository_name, access_token);
                        GHWebhook hook = hooks.FirstOrDefault(h => h.Config.Url.Equals(hook_url));
                        if (hook != null)
                        {
                            return hook.Id;
                        }
                    }
                    throw e;
                }
                throw _errorHandler.WriteLog("An error occurred!",
                    e, DateTime.Now, "Server", "Service-GHWebhook-Create");
            }
        }

        public IList<GHWebhook> GetAll(string gh_username, string repository_name, string access_token)
        {
            try
            {
                return _httpRequest.Send<IList<GHWebhook>>(
                    "https://api.github.com/repos/" + gh_username + "/" + repository_name + "/hooks",
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
                throw e is ServerException ? e : _errorHandler.WriteLog("An error occurred!",
                    e, DateTime.Now, "Server", "Service-GHWebhook-GetAll");
            }
        }
    }
}
