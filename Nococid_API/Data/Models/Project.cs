using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Nococid_API.Data.Models
{
    public class Project : TableBase
    {
        public string Name { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsDelete { get; set; }

        public Guid? ProjectTypeId { get; set; }

        [JsonIgnore]
        [ForeignKey("ProjectTypeId")]
        public virtual ProjectType ProjectType { get; set; }

        [JsonIgnore]
        public virtual ICollection<ProjectTool> ProjectTools { get; set; }
        [JsonIgnore]
        public virtual ICollection<Permission> Permissions { get; set; }
        [JsonIgnore]
        public virtual ICollection<Sprint> Sprints { get; set; }
        [JsonIgnore]
        public virtual ICollection<ProjectFramework> ProjectFrameworks { get; set; }
        [JsonIgnore]
        public virtual ICollection<ProjectRepository> ProjectRepositories { get; set; }
    }
}
