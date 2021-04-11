using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Nococid_API.Data.Models
{
    public class Repository : TableBase
    {
        public string Name { get; set; }
        public string ThirdPartyRepositoryId { get; set; }
        public string HookId { get; set; }
        public string Languages { get; set; }

        [JsonIgnore]
        public virtual ICollection<Branch> Branches { get; set; }
        [JsonIgnore]
        public virtual ICollection<Collaborator> Collaborators { get; set; }
        [JsonIgnore]
        public virtual ICollection<ProjectRepository> ProjectRepositories { get; set; }
    }
}
