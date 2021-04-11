using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Nococid_API.Data.Models
{
    public class Account : TableBase
    {
        public string ThirdPartyAccountId { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string AvatarUrl { get; set; }
        public string AccessToken { get; set; }
        public bool IsMain { get; set; }

        public Guid? UserId { get; set; }
        [JsonIgnore]
        [ForeignKey("UserId")]
        public virtual User User { get; set; }
        public Guid? ToolId { get; set; }
        [JsonIgnore]
        [ForeignKey("ToolId")]
        public virtual Tool Tool { get; set; }

        [JsonIgnore]
        public virtual ICollection<Collaborator> Collaborators { get; set; }
        [JsonIgnore]
        public virtual ICollection<ProjectTool> ProjectTools { get; set; }
        [JsonIgnore]
        public virtual ICollection<Commit> Commits { get; set; }
    }
}
