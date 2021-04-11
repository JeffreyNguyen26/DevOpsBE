using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Nococid_API.Data.Models
{
    public class Framework : TableBase
    {
        public string Name { get; set; }
        
        public Guid? LanguageId { get; set; }

        [JsonIgnore]
        [ForeignKey("LanguageId")]
        public virtual Language Language { get; set; }

        [JsonIgnore]
        public virtual ICollection<ProjectFramework> ProjectFrameworks { get; set; }
    }
}
