using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nococid_API.Models.Github
{
    public class GHReferenceCreation
    {
        [JsonProperty("ref")]
        public string Ref { get; set; }
        [JsonProperty("sha")]
        public string Sha { get; set; }
    }

    public class GHReferenceCreationSuccess
    {
        public string Ref { get; set; }
        public GHReferenceCreationSuccessObject Object { get; set; }
    }

    public class GHReferenceCreationSuccessObject
    {
        public string Sha { get; set; }
    }
}
