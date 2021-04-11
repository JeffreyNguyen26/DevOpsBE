using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nococid_API.Models.Nococid
{
    public class ProjectM
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public ProjectTypeM ProjectType { get; set; }
        public UserM Owner { get; set; }
    }

    public class ProjectVM
    {

    }

    public class ProjectDM
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public ProjectTypeM ProjectType { get; set; }
        public IList<UserPermissionM> Members { get; set; }
    }

    public class ProjectCreateM
    {
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public Guid ProjectTypeId { get; set; }
    }

    public class ProjectTechnologySetupM
    {
        public string Language { get; set; }
        public string Framework { get; set; }
        public Guid RepositoryId { get; set; }

        public IList<Guid> ToolIds { get; set; }
    }

    public class ProjectTechnologyM
    {
        public string Language { get; set; }
        public string Framework { get; set; }
        public RepositoryM Repository { get; set; }

        public IList<ToolM> Tools { get; set; }
    }

    public class ProjectAssignM
    {
        public int MyProperty { get; set; }
    }

    public class ProjectLastSprintM : ProjectM
    {
        public int TotalSprint { get; set; }
        public SprintM LastSprint { get; set; }
    }
}
