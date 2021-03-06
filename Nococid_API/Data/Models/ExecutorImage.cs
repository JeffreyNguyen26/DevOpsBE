using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Nococid_API.Data.Models
{
    public class ExecutorImage : TableBase
    {
        public string Name { get; set; }
        public string Value { get; set; }

        public Guid? ExecutorId { get; set; }

        [JsonIgnore]
        [ForeignKey("ExecutorId")]
        public virtual Executor Executor { get; set; }
    }
}
