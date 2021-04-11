using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nococid_API.Models.Nococid.Configuration
{
    public class StepCreateM
    {
        public string Stage { get; set; }
        public string Name { get; set; }
        public string Text { get; set; }
        public string ReplacedText { get; set; }
    }

    public class StepUpdateM
    {
        public string Stage { get; set; }
        public string Name { get; set; }
        public string Text { get; set; }
        public string ReplacedText { get; set; }
    }
}
