using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nococid_API.Models.CircleCIs
{
    public class CircleCITriggerPipeline
    {
        [JsonProperty("branch")]
        public string Branch { get; set; }
        [JsonProperty("parameters")]
        public IDictionary<string, string> Parameters { get; set; }
    }

    public class CircleCIPipelineM
    {
        public CircleCIPipelineVersionControlM Vcs { get; set; }
    }

    public class CircleCIPipelineVersionControlM
    {
        //current commit id
        public string Revision { get; set; }
    }
}
