using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nococid_API.Models.Herokus
{
    public class HerokuAppM
    {
        public string Name { get; set; }
    }

    public class HerokuAppCreateM
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("region")]
        public string Region { get; set; }
        [JsonProperty("stack")]
        public string Stack { get; set; }
    }

    public class HerokuAppCreateSuccessM
    {
        public string Web_url { get; set; }
    }
}
