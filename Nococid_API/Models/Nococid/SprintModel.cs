using Nococid_API.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nococid_API.Models.Nococid
{
    public class SprintM
    {
        public Guid Id { get; set; }
        public int No { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public Stage Stage { get; set; }
        public Stage NextStage { get; set; }
        public bool IsRequireApproval { get; set; }
    }

    public class SprintDM
    {
        public Guid Id { get; set; }
        public int No { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public Stage Stage { get; set; }
        public IList<Stage> Approvals { get; set; }
    }

    public class SprintUpdateM
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public IList<StageEnum> Approvals { get; set; }
    }

    public class SprintTasksM : SprintM
    {
        public IList<TaskDM> Tasks { get; set; }
    }

    public class Stage
    {
        public StageEnum StageCode { get; set; }
        public string Name { get; set; }
    }

    public class SprintSubmissionsM
    {
        public Guid Id { get; set; }
        public int No { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public Stage Stage { get; set; }
        public IList<SubmissionDM> Submissions { get; set; }
    }
}
