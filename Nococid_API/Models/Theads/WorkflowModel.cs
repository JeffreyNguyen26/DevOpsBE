using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nococid_API.Models.Theads
{
    public class CircleCINococidWorkflowCM
    {
        public string CircleToken { get; set; }
        public string Username { get; set; }
        public Guid CircleWorkflowId { get; set; }
        public Guid SprintId { get; set; }
        public int TotalJob { get; set; }
    }
}
