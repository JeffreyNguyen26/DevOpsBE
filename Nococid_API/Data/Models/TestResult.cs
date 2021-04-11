using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Nococid_API.Data.Models
{
    public class TestResult : TableBase
    {
        public string Name { get; set; }
        public string ClassName { get; set; }
        public bool Passed { get; set; }
        public TimeSpan? Duration { get; set; }
        public string Message { get; set; }

        public Guid? TestId { get; set; }

        [JsonIgnore]
        [ForeignKey("TestId")]
        public virtual Test Test { get; set; }
    }
}
