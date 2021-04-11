using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nococid_API.Models.Github
{
    public class GHCollaboratorCreateM
    {
        [JsonProperty("permissions")]
        public string Permissions { get; set; }
    }

    public class GHInvitationM
    {
        public int Id { get; set; }
    }
}
