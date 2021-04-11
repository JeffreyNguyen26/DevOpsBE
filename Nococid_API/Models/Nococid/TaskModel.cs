using Nococid_API.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nococid_API.Models.Nococid
{
    public class TaskM
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Detail { get; set; }
        public string Status { get; set; }
        public string Side { get; set; }
    }

    public class TaskDM : TaskM
    {
        public UserM AssignedUser { get; set; }
    }

    public class TaskMoreDM : TaskM
    {
        public IList<ProjectToolM> Tools { get; set; }
        public IList<TaskLanguagesM> Languages { get; set; }
        public IList<string> VSCAccountNames { get; set; }
        public string RepositoryName { get; set; }
    }

    public class TaskLanguagesM
    {
        public string Name { get; set; }
        public IList<TaskFrameworkM> Frameworks { get; set; }
    }

    public class TaskFrameworkM
    {
        public string Name { get; set; }
    }

    public class TaskImplementingM
    {
        public ProjectM Project { get; set; }
        public SprintTasksM Sprint { get; set; }
    }

    public class TaskProjectFrameworkM
    {
        public Guid Id { get; set; }
        public string Framework { get; set; }
        public string Language { get; set; }
    }

    public class TaskCommitDM : TaskM
    {
        public TaskProjectM Project { get; set; }
        public TaskSprintM Sprint { get; set; }
    }

    public class TaskProjectM
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }

    public class TaskSprintM
    {
        public Guid Id { get; set; }
        public int No { get; set; }
    }

    public class TaskCreateM
    {
        public string Name { get; set; }
        public string Detail { get; set; }
        public Guid AssignedUserId { get; set; }
    }

    public class TaskSideCreateM
    {
        public string Side { get; set; }
        public IList<TaskCreateM> Tasks { get; set; }
    }

    public class TaskSprintCreateM
    {
        public Guid? SprintId { get; set; }
        public IList<StageEnum> Approvals { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public IList<TaskSideCreateM> Sides { get; set; }
    }

    public class TaskSubmitionM
    {
        public Guid CommitId { get; set; }
        public IList<Guid> TaskIds { get; set; }
    }
}
