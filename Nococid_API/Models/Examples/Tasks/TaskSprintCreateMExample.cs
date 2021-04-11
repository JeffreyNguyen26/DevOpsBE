using Nococid_API.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nococid_API.Models.Examples.Tasks
{
    public class TaskSprintCreateMExample
    {
        public Guid? SprintId = null;
        public IList<StageEnum> Approvals = new List<StageEnum>
        {
            StageEnum.Build,
            StageEnum.Test
        };
        public DateTime? StartDate = DateTime.Now;
        public DateTime? EndDate = DateTime.Now.AddDays(14);
        public IList<TaskSideCreateMExample> Sides = new List<TaskSideCreateMExample>
        {
            new TaskSideCreateMExample
            {
                Side = "Client",
                Tasks = new List<TaskCreateMExample>
                {
                    new TaskCreateMExample
                    {
                        Name = "Add new sprint and tasks for employees",
                        Detail = "Only PM can do this action. A task contains name, detail, assigned employee, a side contains side name and a list of tasks",
                        AssignedUserId = Guid.Empty
                    },
                    new TaskCreateMExample
                    {
                        Name = "Delete a task",
                        Detail = "Only project manager can delete a task",
                        AssignedUserId = Guid.Empty
                    }
                }
            },
            new TaskSideCreateMExample
            {
                Side = "Database",
                Tasks = new List<TaskCreateMExample>
                {
                    new TaskCreateMExample
                    {
                        Name = "Design DevOps Stages tables",
                        Detail = "Each table represent for a stage of DevOps process",
                        AssignedUserId = Guid.Empty
                    }
                }
            }
        };
    }

    public class TaskSideCreateMExample
    {
        public string Side { get; set; }
        public IList<TaskCreateMExample> Tasks { get; set; }
    }

    public class TaskCreateMExample
    {
        public string Name { get; set; }
        public string Detail { get; set; }
        public Guid AssignedUserId { get; set; }
    }
}
