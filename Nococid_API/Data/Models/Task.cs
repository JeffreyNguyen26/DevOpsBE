using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Nococid_API.Data.Models
{
    public class Task : TableBase
    {
        public string Name { get; set; }
        public string Detail { get; set; }
        public string Status { get; set; }
        public bool IsDelete { get; set; }
        public string Side { get; set; }

        public Guid? SprintId { get; set; }
        [JsonIgnore]
        [ForeignKey("SprintId")]
        public virtual Sprint Sprint { get; set; }
        public Guid? UserId { get; set; }
        [JsonIgnore]
        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        [JsonIgnore]
        public virtual ICollection<TaskSubmission> TaskSubmissions { get; set; }
    }
}