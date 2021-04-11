using Nococid_API.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nococid_API.Models.Examples.Sprints
{
    public class SprintUpdateMExample
    {
        public DateTime? StartDate = DateTime.Now;
        public DateTime? EndDate = DateTime.Now.AddDays(14);
        public IList<StageEnum> Approvals = new List<StageEnum> { StageEnum.Build, StageEnum.Test };
    }
}
