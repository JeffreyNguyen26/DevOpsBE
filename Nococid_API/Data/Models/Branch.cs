using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Nococid_API.Data.Models
{
    public class Branch : TableBase
    {
        public string Name { get; set; }
        public string Sha { get; set; }
        public bool IsDefault { get; set; }
        public bool IsDelete { get; set; }

        public Guid? RepositoryId { get; set; }

        [JsonIgnore]
        [ForeignKey("RepositoryId")]
        public virtual Repository Repository { get; set; }

        [JsonIgnore]
        public virtual ICollection<Commit> Commits { get; set; }
    }
}
