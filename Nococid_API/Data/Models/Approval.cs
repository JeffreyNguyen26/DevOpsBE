using Newtonsoft.Json;
using Nococid_API.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Nococid_API.Data.Models
{
    public class Approval : TableBase
    {
        public StageEnum StageCode { get; set; }

        public Guid? SprintId { get; set; }

        [JsonIgnore]
        [ForeignKey("SprintId")]
        public virtual Sprint Sprint { get; set; }
    }
}
