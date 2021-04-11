using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nococid_API.Data.Models
{
    public class Role : TableBase
    {
        public string Name { get; set; }

        [JsonIgnore]
        public virtual ICollection<Permission> Permissions { get; set; }
    }
}
