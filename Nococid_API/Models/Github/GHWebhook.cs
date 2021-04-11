using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nococid_API.Models.Github
{
    public class GHWebhookCreation
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("events")]
        public string[] Events { get; set; }
        [JsonProperty("active")]
        public bool Active { get; set; }
        [JsonProperty("config")]
        public GHWebhookCreationConfig Config { get; set; }
    }

    public class GHWebhookCreationConfig
    {
        [JsonProperty("url")]
        public string Url { get; set; }
        [JsonProperty("content_type")]
        public string Content_type { get; set; }
        [JsonProperty("secret")]
        public string Secret { get; set; }
        [JsonProperty("insecure_ssl")]
        public string Insecure_ssl { get; set; }
        [JsonProperty("token")]
        public string Token { get; set; }
        [JsonProperty("digest")]
        public string Digest { get; set; }
    }

    public class GHWebhookCreationSuccess
    {
        public int Id { get; set; }
    }

    public class GHWebhook
    {
        public string Type { get; set; }
        public int Id { get; set; }
        public bool Active { get; set; }
        public GHWebhookConfig Config { get; set; }
    }

    public class GHWebhookConfig
    {
        public string Url { get; set; }
    }

    public class GHWebhookPayload
    {
        //branch
        public string Ref { get; set; }
        //previous commit id
        public string Before { get; set; }
        //current commit id
        public string After { get; set; }
        public GHWebhookPayloadRepository Repository { get; set; }
        public GHWebhookPayloadCommit Head_commit { get; set; }
    }

    public class GHWebhookPayloadRepository
    {
        public int Id { get; set; }
    }

    public class GHWebhookPayloadCommit
    {
        public string Message { get; set; }
        public string Timestamp { get; set; }
        public GHWebhookPayloadCommitAuthor Author { get; set; }
    }

    public class GHWebhookPayloadCommitAuthor
    {
        public string Username { get; set; }
    }
}
