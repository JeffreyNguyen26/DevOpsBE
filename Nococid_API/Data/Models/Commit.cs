using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Nococid_API.Data.Models
{
    public class Commit : TableBase
    {
        public string PreviousCommitId { get; set; }
        public string CurrentCommitId { get; set; }
        public string Message { get; set; }
        public string MessageBody { get; set; }
        public DateTime? CommitTime { get; set; }
        public bool IsSubmit { get; set; }

        public Guid? AccountId { get; set; }
        public Guid? BranchId { get; set; }

        [JsonIgnore]
        [ForeignKey("BranchId")]
        public Branch Branch { get; set; }
        [JsonIgnore]
        [ForeignKey("AccountId")]
        public Account Account { get; set; }

        [JsonIgnore]
        public virtual ICollection<Submission> Submissions { get; set; }
    }
}
