using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nococid_API.Data.Models
{
    public class Language : TableBase
    {
        public string Name { get; set; }
        public string Side { get; set; }

        [JsonIgnore]
        public virtual ICollection<Framework> Frameworks { get; set; }
    }
}
