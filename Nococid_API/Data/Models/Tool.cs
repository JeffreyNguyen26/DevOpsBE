using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nococid_API.Data.Models
{
    public class Tool : TableBase
    {
        public string Name { get; set; }
        public string ToolType { get; set; }

        [JsonIgnore]
        public virtual ICollection<Account> Accounts { get; set; }
        [JsonIgnore]
        public virtual ICollection<Executor> Executors { get; set; }
        [JsonIgnore]
        public virtual ICollection<ProjectTool> ProjectTools { get; set; }
        [JsonIgnore]
        public virtual ICollection<Pipeline> Pipelines { get; set; }
    }
}
