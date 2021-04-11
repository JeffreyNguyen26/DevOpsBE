using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nococid_API.Models.Theads
{
    public class CircleCINococidPipelineCM
    {
        public string Username { get; set; }
        public string Repository { get; set; }
        public int CircleJobNum { get; set; }
        public string CircleToken { get; set; }
        public Guid CircleWorkflowId { get; set; }
        public string JobName { get; set; }
    }
}
