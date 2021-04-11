using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Nococid_API.Data.Models
{
    public class Permission : TableBase
    {
        public Guid? UserId { get; set; }
        public Guid? RoleId { get; set; }
        public Guid? ProjectId { get; set; }

        [JsonIgnore]
        [ForeignKey("UserId")]
        public virtual User User { get; set; }
        [JsonIgnore]
        [ForeignKey("RoleId")]
        public virtual Role Role { get; set; }
        [JsonIgnore]
        [ForeignKey("ProjectId")]
        public virtual Project Project { get; set; }
    }
}
