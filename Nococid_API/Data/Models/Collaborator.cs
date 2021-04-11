using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Nococid_API.Data.Models
{
    public class Collaborator : TableBase
    {
        public Guid? AccountId { get; set; }
        public Guid? RepositoryId { get; set; }
        public Guid? OwnerId { get; set; }

        [JsonIgnore]
        [ForeignKey("AccountId")]
        public virtual Account Account { get; set; }
        [JsonIgnore]
        [ForeignKey("RepositoryId")]
        public virtual Repository Repository { get; set; }
    }
}
