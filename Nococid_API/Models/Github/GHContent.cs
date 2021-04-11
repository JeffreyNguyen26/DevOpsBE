using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nococid_API.Models.Github
{
    public class GHCreateFile
    {
        [JsonProperty("message")]
        public string Message { get; set; }
        [JsonProperty("content")]
        public string Content { get; set; }
        [JsonProperty("branch")]
        public string Branch { get; set; }
    }

    public class GHUpdateFile : GHCreateFile
    {
        [JsonProperty("sha")]
        public string Sha { get; set; }
    }

    public class GHFile
    {
        public string Type { get; set; }
        public string Name { get; set; }
        public string Sha { get; set; }
    }

    public class GHFileDetail : GHFile
    {
        public string Content { get; set; }
    }
    
    public class GHDeleteFile
    {
        [JsonProperty("message")]
        public string Message { get; set; }
        [JsonProperty("sha")]
        public string Sha { get; set; }
        [JsonProperty("branch")]
        public string Branch { get; set; }
    }
}
